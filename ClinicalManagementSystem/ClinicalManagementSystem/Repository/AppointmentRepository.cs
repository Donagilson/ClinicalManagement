using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ClinicalManagementSystem2025.Services
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly string _connectionString;

        public AppointmentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MVCConnectionString")
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string is missing");
        }

        public async Task<int> AddAppointmentAsync(Appointment appointment)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    INSERT INTO TblAppointments (
                        PatientId, DoctorId, AppointmentDate, AppointmentTime, 
                        Status, Reason, Notes, CreatedBy, CreatedDate
                    ) 
                    OUTPUT INSERTED.AppointmentId
                    VALUES (
                        @PatientId, @DoctorId, @AppointmentDate, @AppointmentTime,
                        @Status, @Reason, @Notes, @CreatedBy, @CreatedDate
                    )";

                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@PatientId", appointment.PatientId);
                command.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
                command.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);
                command.Parameters.AddWithValue("@AppointmentTime", appointment.AppointmentTime);
                command.Parameters.AddWithValue("@Status", appointment.Status);
                command.Parameters.AddWithValue("@Reason", appointment.Reason ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(appointment.Notes) ? DBNull.Value : appointment.Notes);
                command.Parameters.AddWithValue("@CreatedBy", appointment.CreatedBy);
                command.Parameters.AddWithValue("@CreatedDate", appointment.CreatedDate);

                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding appointment: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync()
        {
            var appointments = new List<Appointment>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT 
                        a.AppointmentId, a.PatientId, a.DoctorId, a.AppointmentDate, a.AppointmentTime, 
                        a.Status, a.Reason, a.Notes, a.CreatedBy, a.CreatedDate,
                        p.FirstName, p.LastName, p.Phone AS PatientPhone,
                        u.FullName AS DoctorName, d.Specialization
                    FROM TblAppointments a
                    INNER JOIN TblPatients p ON a.PatientId = p.PatientId
                    INNER JOIN TblDoctors d ON a.DoctorId = d.DoctorId
                    INNER JOIN TblUsers u ON d.UserId = u.UserId
                    WHERE a.AppointmentDate = CAST(GETDATE() AS DATE)
                    ORDER BY a.AppointmentTime";

                using var command = new SqlCommand(sql, connection);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    appointments.Add(CreateAppointmentFromReader(reader));
                }

                return appointments;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting today's appointments: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            var appointments = new List<Appointment>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT 
                        a.AppointmentId, a.PatientId, a.DoctorId, a.AppointmentDate, a.AppointmentTime, 
                        a.Status, a.Reason, a.Notes, a.CreatedBy, a.CreatedDate,
                        p.FirstName, p.LastName, p.Phone AS PatientPhone,
                        u.FullName AS DoctorName, d.Specialization
                    FROM TblAppointments a
                    INNER JOIN TblPatients p ON a.PatientId = p.PatientId
                    INNER JOIN TblDoctors d ON a.DoctorId = d.DoctorId
                    INNER JOIN TblUsers u ON d.UserId = u.UserId
                    WHERE a.PatientId = @PatientId
                    ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    appointments.Add(CreateAppointmentFromReader(reader));
                }

                return appointments;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting appointments by patient ID: {ex.Message}", ex);
            }
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT 
                        a.AppointmentId, a.PatientId, a.DoctorId, a.AppointmentDate, a.AppointmentTime, 
                        a.Status, a.Reason, a.Notes, a.CreatedBy, a.CreatedDate,
                        p.FirstName, p.LastName, p.Phone AS PatientPhone,
                        u.FullName AS DoctorName, d.Specialization
                    FROM TblAppointments a
                    INNER JOIN TblPatients p ON a.PatientId = p.PatientId
                    INNER JOIN TblDoctors d ON a.DoctorId = d.DoctorId
                    INNER JOIN TblUsers u ON d.UserId = u.UserId
                    WHERE a.AppointmentId = @AppointmentId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@AppointmentId", appointmentId);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return CreateAppointmentFromReader(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting appointment by ID: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    UPDATE TblAppointments 
                    SET Status = @Status 
                    WHERE AppointmentId = @AppointmentId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@AppointmentId", appointmentId);
                command.Parameters.AddWithValue("@Status", status);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating appointment status: {ex.Message}", ex);
            }
        }

        public async Task<bool> CreateQuickAppointmentForNewPatientAsync(int patientId, int createdByUserId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    -- Get the first available doctor
                    DECLARE @DefaultDoctorId INT;
                    SELECT TOP 1 @DefaultDoctorId = DoctorId 
                    FROM TblDoctors d 
                    WHERE IsActive = 1 
                    ORDER BY DoctorId;

                    -- If no doctor exists, use default doctor ID 1
                    IF @DefaultDoctorId IS NULL
                        SET @DefaultDoctorId = 1;

                    -- Create appointment for today
                    INSERT INTO TblAppointments (
                        PatientId, DoctorId, AppointmentDate, AppointmentTime, 
                        Status, Reason, CreatedBy, CreatedDate
                    )
                    VALUES (
                        @PatientId, @DefaultDoctorId, CAST(GETDATE() AS DATE), 
                        DATEADD(HOUR, 1, CAST(GETDATE() AS TIME)),
                        'Scheduled', 'New Patient Registration', @CreatedBy, GETDATE()
                    );

                    SELECT 1 AS Success;";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);
                command.Parameters.AddWithValue("@CreatedBy", createdByUserId);

                var result = await command.ExecuteScalarAsync();
                return result != null && Convert.ToInt32(result) == 1;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating quick appointment for new patient: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            var appointments = new List<Appointment>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT 
                        a.AppointmentId, a.PatientId, a.DoctorId, a.AppointmentDate, a.AppointmentTime, 
                        a.Status, a.Reason, a.Notes, a.CreatedBy, a.CreatedDate,
                        p.FirstName, p.LastName, p.Phone AS PatientPhone,
                        u.FullName AS DoctorName, d.Specialization
                    FROM TblAppointments a
                    INNER JOIN TblPatients p ON a.PatientId = p.PatientId
                    INNER JOIN TblDoctors d ON a.DoctorId = d.DoctorId
                    INNER JOIN TblUsers u ON d.UserId = u.UserId
                    ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC";

                using var command = new SqlCommand(sql, connection);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    appointments.Add(CreateAppointmentFromReader(reader));
                }

                return appointments;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all appointments: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
        {
            var appointments = new List<Appointment>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT 
                        a.AppointmentId, a.PatientId, a.DoctorId, a.AppointmentDate, a.AppointmentTime, 
                        a.Status, a.Reason, a.Notes, a.CreatedBy, a.CreatedDate,
                        p.FirstName, p.LastName, p.Phone AS PatientPhone,
                        u.FullName AS DoctorName, d.Specialization
                    FROM TblAppointments a
                    INNER JOIN TblPatients p ON a.PatientId = p.PatientId
                    INNER JOIN TblDoctors d ON a.DoctorId = d.DoctorId
                    INNER JOIN TblUsers u ON d.UserId = u.UserId
                    WHERE a.DoctorId = @DoctorId
                    ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    appointments.Add(CreateAppointmentFromReader(reader));
                }

                return appointments;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting appointments by doctor ID: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsForDateAsync(int doctorId, DateTime date)
        {
            var appointments = new List<Appointment>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT 
                        a.AppointmentId, a.PatientId, a.DoctorId, a.AppointmentDate, a.AppointmentTime, 
                        a.Status, a.Reason, a.Notes, a.CreatedBy, a.CreatedDate,
                        p.FirstName, p.LastName, p.Phone AS PatientPhone,
                        u.FullName AS DoctorName, d.Specialization
                    FROM TblAppointments a
                    INNER JOIN TblPatients p ON a.PatientId = p.PatientId
                    INNER JOIN TblDoctors d ON a.DoctorId = d.DoctorId
                    INNER JOIN TblUsers u ON d.UserId = u.UserId
                    WHERE a.DoctorId = @DoctorId AND a.AppointmentDate = @AppointmentDate
                    ORDER BY a.AppointmentTime";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);
                command.Parameters.AddWithValue("@AppointmentDate", date.Date);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    appointments.Add(CreateAppointmentFromReader(reader));
                }

                return appointments;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting doctor appointments for date: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    UPDATE TblAppointments 
                    SET PatientId = @PatientId,
                        DoctorId = @DoctorId,
                        AppointmentDate = @AppointmentDate,
                        AppointmentTime = @AppointmentTime,
                        Status = @Status,
                        Reason = @Reason,
                        Notes = @Notes
                    WHERE AppointmentId = @AppointmentId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@AppointmentId", appointment.AppointmentId);
                command.Parameters.AddWithValue("@PatientId", appointment.PatientId);
                command.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
                command.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);
                command.Parameters.AddWithValue("@AppointmentTime", appointment.AppointmentTime);
                command.Parameters.AddWithValue("@Status", appointment.Status);
                command.Parameters.AddWithValue("@Reason", appointment.Reason ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(appointment.Notes) ? DBNull.Value : appointment.Notes);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating appointment: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = "DELETE FROM TblAppointments WHERE AppointmentId = @AppointmentId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@AppointmentId", appointmentId);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting appointment: {ex.Message}", ex);
            }
        }

        // Helper method to create Appointment object from SqlDataReader
        private Appointment CreateAppointmentFromReader(SqlDataReader reader)
        {
            return new Appointment
            {
                AppointmentId = GetSafeInt32(reader, "AppointmentId"),
                PatientId = GetSafeInt32(reader, "PatientId"),
                DoctorId = GetSafeInt32(reader, "DoctorId"),
                AppointmentDate = GetSafeDateTime(reader, "AppointmentDate"),
                AppointmentTime = GetSafeTimeSpan(reader, "AppointmentTime"),
                Status = GetSafeString(reader, "Status") ?? "Scheduled",
                Reason = GetSafeString(reader, "Reason") ?? "",
                Notes = GetSafeString(reader, "Notes"),
                CreatedBy = GetSafeInt32(reader, "CreatedBy"),
                CreatedDate = GetSafeDateTime(reader, "CreatedDate"),
                DoctorName = GetSafeString(reader, "DoctorName") ?? "",
                Specialization = GetSafeString(reader, "Specialization") ?? "",
                Patient = new Patient
                {
                    FirstName = GetSafeString(reader, "FirstName") ?? "",
                            LastName = GetSafeString(reader, "LastName") ?? "",
                    Phone = GetSafeString(reader, "PatientPhone") ?? ""
                }
            };
        }

        // Helper method to safely read string values that might be DBNull
        private string? GetSafeString(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
            }
            catch
            {
                return null;
            }
        }

        // Helper method to safely read int values that might be DBNull or strings
        private int GetSafeInt32(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal))
                    return 0;

                var value = reader[columnName];
                if (value is int intValue)
                    return intValue;

                if (value is string stringValue && int.TryParse(stringValue, out int parsedValue))
                    return parsedValue;

                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        // Helper method to safely read DateTime values
        private DateTime GetSafeDateTime(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        // Helper method to safely read TimeSpan values
        private TimeSpan GetSafeTimeSpan(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal))
                    return TimeSpan.Zero;

                var value = reader[columnName];
                if (value is TimeSpan timeSpanValue)
                    return timeSpanValue;

                if (value is DateTime dateTimeValue)
                    return dateTimeValue.TimeOfDay;

                if (value is string stringValue && TimeSpan.TryParse(stringValue, out TimeSpan parsedTimeSpan))
                    return parsedTimeSpan;

                return TimeSpan.Zero;
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }
    }
}