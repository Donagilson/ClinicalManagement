using ClinicalManagementSystem2025.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace ClinicalManagementSystem2025.Repository
{
    public class LabRepository : ILabRepository
    {
        private readonly string _connectionString;

        public LabRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MVCConnectionString")
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string is missing");
        }

        public async Task<LabTest?> GetLabTestByIdAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT LabTestId, TestCode, TestName, Description, Price, NormalRange, Unit, DepartmentId, IsActive, CreatedDate
                    FROM TblLabTests
                    WHERE LabTestId = @LabTestId AND IsActive = 1";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@LabTestId", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new LabTest
                    {
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        TestCode = reader.GetString(reader.GetOrdinal("TestCode")),
                        TestName = reader.GetString(reader.GetOrdinal("TestName")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Unit = reader.IsDBNull(reader.GetOrdinal("Unit")) ? null : reader.GetString(reader.GetOrdinal("Unit")),
                        DepartmentId = reader.IsDBNull(reader.GetOrdinal("DepartmentId")) ? null : reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting lab test: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<LabTest>> GetAllLabTestsAsync()
        {
            var labTests = new List<LabTest>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT LabTestId, TestCode, TestName, Description, Price, NormalRange, Unit, DepartmentId, IsActive, CreatedDate
                    FROM TblLabTests
                    WHERE IsActive = 1
                    ORDER BY TestName";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    labTests.Add(new LabTest
                    {
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        TestCode = reader.GetString(reader.GetOrdinal("TestCode")),
                        TestName = reader.GetString(reader.GetOrdinal("TestName")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Unit = reader.IsDBNull(reader.GetOrdinal("Unit")) ? null : reader.GetString(reader.GetOrdinal("Unit")),
                        DepartmentId = reader.IsDBNull(reader.GetOrdinal("DepartmentId")) ? null : reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                    });
                }

                return labTests;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all lab tests: {ex.Message}", ex);
            }
        }

        public async Task<LabTest> AddLabTestAsync(LabTest labTest)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    INSERT INTO TblLabTests (TestCode, TestName, Description, Price, NormalRange, Unit, DepartmentId, IsActive, CreatedDate)
                    OUTPUT INSERTED.LabTestId
                    VALUES (@TestCode, @TestName, @Description, @Price, @NormalRange, @Unit, @DepartmentId, @IsActive, @CreatedDate)";

                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@TestCode", labTest.TestCode);
                command.Parameters.AddWithValue("@TestName", labTest.TestName);
                command.Parameters.AddWithValue("@Description", labTest.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Price", labTest.Price);
                command.Parameters.AddWithValue("@NormalRange", labTest.NormalRange ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Unit", labTest.Unit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DepartmentId", labTest.DepartmentId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsActive", labTest.IsActive);
                command.Parameters.AddWithValue("@CreatedDate", labTest.CreatedDate);

                var result = await command.ExecuteScalarAsync();
                labTest.LabTestId = Convert.ToInt32(result);

                return labTest;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding lab test: {ex.Message}", ex);
            }
        }

        public async Task<LabTest> UpdateLabTestAsync(LabTest labTest)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    UPDATE TblLabTests
                    SET TestCode = @TestCode, TestName = @TestName, Description = @Description,
                        Price = @Price, NormalRange = @NormalRange, Unit = @Unit, DepartmentId = @DepartmentId
                    WHERE LabTestId = @LabTestId";

                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@LabTestId", labTest.LabTestId);
                command.Parameters.AddWithValue("@TestCode", labTest.TestCode);
                command.Parameters.AddWithValue("@TestName", labTest.TestName);
                command.Parameters.AddWithValue("@Description", labTest.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Price", labTest.Price);
                command.Parameters.AddWithValue("@NormalRange", labTest.NormalRange ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Unit", labTest.Unit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DepartmentId", labTest.DepartmentId ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();
                return labTest;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating lab test: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteLabTestAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = "UPDATE TblLabTests SET IsActive = 0 WHERE LabTestId = @LabTestId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@LabTestId", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting lab test: {ex.Message}", ex);
            }
        }

        public async Task<LabReport?> GetLabReportByIdAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT lr.LabReportId, lr.ReportCode, lr.PatientId, lr.LabTestId, lr.DoctorId, lr.LabTechnicianId,
                           lr.CollectionDate, lr.ReportDate, lr.ResultValue, lr.ResultUnit,
                           lr.Findings, lr.Status, lr.Notes, lr.CreatedDate,
                           p.FirstName + ' ' + p.LastName AS PatientName,
                           lt.TestName,
                           d.FullName AS DoctorName,
                           u.FullName AS LabTechnicianName
                    FROM TblLabReports lr
                    JOIN TblPatients p ON lr.PatientId = p.PatientId
                    JOIN TblLabTests lt ON lr.LabTestId = lt.LabTestId
                    JOIN TblUsers d ON lr.DoctorId = d.UserId
                    JOIN TblUsers u ON lr.LabTechnicianId = u.UserId
                    WHERE lr.LabReportId = @LabReportId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@LabReportId", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new LabReport
                    {
                        LabReportId = reader.GetInt32(reader.GetOrdinal("LabReportId")),
                        ReportCode = reader.GetString(reader.GetOrdinal("ReportCode")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        LabTechnicianId = reader.GetInt32(reader.GetOrdinal("LabTechnicianId")),
                        CollectionDate = reader.GetDateTime(reader.GetOrdinal("CollectionDate")),
                        ReportDate = reader.IsDBNull(reader.GetOrdinal("ReportDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ReportDate")),
                        ResultValue = reader.IsDBNull(reader.GetOrdinal("ResultValue")) ? null : reader.GetString(reader.GetOrdinal("ResultValue")),
                        ResultUnit = reader.IsDBNull(reader.GetOrdinal("ResultUnit")) ? null : reader.GetString(reader.GetOrdinal("ResultUnit")),
                        Findings = reader.IsDBNull(reader.GetOrdinal("Findings")) ? null : reader.GetString(reader.GetOrdinal("Findings")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        PatientName = reader.IsDBNull(reader.GetOrdinal("PatientName")) ? null : reader.GetString(reader.GetOrdinal("PatientName")),
                        TestName = reader.IsDBNull(reader.GetOrdinal("TestName")) ? null : reader.GetString(reader.GetOrdinal("TestName")),
                        DoctorName = reader.IsDBNull(reader.GetOrdinal("DoctorName")) ? null : reader.GetString(reader.GetOrdinal("DoctorName")),
                        LabTechnicianName = reader.IsDBNull(reader.GetOrdinal("LabTechnicianName")) ? null : reader.GetString(reader.GetOrdinal("LabTechnicianName"))
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting lab report: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<LabReport>> GetAllLabReportsAsync()
        {
            var labReports = new List<LabReport>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT LabReportId, ReportCode, PatientId, LabTestId, DoctorId, LabTechnicianId,
                           CollectionDate, ReportDate, ResultValue, ResultUnit,
                           Findings, Status, Notes, CreatedDate
                    FROM TblLabReports
                    ORDER BY CreatedDate DESC";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    labReports.Add(new LabReport
                    {
                        LabReportId = reader.GetInt32(reader.GetOrdinal("LabReportId")),
                        ReportCode = reader.GetString(reader.GetOrdinal("ReportCode")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        LabTechnicianId = reader.GetInt32(reader.GetOrdinal("LabTechnicianId")),
                        CollectionDate = reader.GetDateTime(reader.GetOrdinal("CollectionDate")),
                        ReportDate = reader.IsDBNull(reader.GetOrdinal("ReportDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ReportDate")),
                        ResultValue = reader.IsDBNull(reader.GetOrdinal("ResultValue")) ? null : reader.GetString(reader.GetOrdinal("ResultValue")),
                        ResultUnit = reader.IsDBNull(reader.GetOrdinal("ResultUnit")) ? null : reader.GetString(reader.GetOrdinal("ResultUnit")),
                        Findings = reader.IsDBNull(reader.GetOrdinal("Findings")) ? null : reader.GetString(reader.GetOrdinal("Findings")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                    });
                }

                return labReports;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all lab reports: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<LabReport>> GetLabReportsByPatientAsync(int patientId)
        {
            var labReports = new List<LabReport>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT LabReportId, ReportCode, PatientId, LabTestId, DoctorId, LabTechnicianId,
                           CollectionDate, ReportDate, ResultValue, ResultUnit,
                           Findings, Status, Notes, CreatedDate
                    FROM TblLabReports
                    WHERE PatientId = @PatientId
                    ORDER BY CreatedDate DESC";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    labReports.Add(new LabReport
                    {
                        LabReportId = reader.GetInt32(reader.GetOrdinal("LabReportId")),
                        ReportCode = reader.GetString(reader.GetOrdinal("ReportCode")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        LabTechnicianId = reader.GetInt32(reader.GetOrdinal("LabTechnicianId")),
                        CollectionDate = reader.GetDateTime(reader.GetOrdinal("CollectionDate")),
                        ReportDate = reader.IsDBNull(reader.GetOrdinal("ReportDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ReportDate")),
                        ResultValue = reader.IsDBNull(reader.GetOrdinal("ResultValue")) ? null : reader.GetString(reader.GetOrdinal("ResultValue")),
                        ResultUnit = reader.IsDBNull(reader.GetOrdinal("ResultUnit")) ? null : reader.GetString(reader.GetOrdinal("ResultUnit")),
                        Findings = reader.IsDBNull(reader.GetOrdinal("Findings")) ? null : reader.GetString(reader.GetOrdinal("Findings")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                    });
                }

                return labReports;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting lab reports for patient: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<LabReport>> GetLabReportsByDoctorAsync(int doctorId)
        {
            var labReports = new List<LabReport>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT lr.LabReportId, lr.ReportCode, lr.PatientId, lr.LabTestId, lr.DoctorId, lr.LabTechnicianId,
                           lr.CollectionDate, lr.ReportDate, lr.ResultValue, lr.ResultUnit,
                           lr.Findings, lr.Status, lr.Notes, lr.CreatedDate,
                           p.FirstName + ' ' + p.LastName AS PatientName,
                           lt.TestName,
                           d.FullName AS DoctorName,
                           u.FullName AS LabTechnicianName
                    FROM TblLabReports lr
                    JOIN TblPatients p ON lr.PatientId = p.PatientId
                    JOIN TblLabTests lt ON lr.LabTestId = lt.LabTestId
                    JOIN TblUsers d ON lr.DoctorId = d.UserId
                    JOIN TblUsers u ON lr.LabTechnicianId = u.UserId
                    WHERE lr.DoctorId = @DoctorId
                    ORDER BY lr.CreatedDate DESC";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    labReports.Add(new LabReport
                    {
                        LabReportId = reader.GetInt32(reader.GetOrdinal("LabReportId")),
                        ReportCode = reader.GetString(reader.GetOrdinal("ReportCode")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        LabTechnicianId = reader.GetInt32(reader.GetOrdinal("LabTechnicianId")),
                        CollectionDate = reader.GetDateTime(reader.GetOrdinal("CollectionDate")),
                        ReportDate = reader.IsDBNull(reader.GetOrdinal("ReportDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ReportDate")),
                        ResultValue = reader.IsDBNull(reader.GetOrdinal("ResultValue")) ? null : reader.GetString(reader.GetOrdinal("ResultValue")),
                        ResultUnit = reader.IsDBNull(reader.GetOrdinal("ResultUnit")) ? null : reader.GetString(reader.GetOrdinal("ResultUnit")),
                        Findings = reader.IsDBNull(reader.GetOrdinal("Findings")) ? null : reader.GetString(reader.GetOrdinal("Findings")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        PatientName = reader.IsDBNull(reader.GetOrdinal("PatientName")) ? null : reader.GetString(reader.GetOrdinal("PatientName")),
                        TestName = reader.IsDBNull(reader.GetOrdinal("TestName")) ? null : reader.GetString(reader.GetOrdinal("TestName")),
                        DoctorName = reader.IsDBNull(reader.GetOrdinal("DoctorName")) ? null : reader.GetString(reader.GetOrdinal("DoctorName")),
                        LabTechnicianName = reader.IsDBNull(reader.GetOrdinal("LabTechnicianName")) ? null : reader.GetString(reader.GetOrdinal("LabTechnicianName"))
                    });
                }

                return labReports;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting lab reports for doctor: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<LabReport>> GetPendingLabReportsAsync()
        {
            var labReports = new List<LabReport>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT lr.LabReportId, lr.ReportCode, lr.PatientId, lr.LabTestId, lr.DoctorId, lr.LabTechnicianId,
                           lr.CollectionDate, lr.ReportDate, lr.ResultValue, lr.ResultUnit,
                           lr.Findings, lr.Status, lr.Notes, lr.CreatedDate,
                           p.FirstName + ' ' + p.LastName AS PatientName,
                           lt.TestName,
                           d.FullName AS DoctorName,
                           u.FullName AS LabTechnicianName
                    FROM TblLabReports lr
                    JOIN TblPatients p ON lr.PatientId = p.PatientId
                    JOIN TblLabTests lt ON lr.LabTestId = lt.LabTestId
                    JOIN TblUsers d ON lr.DoctorId = d.UserId
                    JOIN TblUsers u ON lr.LabTechnicianId = u.UserId
                    WHERE lr.Status = 'Pending'
                    ORDER BY lr.CreatedDate ASC";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    labReports.Add(new LabReport
                    {
                        LabReportId = reader.GetInt32(reader.GetOrdinal("LabReportId")),
                        ReportCode = reader.GetString(reader.GetOrdinal("ReportCode")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        LabTechnicianId = reader.GetInt32(reader.GetOrdinal("LabTechnicianId")),
                        CollectionDate = reader.GetDateTime(reader.GetOrdinal("CollectionDate")),
                        ReportDate = reader.IsDBNull(reader.GetOrdinal("ReportDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ReportDate")),
                        ResultValue = reader.IsDBNull(reader.GetOrdinal("ResultValue")) ? null : reader.GetString(reader.GetOrdinal("ResultValue")),
                        ResultUnit = reader.IsDBNull(reader.GetOrdinal("ResultUnit")) ? null : reader.GetString(reader.GetOrdinal("ResultUnit")),
                        Findings = reader.IsDBNull(reader.GetOrdinal("Findings")) ? null : reader.GetString(reader.GetOrdinal("Findings")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                    });
                }

                return labReports;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting pending lab reports: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<LabReport>> GetLabReportsByTechnicianAsync(int technicianId)
        {
            var labReports = new List<LabReport>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT lr.LabReportId, lr.ReportCode, lr.PatientId, lr.LabTestId, lr.DoctorId, lr.LabTechnicianId,
                           lr.CollectionDate, lr.ReportDate, lr.ResultValue, lr.ResultUnit,
                           lr.Findings, lr.Status, lr.Notes, lr.CreatedDate,
                           p.FirstName + ' ' + p.LastName AS PatientName,
                           lt.TestName,
                           d.FullName AS DoctorName,
                           u.FullName AS LabTechnicianName
                    FROM TblLabReports lr
                    JOIN TblPatients p ON lr.PatientId = p.PatientId
                    JOIN TblLabTests lt ON lr.LabTestId = lt.LabTestId
                    JOIN TblUsers d ON lr.DoctorId = d.UserId
                    JOIN TblUsers u ON lr.LabTechnicianId = u.UserId
                    WHERE lr.LabTechnicianId = @TechnicianId
                    ORDER BY lr.CreatedDate DESC";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@TechnicianId", technicianId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    labReports.Add(new LabReport
                    {
                        LabReportId = reader.GetInt32(reader.GetOrdinal("LabReportId")),
                        ReportCode = reader.GetString(reader.GetOrdinal("ReportCode")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        LabTechnicianId = reader.GetInt32(reader.GetOrdinal("LabTechnicianId")),
                        CollectionDate = reader.GetDateTime(reader.GetOrdinal("CollectionDate")),
                        ReportDate = reader.IsDBNull(reader.GetOrdinal("ReportDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ReportDate")),
                        ResultValue = reader.IsDBNull(reader.GetOrdinal("ResultValue")) ? null : reader.GetString(reader.GetOrdinal("ResultValue")),
                        ResultUnit = reader.IsDBNull(reader.GetOrdinal("ResultUnit")) ? null : reader.GetString(reader.GetOrdinal("ResultUnit")),
                        Findings = reader.IsDBNull(reader.GetOrdinal("Findings")) ? null : reader.GetString(reader.GetOrdinal("Findings")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                    });
                }

                return labReports;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting lab reports for technician: {ex.Message}", ex);
            }
        }

        public async Task<LabReport> AddLabReportAsync(LabReport labReport)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    INSERT INTO TblLabReports (ReportCode, PatientId, LabTestId, DoctorId, LabTechnicianId,
                                               CollectionDate, ReportDate, ResultValue, ResultUnit,
                                               Findings, Status, Notes, CreatedDate)
                    OUTPUT INSERTED.LabReportId
                    VALUES (@ReportCode, @PatientId, @LabTestId, @DoctorId, @LabTechnicianId,
                            @CollectionDate, @ReportDate, @ResultValue, @ResultUnit,
                            @Findings, @Status, @Notes, @CreatedDate)";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ReportCode", labReport.ReportCode);
                command.Parameters.AddWithValue("@PatientId", labReport.PatientId);
                command.Parameters.AddWithValue("@LabTestId", labReport.LabTestId);
                command.Parameters.AddWithValue("@DoctorId", labReport.DoctorId);
                command.Parameters.AddWithValue("@LabTechnicianId", labReport.LabTechnicianId);
                command.Parameters.AddWithValue("@CollectionDate", labReport.CollectionDate);
                command.Parameters.AddWithValue("@ReportDate", labReport.ReportDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ResultValue", labReport.ResultValue ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ResultUnit", labReport.ResultUnit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Findings", labReport.Findings ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Status", labReport.Status ?? "Pending");
                command.Parameters.AddWithValue("@Notes", labReport.Notes ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDate", labReport.CreatedDate);

                var result = await command.ExecuteScalarAsync();
                labReport.LabReportId = Convert.ToInt32(result);

                return labReport;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding lab report: {ex.Message}", ex);
            }
        }

        public async Task<LabReport> UpdateLabReportAsync(LabReport labReport)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    UPDATE TblLabReports
                    SET ReportDate = @ReportDate, ResultValue = @ResultValue, ResultUnit = @ResultUnit,
                        Findings = @Findings, Status = @Status, Notes = @Notes
                    WHERE LabReportId = @LabReportId";

                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@LabReportId", labReport.LabReportId);
                command.Parameters.AddWithValue("@ReportDate", labReport.ReportDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ResultValue", labReport.ResultValue ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ResultUnit", labReport.ResultUnit ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Findings", labReport.Findings ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Status", labReport.Status ?? "Pending");
                command.Parameters.AddWithValue("@Notes", labReport.Notes ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();
                return labReport;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating lab report: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteLabReportAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = "DELETE FROM TblLabReports WHERE LabReportId = @LabReportId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@LabReportId", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting lab report: {ex.Message}", ex);
            }
        }

        // Lab Test Prescription methods
        public async Task<LabTestPrescription> AddLabTestPrescriptionAsync(LabTestPrescription prescription)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    INSERT INTO TblLabTestPrescriptions (AppointmentId, PatientId, DoctorId, LabTestId,
                                                        PrescriptionDate, ClinicalNotes, Priority, Status,
                                                        AssignedTechnicianId, Notes, CreatedDate)
                    OUTPUT INSERTED.LabTestPrescriptionId
                    VALUES (@AppointmentId, @PatientId, @DoctorId, @LabTestId,
                            @PrescriptionDate, @ClinicalNotes, @Priority, @Status,
                            @AssignedTechnicianId, @Notes, @CreatedDate)";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@AppointmentId", prescription.AppointmentId);
                command.Parameters.AddWithValue("@PatientId", prescription.PatientId);
                command.Parameters.AddWithValue("@DoctorId", prescription.DoctorId);
                command.Parameters.AddWithValue("@LabTestId", prescription.LabTestId);
                command.Parameters.AddWithValue("@PrescriptionDate", prescription.PrescriptionDate);
                command.Parameters.AddWithValue("@ClinicalNotes", prescription.ClinicalNotes ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Priority", prescription.Priority ?? "Normal");
                command.Parameters.AddWithValue("@Status", prescription.Status ?? "Prescribed");
                command.Parameters.AddWithValue("@AssignedTechnicianId", prescription.AssignedTechnicianId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Notes", prescription.Notes ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDate", prescription.CreatedDate);

                var result = await command.ExecuteScalarAsync();
                prescription.LabTestPrescriptionId = Convert.ToInt32(result);

                return prescription;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding lab test prescription: {ex.Message}", ex);
            }
        }

        public async Task<LabTestPrescription?> GetLabTestPrescriptionByIdAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT ltp.LabTestPrescriptionId, ltp.AppointmentId, ltp.PatientId, ltp.DoctorId, ltp.LabTestId,
                           ltp.PrescriptionDate, ltp.ClinicalNotes, ltp.Priority, ltp.Status,
                           ltp.SampleCollectionDate, ltp.AssignedTechnicianId, ltp.CompletedDate, ltp.Notes, ltp.CreatedDate,
                           p.FirstName + ' ' + p.LastName AS PatientName,
                           d.FullName AS DoctorName,
                           lt.TestName, lt.TestCode, lt.Price,
                           u.FullName AS LabTechnicianName
                    FROM TblLabTestPrescriptions ltp
                    JOIN TblPatients p ON ltp.PatientId = p.PatientId
                    JOIN TblUsers d ON ltp.DoctorId = d.UserId
                    JOIN TblLabTests lt ON ltp.LabTestId = lt.LabTestId
                    LEFT JOIN TblUsers u ON ltp.AssignedTechnicianId = u.UserId
                    WHERE ltp.LabTestPrescriptionId = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new LabTestPrescription
                    {
                        LabTestPrescriptionId = reader.GetInt32(reader.GetOrdinal("LabTestPrescriptionId")),
                        AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                        ClinicalNotes = reader.IsDBNull(reader.GetOrdinal("ClinicalNotes")) ? null : reader.GetString(reader.GetOrdinal("ClinicalNotes")),
                        Priority = reader.IsDBNull(reader.GetOrdinal("Priority")) ? null : reader.GetString(reader.GetOrdinal("Priority")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        SampleCollectionDate = reader.IsDBNull(reader.GetOrdinal("SampleCollectionDate")) ? null : reader.GetDateTime(reader.GetOrdinal("SampleCollectionDate")),
                        AssignedTechnicianId = reader.IsDBNull(reader.GetOrdinal("AssignedTechnicianId")) ? null : reader.GetInt32(reader.GetOrdinal("AssignedTechnicianId")),
                        CompletedDate = reader.IsDBNull(reader.GetOrdinal("CompletedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("CompletedDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        PatientName = reader.IsDBNull(reader.GetOrdinal("PatientName")) ? null : reader.GetString(reader.GetOrdinal("PatientName")),
                        DoctorName = reader.IsDBNull(reader.GetOrdinal("DoctorName")) ? null : reader.GetString(reader.GetOrdinal("DoctorName")),
                        TestName = reader.IsDBNull(reader.GetOrdinal("TestName")) ? null : reader.GetString(reader.GetOrdinal("TestName")),
                        TestCode = reader.IsDBNull(reader.GetOrdinal("TestCode")) ? null : reader.GetString(reader.GetOrdinal("TestCode")),
                        TestPrice = reader.GetDecimal(reader.GetOrdinal("Price")),
                        LabTechnicianName = reader.IsDBNull(reader.GetOrdinal("LabTechnicianName")) ? null : reader.GetString(reader.GetOrdinal("LabTechnicianName"))
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting lab test prescription: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<LabTestPrescription>> GetAllLabTestPrescriptionsAsync()
        {
            var prescriptions = new List<LabTestPrescription>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT ltp.LabTestPrescriptionId, ltp.AppointmentId, ltp.PatientId, ltp.DoctorId, ltp.LabTestId,
                           ltp.PrescriptionDate, ltp.ClinicalNotes, ltp.Priority, ltp.Status,
                           ltp.SampleCollectionDate, ltp.AssignedTechnicianId, ltp.CompletedDate, ltp.Notes, ltp.CreatedDate,
                           p.FirstName + ' ' + p.LastName AS PatientName,
                           d.FullName AS DoctorName,
                           lt.TestName, lt.TestCode, lt.Price,
                           u.FullName AS LabTechnicianName
                    FROM TblLabTestPrescriptions ltp
                    JOIN TblPatients p ON ltp.PatientId = p.PatientId
                    JOIN TblUsers d ON ltp.DoctorId = d.UserId
                    JOIN TblLabTests lt ON ltp.LabTestId = lt.LabTestId
                    LEFT JOIN TblUsers u ON ltp.AssignedTechnicianId = u.UserId
                    ORDER BY ltp.CreatedDate DESC";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    prescriptions.Add(new LabTestPrescription
                    {
                        LabTestPrescriptionId = reader.GetInt32(reader.GetOrdinal("LabTestPrescriptionId")),
                        AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                        ClinicalNotes = reader.IsDBNull(reader.GetOrdinal("ClinicalNotes")) ? null : reader.GetString(reader.GetOrdinal("ClinicalNotes")),
                        Priority = reader.IsDBNull(reader.GetOrdinal("Priority")) ? null : reader.GetString(reader.GetOrdinal("Priority")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        SampleCollectionDate = reader.IsDBNull(reader.GetOrdinal("SampleCollectionDate")) ? null : reader.GetDateTime(reader.GetOrdinal("SampleCollectionDate")),
                        AssignedTechnicianId = reader.IsDBNull(reader.GetOrdinal("AssignedTechnicianId")) ? null : reader.GetInt32(reader.GetOrdinal("AssignedTechnicianId")),
                        CompletedDate = reader.IsDBNull(reader.GetOrdinal("CompletedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("CompletedDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        PatientName = reader.IsDBNull(reader.GetOrdinal("PatientName")) ? null : reader.GetString(reader.GetOrdinal("PatientName")),
                        DoctorName = reader.IsDBNull(reader.GetOrdinal("DoctorName")) ? null : reader.GetString(reader.GetOrdinal("DoctorName")),
                        TestName = reader.IsDBNull(reader.GetOrdinal("TestName")) ? null : reader.GetString(reader.GetOrdinal("TestName")),
                        TestCode = reader.IsDBNull(reader.GetOrdinal("TestCode")) ? null : reader.GetString(reader.GetOrdinal("TestCode")),
                        TestPrice = reader.GetDecimal(reader.GetOrdinal("Price")),
                        LabTechnicianName = reader.IsDBNull(reader.GetOrdinal("LabTechnicianName")) ? null : reader.GetString(reader.GetOrdinal("LabTechnicianName"))
                    });
                }

                return prescriptions;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all lab test prescriptions: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<LabTestPrescription>> GetPendingLabTestPrescriptionsAsync()
        {
            var prescriptions = new List<LabTestPrescription>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT ltp.LabTestPrescriptionId, ltp.AppointmentId, ltp.PatientId, ltp.DoctorId, ltp.LabTestId,
                           ltp.PrescriptionDate, ltp.ClinicalNotes, ltp.Priority, ltp.Status,
                           ltp.SampleCollectionDate, ltp.AssignedTechnicianId, ltp.CompletedDate, ltp.Notes, ltp.CreatedDate,
                           p.FirstName + ' ' + p.LastName AS PatientName,
                           d.FullName AS DoctorName,
                           lt.TestName, lt.TestCode, lt.Price,
                           u.FullName AS LabTechnicianName
                    FROM TblLabTestPrescriptions ltp
                    JOIN TblPatients p ON ltp.PatientId = p.PatientId
                    JOIN TblUsers d ON ltp.DoctorId = d.UserId
                    JOIN TblLabTests lt ON ltp.LabTestId = lt.LabTestId
                    LEFT JOIN TblUsers u ON ltp.AssignedTechnicianId = u.UserId
                    WHERE ltp.Status IN ('Prescribed', 'Assigned')
                    ORDER BY ltp.Priority DESC, ltp.CreatedDate ASC";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    prescriptions.Add(new LabTestPrescription
                    {
                        LabTestPrescriptionId = reader.GetInt32(reader.GetOrdinal("LabTestPrescriptionId")),
                        AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                        ClinicalNotes = reader.IsDBNull(reader.GetOrdinal("ClinicalNotes")) ? null : reader.GetString(reader.GetOrdinal("ClinicalNotes")),
                        Priority = reader.IsDBNull(reader.GetOrdinal("Priority")) ? null : reader.GetString(reader.GetOrdinal("Priority")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        SampleCollectionDate = reader.IsDBNull(reader.GetOrdinal("SampleCollectionDate")) ? null : reader.GetDateTime(reader.GetOrdinal("SampleCollectionDate")),
                        AssignedTechnicianId = reader.IsDBNull(reader.GetOrdinal("AssignedTechnicianId")) ? null : reader.GetInt32(reader.GetOrdinal("AssignedTechnicianId")),
                        CompletedDate = reader.IsDBNull(reader.GetOrdinal("CompletedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("CompletedDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        PatientName = reader.IsDBNull(reader.GetOrdinal("PatientName")) ? null : reader.GetString(reader.GetOrdinal("PatientName")),
                        DoctorName = reader.IsDBNull(reader.GetOrdinal("DoctorName")) ? null : reader.GetString(reader.GetOrdinal("DoctorName")),
                        TestName = reader.IsDBNull(reader.GetOrdinal("TestName")) ? null : reader.GetString(reader.GetOrdinal("TestName")),
                        TestCode = reader.IsDBNull(reader.GetOrdinal("TestCode")) ? null : reader.GetString(reader.GetOrdinal("TestCode")),
                        TestPrice = reader.GetDecimal(reader.GetOrdinal("Price")),
                        LabTechnicianName = reader.IsDBNull(reader.GetOrdinal("LabTechnicianName")) ? null : reader.GetString(reader.GetOrdinal("LabTechnicianName"))
                    });
                }

                return prescriptions;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting pending lab test prescriptions: {ex.Message}", ex);
            }
        }

        public async Task<LabTestPrescription> UpdateLabTestPrescriptionAsync(LabTestPrescription prescription)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    UPDATE TblLabTestPrescriptions
                    SET Status = @Status, SampleCollectionDate = @SampleCollectionDate,
                        AssignedTechnicianId = @AssignedTechnicianId, CompletedDate = @CompletedDate,
                        Notes = @Notes
                    WHERE LabTestPrescriptionId = @LabTestPrescriptionId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@LabTestPrescriptionId", prescription.LabTestPrescriptionId);
                command.Parameters.AddWithValue("@Status", prescription.Status ?? "Prescribed");
                command.Parameters.AddWithValue("@SampleCollectionDate", prescription.SampleCollectionDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AssignedTechnicianId", prescription.AssignedTechnicianId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CompletedDate", prescription.CompletedDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Notes", prescription.Notes ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();
                return prescription;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating lab test prescription: {ex.Message}", ex);
            }
        }

        public async Task<bool> AssignTechnicianToPrescriptionAsync(int prescriptionId, int technicianId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    UPDATE TblLabTestPrescriptions
                    SET AssignedTechnicianId = @TechnicianId, Status = 'Assigned'
                    WHERE LabTestPrescriptionId = @PrescriptionId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
                command.Parameters.AddWithValue("@TechnicianId", technicianId);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error assigning technician to prescription: {ex.Message}", ex);
            }
        }

        // Helper methods for getting prescriptions by different criteria
        public async Task<IEnumerable<LabTestPrescription>> GetLabTestPrescriptionsByPatientAsync(int patientId)
        {
            var prescriptions = new List<LabTestPrescription>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT ltp.LabTestPrescriptionId, ltp.AppointmentId, ltp.PatientId, ltp.DoctorId, ltp.LabTestId,
                           ltp.PrescriptionDate, ltp.ClinicalNotes, ltp.Priority, ltp.Status,
                           ltp.SampleCollectionDate, ltp.AssignedTechnicianId, ltp.CompletedDate, ltp.Notes, ltp.CreatedDate,
                           d.FullName AS DoctorName, lt.TestName, lt.TestCode, lt.Price, u.FullName AS LabTechnicianName
                    FROM TblLabTestPrescriptions ltp
                    JOIN TblUsers d ON ltp.DoctorId = d.UserId
                    JOIN TblLabTests lt ON ltp.LabTestId = lt.LabTestId
                    LEFT JOIN TblUsers u ON ltp.AssignedTechnicianId = u.UserId
                    WHERE ltp.PatientId = @PatientId
                    ORDER BY ltp.CreatedDate DESC";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    prescriptions.Add(new LabTestPrescription
                    {
                        LabTestPrescriptionId = reader.GetInt32(reader.GetOrdinal("LabTestPrescriptionId")),
                        AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                        ClinicalNotes = reader.IsDBNull(reader.GetOrdinal("ClinicalNotes")) ? null : reader.GetString(reader.GetOrdinal("ClinicalNotes")),
                        Priority = reader.IsDBNull(reader.GetOrdinal("Priority")) ? null : reader.GetString(reader.GetOrdinal("Priority")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        SampleCollectionDate = reader.IsDBNull(reader.GetOrdinal("SampleCollectionDate")) ? null : reader.GetDateTime(reader.GetOrdinal("SampleCollectionDate")),
                        AssignedTechnicianId = reader.IsDBNull(reader.GetOrdinal("AssignedTechnicianId")) ? null : reader.GetInt32(reader.GetOrdinal("AssignedTechnicianId")),
                        CompletedDate = reader.IsDBNull(reader.GetOrdinal("CompletedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("CompletedDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        DoctorName = reader.IsDBNull(reader.GetOrdinal("DoctorName")) ? null : reader.GetString(reader.GetOrdinal("DoctorName")),
                        TestName = reader.IsDBNull(reader.GetOrdinal("TestName")) ? null : reader.GetString(reader.GetOrdinal("TestName")),
                        TestCode = reader.IsDBNull(reader.GetOrdinal("TestCode")) ? null : reader.GetString(reader.GetOrdinal("TestCode")),
                        TestPrice = reader.GetDecimal(reader.GetOrdinal("Price")),
                        LabTechnicianName = reader.IsDBNull(reader.GetOrdinal("LabTechnicianName")) ? null : reader.GetString(reader.GetOrdinal("LabTechnicianName"))
                    });
                }

                return prescriptions;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting lab test prescriptions for patient: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<LabTestPrescription>> GetLabTestPrescriptionsByDoctorAsync(int doctorId)
        {
            var prescriptions = new List<LabTestPrescription>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT ltp.LabTestPrescriptionId, ltp.AppointmentId, ltp.PatientId, ltp.DoctorId, ltp.LabTestId,
                           ltp.PrescriptionDate, ltp.ClinicalNotes, ltp.Priority, ltp.Status,
                           ltp.SampleCollectionDate, ltp.AssignedTechnicianId, ltp.CompletedDate, ltp.Notes, ltp.CreatedDate,
                           p.FirstName + ' ' + p.LastName AS PatientName, lt.TestName, lt.TestCode, lt.Price,
                           u.FullName AS LabTechnicianName
                    FROM TblLabTestPrescriptions ltp
                    JOIN TblPatients p ON ltp.PatientId = p.PatientId
                    JOIN TblLabTests lt ON ltp.LabTestId = lt.LabTestId
                    LEFT JOIN TblUsers u ON ltp.AssignedTechnicianId = u.UserId
                    WHERE ltp.DoctorId = @DoctorId
                    ORDER BY ltp.CreatedDate DESC";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@DoctorId", doctorId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    prescriptions.Add(new LabTestPrescription
                    {
                        LabTestPrescriptionId = reader.GetInt32(reader.GetOrdinal("LabTestPrescriptionId")),
                        AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                        ClinicalNotes = reader.IsDBNull(reader.GetOrdinal("ClinicalNotes")) ? null : reader.GetString(reader.GetOrdinal("ClinicalNotes")),
                        Priority = reader.IsDBNull(reader.GetOrdinal("Priority")) ? null : reader.GetString(reader.GetOrdinal("Priority")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        SampleCollectionDate = reader.IsDBNull(reader.GetOrdinal("SampleCollectionDate")) ? null : reader.GetDateTime(reader.GetOrdinal("SampleCollectionDate")),
                        AssignedTechnicianId = reader.IsDBNull(reader.GetOrdinal("AssignedTechnicianId")) ? null : reader.GetInt32(reader.GetOrdinal("AssignedTechnicianId")),
                        CompletedDate = reader.IsDBNull(reader.GetOrdinal("CompletedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("CompletedDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        PatientName = reader.IsDBNull(reader.GetOrdinal("PatientName")) ? null : reader.GetString(reader.GetOrdinal("PatientName")),
                        TestName = reader.IsDBNull(reader.GetOrdinal("TestName")) ? null : reader.GetString(reader.GetOrdinal("TestName")),
                        TestCode = reader.IsDBNull(reader.GetOrdinal("TestCode")) ? null : reader.GetString(reader.GetOrdinal("TestCode")),
                        TestPrice = reader.GetDecimal(reader.GetOrdinal("Price")),
                        LabTechnicianName = reader.IsDBNull(reader.GetOrdinal("LabTechnicianName")) ? null : reader.GetString(reader.GetOrdinal("LabTechnicianName"))
                    });
                }

                return prescriptions;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting lab test prescriptions for doctor: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<LabTestPrescription>> GetAssignedLabTestPrescriptionsAsync(int technicianId)
        {
            var prescriptions = new List<LabTestPrescription>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT ltp.LabTestPrescriptionId, ltp.AppointmentId, ltp.PatientId, ltp.DoctorId, ltp.LabTestId,
                           ltp.PrescriptionDate, ltp.ClinicalNotes, ltp.Priority, ltp.Status,
                           ltp.SampleCollectionDate, ltp.AssignedTechnicianId, ltp.CompletedDate, ltp.Notes, ltp.CreatedDate,
                           p.FirstName + ' ' + p.LastName AS PatientName,
                           d.FullName AS DoctorName,
                           lt.TestName, lt.TestCode, lt.Price
                    FROM TblLabTestPrescriptions ltp
                    JOIN TblPatients p ON ltp.PatientId = p.PatientId
                    JOIN TblUsers d ON ltp.DoctorId = d.UserId
                    JOIN TblLabTests lt ON ltp.LabTestId = lt.LabTestId
                    WHERE ltp.AssignedTechnicianId = @TechnicianId AND ltp.Status IN ('Assigned', 'InProgress')
                    ORDER BY ltp.Priority DESC, ltp.CreatedDate ASC";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@TechnicianId", technicianId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    prescriptions.Add(new LabTestPrescription
                    {
                        LabTestPrescriptionId = reader.GetInt32(reader.GetOrdinal("LabTestPrescriptionId")),
                        AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                        PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                        DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        LabTestId = reader.GetInt32(reader.GetOrdinal("LabTestId")),
                        PrescriptionDate = reader.GetDateTime(reader.GetOrdinal("PrescriptionDate")),
                        ClinicalNotes = reader.IsDBNull(reader.GetOrdinal("ClinicalNotes")) ? null : reader.GetString(reader.GetOrdinal("ClinicalNotes")),
                        Priority = reader.IsDBNull(reader.GetOrdinal("Priority")) ? null : reader.GetString(reader.GetOrdinal("Priority")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                        SampleCollectionDate = reader.IsDBNull(reader.GetOrdinal("SampleCollectionDate")) ? null : reader.GetDateTime(reader.GetOrdinal("SampleCollectionDate")),
                        AssignedTechnicianId = reader.IsDBNull(reader.GetOrdinal("AssignedTechnicianId")) ? null : reader.GetInt32(reader.GetOrdinal("AssignedTechnicianId")),
                        CompletedDate = reader.IsDBNull(reader.GetOrdinal("CompletedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("CompletedDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        PatientName = reader.IsDBNull(reader.GetOrdinal("PatientName")) ? null : reader.GetString(reader.GetOrdinal("PatientName")),
                        DoctorName = reader.IsDBNull(reader.GetOrdinal("DoctorName")) ? null : reader.GetString(reader.GetOrdinal("DoctorName")),
                        TestName = reader.IsDBNull(reader.GetOrdinal("TestName")) ? null : reader.GetString(reader.GetOrdinal("TestName")),
                        TestCode = reader.IsDBNull(reader.GetOrdinal("TestCode")) ? null : reader.GetString(reader.GetOrdinal("TestCode")),
                        TestPrice = reader.GetDecimal(reader.GetOrdinal("Price"))
                    });
                }

                return prescriptions;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting assigned lab test prescriptions: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteLabTestPrescriptionAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = "DELETE FROM TblLabTestPrescriptions WHERE LabTestPrescriptionId = @Id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting lab test prescription: {ex.Message}", ex);
            }
        }
    }
}