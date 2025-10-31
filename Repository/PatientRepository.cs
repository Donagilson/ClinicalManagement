using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicalManagementSystem2025.Services
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string _connectionString;

        public PatientRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MVCConnectionString")
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string is missing");
        }

      public async Task<bool> UpdatePatientAsync(Patient patient)
      {
          try
          {
              using var connection = new SqlConnection(_connectionString);
              await connection.OpenAsync();

              const string sql = @"
                  UPDATE TblPatients
                  SET FirstName = @FirstName,
                      LastName = @LastName,
                      Gender = @Gender,
                      DateOfBirth = @DateOfBirth,
                      Phone = @Phone,
                      Email = @Email,
                      Address = @Address,
                      BloodGroup = @BloodGroup,
                      EmergencyContact = @EmergencyContact,
                      IsActive = @IsActive
                  WHERE PatientId = @PatientId";

              using var command = new SqlCommand(sql, connection);

              command.Parameters.AddWithValue("@PatientId", patient.PatientId);
              command.Parameters.AddWithValue("@FirstName", patient.FirstName);
              command.Parameters.AddWithValue("@LastName", patient.LastName);
              command.Parameters.AddWithValue("@Gender", patient.Gender);
              command.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);
              command.Parameters.AddWithValue("@Phone", patient.Phone);
              command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(patient.Email) ? DBNull.Value : patient.Email);
              command.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(patient.Address) ? DBNull.Value : patient.Address);
              command.Parameters.AddWithValue("@BloodGroup", string.IsNullOrEmpty(patient.BloodGroup) ? DBNull.Value : patient.BloodGroup);
              command.Parameters.AddWithValue("@EmergencyContact", string.IsNullOrEmpty(patient.EmergencyContact) ? DBNull.Value : patient.EmergencyContact);
              command.Parameters.AddWithValue("@IsActive", patient.IsActive);

              var rows = await command.ExecuteNonQueryAsync();
              return rows > 0;
          }
          catch (Exception ex)
          {
              throw new Exception($"Error updating patient: {ex.Message}", ex);
          }
      }

        public async Task<int> AddPatientAsync(Patient patient)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    INSERT INTO TblPatients (
                        FirstName, LastName, Gender, DateOfBirth, Phone,
                        Email, Address, BloodGroup, EmergencyContact,
                        RegistrationDate, IsActive
                    )
                    OUTPUT INSERTED.PatientId
                    VALUES (
                        @FirstName, @LastName, @Gender, @DateOfBirth, @Phone,
                        @Email, @Address, @BloodGroup, @EmergencyContact,
                        @RegistrationDate, @IsActive
                    )";

                using var command = new SqlCommand(sql, connection);

                // Add parameters - matching TblPatients columns
                command.Parameters.AddWithValue("@FirstName", patient.FirstName);
                command.Parameters.AddWithValue("@LastName", patient.LastName);
                command.Parameters.AddWithValue("@Gender", patient.Gender);
                command.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);
                command.Parameters.AddWithValue("@Phone", patient.Phone);
                command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(patient.Email) ? DBNull.Value : patient.Email);
                command.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(patient.Address) ? DBNull.Value : patient.Address);
                command.Parameters.AddWithValue("@BloodGroup", string.IsNullOrEmpty(patient.BloodGroup) ? DBNull.Value : patient.BloodGroup);
                command.Parameters.AddWithValue("@EmergencyContact", string.IsNullOrEmpty(patient.EmergencyContact) ? DBNull.Value : patient.EmergencyContact);
                command.Parameters.AddWithValue("@RegistrationDate", patient.RegistrationDate);
                command.Parameters.AddWithValue("@IsActive", patient.IsActive);

                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (SqlException ex)
            {
                // Handle specific SQL errors
                if (ex.Number == 2627) // Unique constraint violation
                {
                    throw new Exception("A patient with this phone number already exists.");
                }
                throw new Exception($"Database error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding patient: {ex.Message}", ex);
            }
        }

        public async Task<int> AddPatientWithAppointmentAsync(Patient patient, int createdByUserId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // 1. Add Patient
                const string patientSql = @"
                    INSERT INTO TblPatients (
                        FirstName, LastName, Gender, DateOfBirth, Phone,
                        Email, Address, BloodGroup, EmergencyContact,
                        RegistrationDate, IsActive
                    )
                    OUTPUT INSERTED.PatientId
                    VALUES (
                        @FirstName, @LastName, @Gender, @DateOfBirth, @Phone,
                        @Email, @Address, @BloodGroup, @EmergencyContact,
                        @RegistrationDate, @IsActive
                    )";

                using var patientCommand = new SqlCommand(patientSql, connection, transaction);
                patientCommand.Parameters.AddWithValue("@FirstName", patient.FirstName);
                patientCommand.Parameters.AddWithValue("@LastName", patient.LastName);
                patientCommand.Parameters.AddWithValue("@Gender", patient.Gender);
                patientCommand.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);
                patientCommand.Parameters.AddWithValue("@Phone", patient.Phone);
                patientCommand.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(patient.Email) ? DBNull.Value : patient.Email);
                patientCommand.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(patient.Address) ? DBNull.Value : patient.Address);
                patientCommand.Parameters.AddWithValue("@BloodGroup", string.IsNullOrEmpty(patient.BloodGroup) ? DBNull.Value : patient.BloodGroup);
                patientCommand.Parameters.AddWithValue("@EmergencyContact", string.IsNullOrEmpty(patient.EmergencyContact) ? DBNull.Value : patient.EmergencyContact);
                patientCommand.Parameters.AddWithValue("@RegistrationDate", patient.RegistrationDate);
                patientCommand.Parameters.AddWithValue("@IsActive", patient.IsActive);

                var patientId = Convert.ToInt32(await patientCommand.ExecuteScalarAsync());

                if (patientId > 0)
                {
                    // 2. Create Automatic Appointment for Today
                    const string appointmentSql = @"
                        -- Get the first available doctor
                        DECLARE @DefaultDoctorId INT;
                        SELECT TOP 1 @DefaultDoctorId = DoctorId
                        FROM TblDoctors d
                        WHERE IsActive = 1
                        ORDER BY DoctorId;

                        -- If no doctor exists, use a default one
                        IF @DefaultDoctorId IS NULL
                            SET @DefaultDoctorId = 1;

                        -- Create appointment for today (1 hour from current time)
                        INSERT INTO TblAppointments (
                            PatientId, DoctorId, AppointmentDate, AppointmentTime,
                            Status, Reason, CreatedBy, CreatedDate
                        )
                        VALUES (
                            @PatientId, @DefaultDoctorId, CAST(GETDATE() AS DATE),
                            DATEADD(HOUR, 1, CAST(GETDATE() AS TIME)),
                            'Scheduled', 'New Patient Registration', @CreatedBy, GETDATE()
                        );";

                    using var appointmentCommand = new SqlCommand(appointmentSql, connection, transaction);
                    appointmentCommand.Parameters.AddWithValue("@PatientId", patientId);
                    appointmentCommand.Parameters.AddWithValue("@CreatedBy", createdByUserId);

                    await appointmentCommand.ExecuteNonQueryAsync();

                    transaction.Commit();
                    return patientId;
                }

                transaction.Rollback();
                return 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Error adding patient with appointment: {ex.Message}", ex);
            }
        }

        public async Task<Patient?> GetPatientByIdAsync(int patientId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = "SELECT * FROM TblPatients WHERE PatientId = @PatientId";
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Patient
                    {
                        PatientId = reader.GetInt32("PatientId"),
                        FirstName = reader["FirstName"] as string ?? "",
                        LastName = reader["LastName"] as string ?? "",
                        Gender = reader["Gender"] as string ?? "",
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Phone = reader["Phone"] as string ?? "",
                        Email = reader["Email"] as string,
                        Address = reader["Address"] as string,
                        BloodGroup = reader["BloodGroup"] as string,
                        EmergencyContact = reader["EmergencyContact"] as string,
                        RegistrationDate = reader.GetDateTime("RegistrationDate"),
                        IsActive = reader.GetBoolean("IsActive")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting patient by ID: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Patient>> GetPatientsByPhoneAsync(string phone)
        {
            var patients = new List<Patient>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = "SELECT * FROM TblPatients WHERE Phone = @Phone AND IsActive = 1";
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Phone", phone);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    patients.Add(new Patient
                    {
                        PatientId = reader.GetInt32("PatientId"),
                        FirstName = reader["FirstName"] as string ?? "",
                        LastName = reader["LastName"] as string ?? "",
                        Gender = reader["Gender"] as string ?? "",
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Phone = reader["Phone"] as string ?? "",
                        Email = reader["Email"] as string,
                        Address = reader["Address"] as string,
                        BloodGroup = reader["BloodGroup"] as string,
                        EmergencyContact = reader["EmergencyContact"] as string,
                        RegistrationDate = reader.GetDateTime("RegistrationDate"),
                        IsActive = reader.GetBoolean("IsActive")
                    });
                }

                return patients;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting patients by phone: {ex.Message}", ex);
            }
        }

        public async Task<Patient?> SearchPatientByIdAsync(int patientId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT * FROM TblPatients
                    WHERE PatientId = @PatientId
                    AND IsActive = 1";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Patient
                    {
                        PatientId = reader.GetInt32("PatientId"),
                        FirstName = reader["FirstName"] as string ?? "",
                        LastName = reader["LastName"] as string ?? "",
                        Gender = reader["Gender"] as string ?? "",
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Phone = reader["Phone"] as string ?? "",
                        Email = reader["Email"] as string,
                        Address = reader["Address"] as string,
                        BloodGroup = reader["BloodGroup"] as string,
                        EmergencyContact = reader["EmergencyContact"] as string,
                        RegistrationDate = reader.GetDateTime("RegistrationDate"),
                        IsActive = reader.GetBoolean("IsActive")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching patient by ID: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
        {
            var patients = new List<Patient>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT * FROM TblPatients
                    WHERE IsActive = 1
                    AND (
                        PatientId = @SearchTermInt
                        OR Phone LIKE @SearchTerm
                        OR FirstName LIKE @SearchTerm
                        OR LastName LIKE @SearchTerm
                        OR Email LIKE @SearchTerm
                        OR CONCAT(FirstName, ' ', LastName) LIKE @SearchTerm
                    )
                    ORDER BY FirstName, LastName";

                using var command = new SqlCommand(sql, connection);

                // Try to parse as integer for PatientId search
                if (int.TryParse(searchTerm, out int patientId))
                {
                    command.Parameters.AddWithValue("@SearchTermInt", patientId);
                }
                else
                {
                    command.Parameters.AddWithValue("@SearchTermInt", DBNull.Value);
                }

                command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    patients.Add(new Patient
                    {
                        PatientId = reader.GetInt32("PatientId"),
                        FirstName = reader["FirstName"] as string ?? "",
                        LastName = reader["LastName"] as string ?? "",
                        Gender = reader["Gender"] as string ?? "",
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Phone = reader["Phone"] as string ?? "",
                        Email = reader["Email"] as string,
                        Address = reader["Address"] as string,
                        BloodGroup = reader["BloodGroup"] as string,
                        EmergencyContact = reader["EmergencyContact"] as string,
                        RegistrationDate = reader.GetDateTime("RegistrationDate"),
                        IsActive = reader.GetBoolean("IsActive")
                    });
                }

                return patients;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching patients: {ex.Message}", ex);
            }
        }

        // ========== ADDITIONAL METHODS FOR SERVICE LAYER COMPATIBILITY ==========

        public Task<int> AddAsync(Patient patient)
        {
            return AddPatientAsync(patient);
        }

        public Task<Patient?> GetByIdAsync(int id)
        {
            return GetPatientByIdAsync(id);
        }

        public async Task<Patient?> GetByPhoneAsync(string phone)
        {
            var patients = await GetPatientsByPhoneAsync(phone);
            return patients.FirstOrDefault();
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            var patients = new List<Patient>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = "SELECT * FROM TblPatients WHERE IsActive = 1 ORDER BY FirstName, LastName";
                using var command = new SqlCommand(sql, connection);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    patients.Add(new Patient
                    {
                        PatientId = reader.GetInt32("PatientId"),
                        FirstName = reader["FirstName"] as string ?? "",
                        LastName = reader["LastName"] as string ?? "",
                        Gender = reader["Gender"] as string ?? "",
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Phone = reader["Phone"] as string ?? "",
                        Email = reader["Email"] as string,
                        Address = reader["Address"] as string,
                        BloodGroup = reader["BloodGroup"] as string,
                        EmergencyContact = reader["EmergencyContact"] as string,
                        RegistrationDate = reader.GetDateTime("RegistrationDate"),
                        IsActive = reader.GetBoolean("IsActive")
                    });
                }

                return patients;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all patients: {ex.Message}", ex);
            }
        }
    }
}
