using System.Data;
using System.Data.SqlClient;
using ClinicalManagementSystem2025.Models;
using Microsoft.Extensions.Configuration;

namespace ClinicalManagementSystem2025.Repository
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly string _connectionString;

        public DoctorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ClinicalConnectionString") ??
                              configuration.GetConnectionString("DefaultConnection") ??
                              configuration.GetConnectionString("MVCConnectionString") ??
                              throw new ArgumentNullException("Connection string not found");
        }

        public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync(int? departmentId = null)
        {
            var doctors = new List<Doctor>();
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                SELECT d.DoctorId, u.FullName as DoctorName,
                       d.Specialization, d.Qualification, dep.DepartmentName,
                       d.ConsultationFee, d.AvailableFrom, d.AvailableTo
                FROM TblDoctors d
                INNER JOIN TblUsers u ON d.UserId = u.UserId
                INNER JOIN TblDepartments dep ON d.DepartmentId = dep.DepartmentId
                WHERE d.IsActive = 1 AND u.IsActive = 1";

            if (departmentId.HasValue)
            {
                sql += " AND d.DepartmentId = @DepartmentId";
            }

            sql += " ORDER BY d.Specialization, DoctorName";

            using var command = new SqlCommand(sql, connection);
            if (departmentId.HasValue)
            {
                command.Parameters.AddWithValue("@DepartmentId", departmentId.Value);
            }

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                doctors.Add(new Doctor
                {
                    DoctorId = reader.GetInt32("DoctorId"),
                    DoctorName = reader.GetString("DoctorName"),
                    Specialization = reader.GetString("Specialization"),
                    Qualification = reader.IsDBNull(reader.GetOrdinal("Qualification")) ? null : reader.GetString(reader.GetOrdinal("Qualification")),
                    // DepartmentName = reader.GetString("DepartmentName"),
                    ConsultationFee = reader.GetDecimal("ConsultationFee"),
                    //AvailableFrom = reader.IsDBNull(reader.GetOrdinal("AvailableFrom")) ? null : reader.GetTimeSpan("AvailableFrom"),
                    //AvailableTo = reader.IsDBNull(reader.GetOrdinal("AvailableTo")) ? null : reader.GetTimeSpan("AvailableTo")
                });
            }

            return doctors;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            var departments = new List<Department>();
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT DepartmentId, DepartmentName, Description, IsActive
                FROM TblDepartments
                WHERE IsActive = 1
                ORDER BY DepartmentName";

            using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                departments.Add(new Department
                {
                    DepartmentId = reader.GetInt32("DepartmentId"),
                    DepartmentName = reader.GetString("DepartmentName"),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }

            return departments;
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int doctorId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT d.DoctorId, u.FullName as DoctorName,
                       d.Specialization, d.Qualification, dep.DepartmentName,
                       d.ConsultationFee, d.AvailableFrom, d.AvailableTo
                FROM TblDoctors d
                INNER JOIN TblUsers u ON d.UserId = u.UserId
                INNER JOIN TblDepartments dep ON d.DepartmentId = dep.DepartmentId
                WHERE d.DoctorId = @DoctorId AND d.IsActive = 1";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@DoctorId", doctorId);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Doctor
                {
                    DoctorId = reader.GetInt32("DoctorId"),
                    DoctorName = reader.GetString("DoctorName"),
                    Specialization = reader.GetString("Specialization"),
                    Qualification = reader.IsDBNull(reader.GetOrdinal("Qualification")) ? null : reader.GetString(reader.GetOrdinal("Qualification")),
                    // DepartmentName = reader.GetString("DepartmentName"),
                    ConsultationFee = reader.GetDecimal("ConsultationFee"),
                    // AvailableFrom = reader.IsDBNull(reader.GetOrdinal("AvailableFrom")) ? null : reader.GetTimeSpan("AvailableFrom"),
                    //AvailableTo = reader.IsDBNull(reader.GetOrdinal("AvailableTo")) ? null : reader.GetTimeSpan("AvailableTo")
                };
            }

            return null;
        }


        public async Task<List<DoctorDto>?> GetDoctorsByDepartmentAsync(int departmentId)
        {
            var doctors = new List<DoctorDto>();
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT d.DoctorId, u.FullName as DoctorName,
                       d.Specialization, d.ConsultationFee, d.AvailableFrom, d.AvailableTo
                FROM TblDoctors d
                INNER JOIN TblUsers u ON d.UserId = u.UserId
                WHERE d.DepartmentId = @DepartmentId AND d.IsActive = 1 AND u.IsActive = 1";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@DepartmentId", departmentId);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                doctors.Add(new DoctorDto
                {
                    DoctorId = reader.GetInt32("DoctorId"),
                    DoctorName = reader.GetString("DoctorName"),
                    Specialization = reader.GetString("Specialization"),
                    ConsultationFee = reader.GetDecimal("ConsultationFee"),
                    //AvailableFrom = reader.GetTimeSpan("AvailableFrom"),
                    //AvailableTo = reader.GetTimeSpan("AvailableTo")
                });
            }

            return doctors;
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = new List<DoctorDto>();
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT d.DoctorId, u.FullName as DoctorName,
                       d.Specialization, d.ConsultationFee, d.AvailableFrom, d.AvailableTo
                FROM TblDoctors d
                INNER JOIN TblUsers u ON d.UserId = u.UserId
                WHERE d.IsActive = 1 AND u.IsActive = 1";

            using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                doctors.Add(new DoctorDto
                {
                    DoctorId = reader.GetInt32("DoctorId"),
                    DoctorName = reader.GetString("DoctorName"),
                    Specialization = reader.GetString("Specialization"),
                    ConsultationFee = reader.GetDecimal("ConsultationFee"),
                    // AvailableFrom = reader.GetTimeSpan("AvailableFrom"),
                    //AvailableTo = reader.GetTimeSpan("AvailableTo")
                });
            }

            return doctors;
        }

        public async Task<int> AddDoctorAsync(Doctor doctor)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                INSERT INTO TblDoctors (UserId, Specialization, Qualification, Experience,
                                      DepartmentId, ConsultationFee, AvailableFrom, AvailableTo, IsActive)
                OUTPUT INSERTED.DoctorId
                VALUES (@UserId, @Specialization, @Qualification, @Experience,
                        @DepartmentId, @ConsultationFee, @AvailableFrom, @AvailableTo, @IsActive)";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", doctor.UserId);
            command.Parameters.AddWithValue("@Specialization", doctor.Specialization ?? string.Empty);
            command.Parameters.AddWithValue("@Qualification", doctor.Qualification ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Experience", doctor.Experience ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DepartmentId", doctor.DepartmentId);
            command.Parameters.AddWithValue("@ConsultationFee", doctor.ConsultationFee ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@AvailableFrom", doctor.AvailableFrom ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@AvailableTo", doctor.AvailableTo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", doctor.IsActive);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                UPDATE TblDoctors
                SET UserId = @UserId, Specialization = @Specialization, SpecializationId = @SpecializationId,
                    Qualification = @Qualification, Experience = @Experience, DepartmentId = @DepartmentId,
                    ConsultationFee = @ConsultationFee, AvailableFrom = @AvailableFrom, AvailableTo = @AvailableTo,
                    IsActive = @IsActive
                WHERE DoctorId = @DoctorId";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@DoctorId", doctor.DoctorId);
            command.Parameters.AddWithValue("@UserId", doctor.UserId);
            command.Parameters.AddWithValue("@Specialization", doctor.Specialization ?? string.Empty);
            command.Parameters.AddWithValue("@SpecializationId", doctor.SpecializationId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Qualification", doctor.Qualification ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Experience", doctor.Experience ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DepartmentId", doctor.DepartmentId);
            command.Parameters.AddWithValue("@ConsultationFee", doctor.ConsultationFee ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@AvailableFrom", doctor.AvailableFrom ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@AvailableTo", doctor.AvailableTo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", doctor.IsActive);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteDoctorAsync(int doctorId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "UPDATE TblDoctors SET IsActive = 0 WHERE DoctorId = @DoctorId";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@DoctorId", doctorId);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsWithDetailsAsync()
        {
            var doctors = new List<Doctor>();
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT d.DoctorId, d.UserId, d.Specialization, d.Qualification,
                       d.Experience, d.DepartmentId, d.ConsultationFee, d.AvailableFrom, d.AvailableTo, d.IsActive,
                       u.FullName, u.Email, u.Phone,
                       dep.DepartmentName
                FROM TblDoctors d
                INNER JOIN TblUsers u ON d.UserId = u.UserId
                INNER JOIN TblDepartments dep ON d.DepartmentId = dep.DepartmentId
                ORDER BY u.FullName";

            using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var emailOrdinal = reader.GetOrdinal("Email");
                var phoneOrdinal = reader.GetOrdinal("Phone");
                // SpecializationId column not present in current DB schema
                var qualificationOrdinal = reader.GetOrdinal("Qualification");
                var experienceOrdinal = reader.GetOrdinal("Experience");
                var consultationFeeOrdinal = reader.GetOrdinal("ConsultationFee");
                var availableFromOrdinal = reader.GetOrdinal("AvailableFrom");
                var availableToOrdinal = reader.GetOrdinal("AvailableTo");

                doctors.Add(new Doctor
                {
                    DoctorId = reader.GetInt32("DoctorId"),
                    UserId = reader.GetInt32("UserId"),
                    Specialization = reader.GetString("Specialization"),
                    SpecializationId = null,
                    Qualification = reader.IsDBNull(qualificationOrdinal) ? null : reader.GetString(qualificationOrdinal),
                    Experience = reader.IsDBNull(experienceOrdinal) ? null : reader.GetInt32(experienceOrdinal),
                    DepartmentId = reader.GetInt32("DepartmentId"),
                    ConsultationFee = reader.IsDBNull(consultationFeeOrdinal) ? null : reader.GetDecimal(consultationFeeOrdinal),
                    AvailableFrom = reader.IsDBNull(availableFromOrdinal) ? null : reader.GetTimeSpan(availableFromOrdinal),
                    AvailableTo = reader.IsDBNull(availableToOrdinal) ? null : reader.GetTimeSpan(availableToOrdinal),
                    IsActive = reader.GetBoolean("IsActive"),
                    User = new User
                    {
                        UserId = reader.GetInt32("UserId"),
                        UserName = reader.GetString("FullName"), // Using FullName as UserName for display
                        FullName = reader.GetString("FullName"),
                        Email = reader.IsDBNull(emailOrdinal) ? null : reader.GetString(emailOrdinal),
                        Phone = reader.IsDBNull(phoneOrdinal) ? null : reader.GetString(phoneOrdinal)
                    },
                    Department = new Department
                    {
                        DepartmentId = reader.GetInt32("DepartmentId"),
                        DepartmentName = reader.GetString("DepartmentName")
                    }
                });
            }

            return doctors;
        }

        public async Task<IEnumerable<Specialization>> GetAllSpecializationsAsync()
        {
            var specializations = new List<Specialization>();
            using var connection = new SqlConnection(_connectionString);
            
            try
            {
                using var command = new SqlCommand("[dbo].[sp_GetAllSpecializations]", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    specializations.Add(new Specialization
                    {
                        SpecializationId = reader.GetInt32("SpecializationId"),
                        SpecializationName = reader.GetString("SpecializationName"),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        DepartmentId = reader.GetInt32("DepartmentId"),
                        IsActive = reader.GetBoolean("IsActive"),
                        CreatedDate = reader.GetDateTime("CreatedDate"),
                        Department = new Department
                        {
                            DepartmentId = reader.GetInt32("DepartmentId"),
                            DepartmentName = reader.GetString("DepartmentName")
                        }
                    });
                }
            }
            catch (SqlException ex) when (ex.Number == 208 || ex.Number == 2812)
            {
                await connection.CloseAsync();

                var sqlPlural = @"
                    SELECT s.SpecializationId, s.SpecializationName, s.Description, s.DepartmentId, s.IsActive,
                           ISNULL(s.CreatedDate, GETDATE()) AS CreatedDate,
                           d.DepartmentName
                    FROM TblSpecializations s
                    INNER JOIN TblDepartments d ON s.DepartmentId = d.DepartmentId
                    WHERE s.IsActive = 1
                    ORDER BY s.SpecializationName";

                try
                {
                    using var cmd = new SqlCommand(sqlPlural, connection);
                    await connection.OpenAsync();
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        specializations.Add(new Specialization
                        {
                            SpecializationId = reader.GetInt32("SpecializationId"),
                            SpecializationName = reader.GetString("SpecializationName"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            DepartmentId = reader.GetInt32("DepartmentId"),
                            IsActive = reader.GetBoolean("IsActive"),
                            CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            Department = new Department
                            {
                                DepartmentId = reader.GetInt32("DepartmentId"),
                                DepartmentName = reader.GetString("DepartmentName")
                            }
                        });
                    }
                }
                catch (SqlException)
                {
                    // If no specialization table exists, return empty list to prevent crash
                    // This allows the Add Doctor page to load without specializations
                }
            }

            return specializations;
        }

        public async Task<IEnumerable<Specialization>> GetSpecializationsByDepartmentAsync(int departmentId)
        {
            var specializations = new List<Specialization>();
            using var connection = new SqlConnection(_connectionString);
            
            try
            {
                using var command = new SqlCommand("[dbo].[sp_GetSpecializationsByDepartment]", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@DepartmentId", departmentId);
                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    specializations.Add(new Specialization
                    {
                        SpecializationId = reader.GetInt32("SpecializationId"),
                        SpecializationName = reader.GetString("SpecializationName"),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        DepartmentId = reader.GetInt32("DepartmentId"),
                        IsActive = reader.GetBoolean("IsActive"),
                        Department = new Department
                        {
                            DepartmentId = reader.GetInt32("DepartmentId"),
                            DepartmentName = reader.GetString("DepartmentName")
                        }
                    });
                }
            }
            catch (SqlException ex) when (ex.Number == 208 || ex.Number == 2812)
            {
                await connection.CloseAsync();

                var sqlPlural = @"
                    SELECT s.SpecializationId, s.SpecializationName, s.Description, s.DepartmentId, s.IsActive,
                           d.DepartmentName
                    FROM TblSpecializations s
                    INNER JOIN TblDepartments d ON s.DepartmentId = d.DepartmentId
                    WHERE s.IsActive = 1 AND s.DepartmentId = @DepartmentId
                    ORDER BY s.SpecializationName";

                try
                {
                    using var cmd = new SqlCommand(sqlPlural, connection);
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                    await connection.OpenAsync();
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        specializations.Add(new Specialization
                        {
                            SpecializationId = reader.GetInt32("SpecializationId"),
                            SpecializationName = reader.GetString("SpecializationName"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            DepartmentId = reader.GetInt32("DepartmentId"),
                            IsActive = reader.GetBoolean("IsActive"),
                            Department = new Department
                            {
                                DepartmentId = reader.GetInt32("DepartmentId"),
                                DepartmentName = reader.GetString("DepartmentName")
                            }
                        });
                    }
                }
                catch (SqlException)
                {
                    // If no specialization table exists, return empty list to prevent crash
                    // This allows the Add Doctor page to load without specializations
                }
            }

            return specializations;
        }
    }
}
