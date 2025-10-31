using ClinicalManagementSystem2025.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace ClinicalManagementSystem2025.Repository
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly string _connectionString;

        public PrescriptionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MVCConnectionString")
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string is missing");
        }

        public async Task<Prescription> AddPrescriptionAsync(Prescription prescription)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Try with Status column first
                const string sqlWithStatus = @"
                    INSERT INTO TblPrescriptions (AppointmentId, PatientId, DoctorId, Diagnosis, PrescriptionDate, Notes, Status)
                    OUTPUT INSERTED.PrescriptionId
                    VALUES (@AppointmentId, @PatientId, @DoctorId, @Diagnosis, @PrescriptionDate, @Notes, @Status)";

                using var command = new SqlCommand(sqlWithStatus, connection);

                command.Parameters.AddWithValue("@AppointmentId", prescription.AppointmentId);
                command.Parameters.AddWithValue("@PatientId", prescription.PatientId);
                command.Parameters.AddWithValue("@DoctorId", prescription.DoctorId);
                command.Parameters.AddWithValue("@Diagnosis", prescription.Diagnosis ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PrescriptionDate", prescription.PrescriptionDate);
                command.Parameters.AddWithValue("@Notes", prescription.Notes ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Status", prescription.Status ?? (object)DBNull.Value);

                var result = await command.ExecuteScalarAsync();
                prescription.PrescriptionId = Convert.ToInt32(result);

                return prescription;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid column name"))
            {
                // Fallback to insert without Status column
                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();

                    const string fallbackSql = @"
                        INSERT INTO TblPrescriptions (AppointmentId, PatientId, DoctorId, Diagnosis, PrescriptionDate, Notes)
                        OUTPUT INSERTED.PrescriptionId
                        VALUES (@AppointmentId, @PatientId, @DoctorId, @Diagnosis, @PrescriptionDate, @Notes)";

                    using var command = new SqlCommand(fallbackSql, connection);

                    command.Parameters.AddWithValue("@AppointmentId", prescription.AppointmentId);
                    command.Parameters.AddWithValue("@PatientId", prescription.PatientId);
                    command.Parameters.AddWithValue("@DoctorId", prescription.DoctorId);
                    command.Parameters.AddWithValue("@Diagnosis", prescription.Diagnosis ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PrescriptionDate", prescription.PrescriptionDate);
                    command.Parameters.AddWithValue("@Notes", prescription.Notes ?? (object)DBNull.Value);

                    var result = await command.ExecuteScalarAsync();
                    prescription.PrescriptionId = Convert.ToInt32(result);

                    return prescription;
                }
                catch
                {
                    throw new Exception($"Error adding prescription: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding prescription: {ex.Message}", ex);
            }
        }

        public async Task<Prescription?> GetPrescriptionByIdAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Try with new columns first
                const string sqlWithStatus = @"
                    SELECT PrescriptionId, AppointmentId, PatientId, DoctorId, Diagnosis, PrescriptionDate, Notes, Status, FulfilledDate
                    FROM TblPrescriptions
                    WHERE PrescriptionId = @PrescriptionId";

                using var command = new SqlCommand(sqlWithStatus, connection);
                command.Parameters.AddWithValue("@PrescriptionId", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Prescription
                    {
                        PrescriptionId = reader.GetInt32(reader.GetOrdinal("PrescriptionId")),
                        AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        Diagnosis = reader.IsDBNull(reader.GetOrdinal("Diagnosis")) ? null : reader.GetString(reader.GetOrdinal("Diagnosis")),
                        PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? "Pending" : reader.GetString(reader.GetOrdinal("Status")),
                        FulfilledDate = reader.IsDBNull(reader.GetOrdinal("FulfilledDate")) ? null : reader.GetDateTime(reader.GetOrdinal("FulfilledDate"))
                    };
                }

                return null;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid column name"))
            {
                // Fallback to query without Status and FulfilledDate columns
                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();

                    const string fallbackSql = @"
                        SELECT PrescriptionId, AppointmentId, PatientId, DoctorId, Diagnosis, PrescriptionDate, Notes
                        FROM TblPrescriptions
                        WHERE PrescriptionId = @PrescriptionId";

                    using var command = new SqlCommand(fallbackSql, connection);
                    command.Parameters.AddWithValue("@PrescriptionId", id);

                    using var reader = await command.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        return new Prescription
                        {
                            PrescriptionId = reader.GetInt32(reader.GetOrdinal("PrescriptionId")),
                            AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                            PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                            DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                            Diagnosis = reader.IsDBNull(reader.GetOrdinal("Diagnosis")) ? null : reader.GetString(reader.GetOrdinal("Diagnosis")),
                            PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                            Status = "Pending" // Default status for older records
                        };
                    }

                    return null;
                }
                catch
                {
                    throw new Exception($"Error getting prescription: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting prescription: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync()
        {
            var prescriptions = new List<Prescription>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Try with new columns first
                const string sqlWithStatus = @"
                    SELECT PrescriptionId, AppointmentId, PatientId, DoctorId, Diagnosis, PrescriptionDate, Notes, Status, FulfilledDate
                    FROM TblPrescriptions
                    ORDER BY PrescriptionDate DESC";

                using var command = new SqlCommand(sqlWithStatus, connection);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    prescriptions.Add(new Prescription
                    {
                        PrescriptionId = reader.GetInt32(reader.GetOrdinal("PrescriptionId")),
                        AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        Diagnosis = reader.IsDBNull(reader.GetOrdinal("Diagnosis")) ? null : reader.GetString(reader.GetOrdinal("Diagnosis")),
                        PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? "Pending" : reader.GetString(reader.GetOrdinal("Status")),
                        FulfilledDate = reader.IsDBNull(reader.GetOrdinal("FulfilledDate")) ? null : reader.GetDateTime(reader.GetOrdinal("FulfilledDate"))
                    });
                }

                return prescriptions;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid column name"))
            {
                // Fallback to query without Status and FulfilledDate columns
                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();

                    const string fallbackSql = @"
                        SELECT PrescriptionId, AppointmentId, PatientId, DoctorId, Diagnosis, PrescriptionDate, Notes
                        FROM TblPrescriptions
                        ORDER BY PrescriptionDate DESC";

                    using var command = new SqlCommand(fallbackSql, connection);

                    using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        prescriptions.Add(new Prescription
                        {
                            PrescriptionId = reader.GetInt32(reader.GetOrdinal("PrescriptionId")),
                            AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                            PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                            DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                            Diagnosis = reader.IsDBNull(reader.GetOrdinal("Diagnosis")) ? null : reader.GetString(reader.GetOrdinal("Diagnosis")),
                            PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                            Status = "Pending" // Default status for older records
                        });
                    }

                    return prescriptions;
                }
                catch
                {
                    throw new Exception($"Error getting all prescriptions: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all prescriptions: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByPatientAsync(int patientId)
        {
            var prescriptions = new List<Prescription>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Try with new columns first
                const string sqlWithStatus = @"
                    SELECT PrescriptionId, AppointmentId, PatientId, DoctorId, Diagnosis, PrescriptionDate, Notes, Status, FulfilledDate
                    FROM TblPrescriptions
                    WHERE PatientId = @PatientId
                    ORDER BY PrescriptionDate DESC";

                using var command = new SqlCommand(sqlWithStatus, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    prescriptions.Add(new Prescription
                    {
                        PrescriptionId = reader.GetInt32(reader.GetOrdinal("PrescriptionId")),
                        AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        Diagnosis = reader.IsDBNull(reader.GetOrdinal("Diagnosis")) ? null : reader.GetString(reader.GetOrdinal("Diagnosis")),
                        PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? "Pending" : reader.GetString(reader.GetOrdinal("Status")),
                        FulfilledDate = reader.IsDBNull(reader.GetOrdinal("FulfilledDate")) ? null : reader.GetDateTime(reader.GetOrdinal("FulfilledDate"))
                    });
                }

                return prescriptions;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid column name"))
            {
                // Fallback to query without Status and FulfilledDate columns
                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();

                    const string fallbackSql = @"
                        SELECT PrescriptionId, AppointmentId, PatientId, DoctorId, Diagnosis, PrescriptionDate, Notes
                        FROM TblPrescriptions
                        WHERE PatientId = @PatientId
                        ORDER BY PrescriptionDate DESC";

                    using var command = new SqlCommand(fallbackSql, connection);
                    command.Parameters.AddWithValue("@PatientId", patientId);

                    using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        prescriptions.Add(new Prescription
                        {
                            PrescriptionId = reader.GetInt32(reader.GetOrdinal("PrescriptionId")),
                            AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                            PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                            DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                            Diagnosis = reader.IsDBNull(reader.GetOrdinal("Diagnosis")) ? null : reader.GetString(reader.GetOrdinal("Diagnosis")),
                            PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                            Status = "Pending" // Default status for older records
                        });
                    }

                    return prescriptions;
                }
                catch
                {
                    throw new Exception($"Error getting prescriptions for patient: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting prescriptions for patient: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorAsync(int doctorId)
        {
            var prescriptions = new List<Prescription>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Try with new columns first
                const string sqlWithStatus = @"
                    SELECT PrescriptionId, AppointmentId, PatientId, DoctorId, Diagnosis, PrescriptionDate, Notes, Status, FulfilledDate
                    FROM TblPrescriptions
                    WHERE DoctorId = @DoctorId
                    ORDER BY PrescriptionDate DESC";

                using var command = new SqlCommand(sqlWithStatus, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    prescriptions.Add(new Prescription
                    {
                        PrescriptionId = reader.GetInt32(reader.GetOrdinal("PrescriptionId")),
                        AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        Diagnosis = reader.IsDBNull(reader.GetOrdinal("Diagnosis")) ? null : reader.GetString(reader.GetOrdinal("Diagnosis")),
                        PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? "Pending" : reader.GetString(reader.GetOrdinal("Status")),
                        FulfilledDate = reader.IsDBNull(reader.GetOrdinal("FulfilledDate")) ? null : reader.GetDateTime(reader.GetOrdinal("FulfilledDate"))
                    });
                }

                return prescriptions;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid column name"))
            {
                // Fallback to query without Status and FulfilledDate columns
                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();

                    const string fallbackSql = @"
                        SELECT PrescriptionId, AppointmentId, PatientId, DoctorId, Diagnosis, PrescriptionDate, Notes
                        FROM TblPrescriptions
                        WHERE DoctorId = @DoctorId
                        ORDER BY PrescriptionDate DESC";

                    using var command = new SqlCommand(fallbackSql, connection);
                    command.Parameters.AddWithValue("@DoctorId", doctorId);

                    using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        prescriptions.Add(new Prescription
                        {
                            PrescriptionId = reader.GetInt32(reader.GetOrdinal("PrescriptionId")),
                            AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                            PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                            DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                            Diagnosis = reader.IsDBNull(reader.GetOrdinal("Diagnosis")) ? null : reader.GetString(reader.GetOrdinal("Diagnosis")),
                            PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                            Status = "Pending" // Default status for older records
                        });
                    }

                    return prescriptions;
                }
                catch
                {
                    throw new Exception($"Error getting prescriptions for doctor: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting prescriptions for doctor: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdatePrescriptionAsync(Prescription prescription)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    UPDATE TblPrescriptions
                    SET Diagnosis = @Diagnosis, Notes = @Notes, Status = @Status, FulfilledDate = @FulfilledDate
                    WHERE PrescriptionId = @PrescriptionId";

                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@PrescriptionId", prescription.PrescriptionId);
                command.Parameters.AddWithValue("@Diagnosis", prescription.Diagnosis ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Notes", prescription.Notes ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Status", prescription.Status ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FulfilledDate", prescription.FulfilledDate.HasValue ? (object)prescription.FulfilledDate.Value : DBNull.Value);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid column name"))
            {
                // If Status or FulfilledDate columns don't exist, update without them
                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();

                    const string fallbackSql = @"
                        UPDATE TblPrescriptions
                        SET Diagnosis = @Diagnosis, Notes = @Notes
                        WHERE PrescriptionId = @PrescriptionId";

                    using var command = new SqlCommand(fallbackSql, connection);

                    command.Parameters.AddWithValue("@PrescriptionId", prescription.PrescriptionId);
                    command.Parameters.AddWithValue("@Diagnosis", prescription.Diagnosis ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Notes", prescription.Notes ?? (object)DBNull.Value);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
                catch
                {
                    throw new Exception($"Error updating prescription: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating prescription: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeletePrescriptionAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = "DELETE FROM TblPrescriptions WHERE PrescriptionId = @PrescriptionId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PrescriptionId", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting prescription: {ex.Message}", ex);
            }
        }

        public async Task<PrescriptionDetail> AddPrescriptionDetailAsync(PrescriptionDetail prescriptionDetail)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    INSERT INTO TblPrescriptionDetails (PrescriptionId, MedicineId, Dosage, Frequency, Duration, Instructions, Quantity, MedicineName, Price)
                    OUTPUT INSERTED.PrescriptionDetailId
                    VALUES (@PrescriptionId, @MedicineId, @Dosage, @Frequency, @Duration, @Instructions, @Quantity, @MedicineName, @Price)";

                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@PrescriptionId", prescriptionDetail.PrescriptionId);
                command.Parameters.AddWithValue("@MedicineId", prescriptionDetail.MedicineId);
                command.Parameters.AddWithValue("@Dosage", prescriptionDetail.Dosage ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Frequency", prescriptionDetail.Frequency ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Duration", prescriptionDetail.Duration ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Instructions", prescriptionDetail.Instructions ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Quantity", prescriptionDetail.Quantity);
                command.Parameters.AddWithValue("@MedicineName", prescriptionDetail.MedicineName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Price", prescriptionDetail.Price);

                var result = await command.ExecuteScalarAsync();
                prescriptionDetail.PrescriptionDetailId = Convert.ToInt32(result);

                return prescriptionDetail;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid column name"))
            {
                // Fallback to insert without MedicineName and Price columns
                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();

                    const string fallbackSql = @"
                        INSERT INTO TblPrescriptionDetails (PrescriptionId, MedicineId, Dosage, Frequency, Duration, Instructions, Quantity)
                        OUTPUT INSERTED.PrescriptionDetailId
                        VALUES (@PrescriptionId, @MedicineId, @Dosage, @Frequency, @Duration, @Instructions, @Quantity)";

                    using var command = new SqlCommand(fallbackSql, connection);

                    command.Parameters.AddWithValue("@PrescriptionId", prescriptionDetail.PrescriptionId);
                    command.Parameters.AddWithValue("@MedicineId", prescriptionDetail.MedicineId);
                    command.Parameters.AddWithValue("@Dosage", prescriptionDetail.Dosage ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Frequency", prescriptionDetail.Frequency ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Duration", prescriptionDetail.Duration ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Instructions", prescriptionDetail.Instructions ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Quantity", prescriptionDetail.Quantity);

                    var result = await command.ExecuteScalarAsync();
                    prescriptionDetail.PrescriptionDetailId = Convert.ToInt32(result);

                    return prescriptionDetail;
                }
                catch
                {
                    throw new Exception($"Error adding prescription detail: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding prescription detail: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<PrescriptionDetail>> GetPrescriptionDetailsAsync(int prescriptionId)
        {
            var prescriptionDetails = new List<PrescriptionDetail>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT pd.PrescriptionDetailId, pd.PrescriptionId, pd.MedicineId, pd.Dosage, pd.Frequency,
                           pd.Duration, pd.Instructions, pd.Quantity, m.MedicineName, m.Price
                    FROM TblPrescriptionDetails pd
                    LEFT JOIN TblMedicines m ON pd.MedicineId = m.MedicineId
                    WHERE pd.PrescriptionId = @PrescriptionId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    prescriptionDetails.Add(new PrescriptionDetail
                    {
                        PrescriptionDetailId = reader.GetInt32(reader.GetOrdinal("PrescriptionDetailId")),
                        PrescriptionId = reader.GetInt32(reader.GetOrdinal("PrescriptionId")),
                        MedicineId = reader.GetInt32(reader.GetOrdinal("MedicineId")),
                        Dosage = reader.IsDBNull(reader.GetOrdinal("Dosage")) ? null : reader.GetString(reader.GetOrdinal("Dosage")),
                        Frequency = reader.IsDBNull(reader.GetOrdinal("Frequency")) ? null : reader.GetString(reader.GetOrdinal("Frequency")),
                        Duration = reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("Duration")),
                        Instructions = reader.IsDBNull(reader.GetOrdinal("Instructions")) ? null : reader.GetString(reader.GetOrdinal("Instructions")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        MedicineName = reader.IsDBNull(reader.GetOrdinal("MedicineName")) ? null : reader.GetString(reader.GetOrdinal("MedicineName")),
                        Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Price"))
                    });
                }

                return prescriptionDetails;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid column name"))
            {
                // Fallback query without MedicineName and Price columns
                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();

                    const string fallbackSql = @"
                        SELECT pd.PrescriptionDetailId, pd.PrescriptionId, pd.MedicineId, pd.Dosage, pd.Frequency,
                               pd.Duration, pd.Instructions, pd.Quantity, m.MedicineName, m.Price
                        FROM TblPrescriptionDetails pd
                        LEFT JOIN TblMedicines m ON pd.MedicineId = m.MedicineId
                        WHERE pd.PrescriptionId = @PrescriptionId";

                    using var command = new SqlCommand(fallbackSql, connection);
                    command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);

                    using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        prescriptionDetails.Add(new PrescriptionDetail
                        {
                            PrescriptionDetailId = reader.GetInt32(reader.GetOrdinal("PrescriptionDetailId")),
                            PrescriptionId = reader.GetInt32(reader.GetOrdinal("PrescriptionId")),
                            MedicineId = reader.GetInt32(reader.GetOrdinal("MedicineId")),
                            Dosage = reader.IsDBNull(reader.GetOrdinal("Dosage")) ? null : reader.GetString(reader.GetOrdinal("Dosage")),
                            Frequency = reader.IsDBNull(reader.GetOrdinal("Frequency")) ? null : reader.GetString(reader.GetOrdinal("Frequency")),
                            Duration = reader.IsDBNull(reader.GetOrdinal("Duration")) ? null : reader.GetString(reader.GetOrdinal("Duration")),
                            Instructions = reader.IsDBNull(reader.GetOrdinal("Instructions")) ? null : reader.GetString(reader.GetOrdinal("Instructions")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            MedicineName = reader.IsDBNull(reader.GetOrdinal("MedicineName")) ? $"Medicine #{reader.GetInt32(reader.GetOrdinal("MedicineId"))}" : reader.GetString(reader.GetOrdinal("MedicineName")),
                            Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Price"))
                        });
                    }

                    return prescriptionDetails;
                }
                catch
                {
                    throw new Exception($"Error getting prescription details: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting prescription details: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdatePrescriptionDetailAsync(PrescriptionDetail prescriptionDetail)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    UPDATE TblPrescriptionDetails
                    SET Dosage = @Dosage, Frequency = @Frequency, Duration = @Duration,
                        Instructions = @Instructions, Quantity = @Quantity, MedicineName = @MedicineName, Price = @Price
                    WHERE PrescriptionDetailId = @PrescriptionDetailId";

                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@PrescriptionDetailId", prescriptionDetail.PrescriptionDetailId);
                command.Parameters.AddWithValue("@Dosage", prescriptionDetail.Dosage ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Frequency", prescriptionDetail.Frequency ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Duration", prescriptionDetail.Duration ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Instructions", prescriptionDetail.Instructions ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Quantity", prescriptionDetail.Quantity);
                command.Parameters.AddWithValue("@MedicineName", prescriptionDetail.MedicineName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Price", prescriptionDetail.Price);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (SqlException ex) when (ex.Message.Contains("Invalid column name"))
            {
                // Fallback to update without MedicineName and Price columns
                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();

                    const string fallbackSql = @"
                        UPDATE TblPrescriptionDetails
                        SET Dosage = @Dosage, Frequency = @Frequency, Duration = @Duration,
                            Instructions = @Instructions, Quantity = @Quantity
                        WHERE PrescriptionDetailId = @PrescriptionDetailId";

                    using var command = new SqlCommand(fallbackSql, connection);

                    command.Parameters.AddWithValue("@PrescriptionDetailId", prescriptionDetail.PrescriptionDetailId);
                    command.Parameters.AddWithValue("@Dosage", prescriptionDetail.Dosage ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Frequency", prescriptionDetail.Frequency ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Duration", prescriptionDetail.Duration ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Instructions", prescriptionDetail.Instructions ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Quantity", prescriptionDetail.Quantity);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
                catch
                {
                    throw new Exception($"Error updating prescription detail: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating prescription detail: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeletePrescriptionDetailAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = "DELETE FROM TblPrescriptionDetails WHERE PrescriptionDetailId = @PrescriptionDetailId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PrescriptionDetailId", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting prescription detail: {ex.Message}", ex);
            }
        }
    }
}
