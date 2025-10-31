/*
using ClinicalManagementSystem2025.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace ClinicalManagementSystem2025.Repository
{
    public class SimpleMedicalNoteRepository : IMedicalNoteRepository
    {
        private readonly string _connectionString;

        public SimpleMedicalNoteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MVCConnectionString")
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string is missing");
        }

        public async Task<int> AddMedicalNoteAsync(MedicalNote medicalNote)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Check if LabTests column exists in the database
                var labTestsColumnExists = await CheckColumnExistsAsync(connection, "TblMedicalNotes", "LabTests");

                string sql;
                if (labTestsColumnExists)
                {
                    sql = @"
                        INSERT INTO TblMedicalNotes (
                            PatientId, DoctorId, AppointmentId, Title, Notes, Diagnosis,
                            Prescription, FollowUpInstructions, NextAppointmentDate,
                            NoteType, Priority, IsActive, CreatedBy, CreatedDate, LabTests
                        )
                        OUTPUT INSERTED.MedicalNoteId
                        VALUES (
                            @PatientId, @DoctorId, @AppointmentId, @Title, @Notes, @Diagnosis,
                            @Prescription, @FollowUpInstructions, @NextAppointmentDate,
                            @NoteType, @Priority, @IsActive, @CreatedBy, @CreatedDate, @LabTests
                        )";
                }
                else
                {
                    sql = @"
                        INSERT INTO TblMedicalNotes (
                            PatientId, DoctorId, AppointmentId, Title, Notes, Diagnosis,
                            Prescription, FollowUpInstructions, NextAppointmentDate,
                            NoteType, Priority, IsActive, CreatedBy, CreatedDate
                        )
                        OUTPUT INSERTED.MedicalNoteId
                        VALUES (
                            @PatientId, @DoctorId, @AppointmentId, @Title, @Notes, @Diagnosis,
                            @Prescription, @FollowUpInstructions, @NextAppointmentDate,
                            @NoteType, @Priority, @IsActive, @CreatedBy, @CreatedDate
                        )";
                }

                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@PatientId", medicalNote.PatientId);
                command.Parameters.AddWithValue("@DoctorId", medicalNote.DoctorId);
                command.Parameters.AddWithValue("@AppointmentId", medicalNote.AppointmentId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Title", medicalNote.Title);
                command.Parameters.AddWithValue("@Notes", medicalNote.Notes);
                command.Parameters.AddWithValue("@Diagnosis", medicalNote.Diagnosis ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Prescription", medicalNote.Prescription ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FollowUpInstructions", medicalNote.FollowUpInstructions ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@NextAppointmentDate", medicalNote.NextAppointmentDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@NoteType", medicalNote.NoteType);
                command.Parameters.AddWithValue("@Priority", medicalNote.Priority);
                command.Parameters.AddWithValue("@IsActive", medicalNote.IsActive);
                command.Parameters.AddWithValue("@CreatedBy", medicalNote.CreatedBy);
                command.Parameters.AddWithValue("@CreatedDate", medicalNote.CreatedDate);

                if (labTestsColumnExists)
                {
                    command.Parameters.AddWithValue("@LabTests", medicalNote.LabTests ?? (object)DBNull.Value);
                }

                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding medical note: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<MedicalNote>> GetMedicalNotesByPatientIdAsync(int patientId)
        {
            var medicalNotes = new List<MedicalNote>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Check if required columns exist
                var labTestsColumnExists = await CheckColumnExistsAsync(connection, "TblMedicalNotes", "LabTests");
                var prescriptionColumnExists = await CheckColumnExistsAsync(connection, "TblMedicalNotes", "Prescription");

                // Build SELECT query dynamically based on available columns
                var selectColumns = new List<string>
                {
                    "MedicalNoteId", "PatientId", "DoctorId", "AppointmentId", "Title", "Notes",
                    "Diagnosis", "FollowUpInstructions", "NextAppointmentDate", "NoteType",
                    "Priority", "IsActive", "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate"
                };

                if (prescriptionColumnExists)
                    selectColumns.Add("Prescription");
                if (labTestsColumnExists)
                    selectColumns.Add("LabTests");

                string sql = $@"
                    SELECT {string.Join(", ", selectColumns)}
                    FROM TblMedicalNotes
                    WHERE PatientId = @PatientId AND IsActive = 1
                    ORDER BY CreatedDate DESC";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    medicalNotes.Add(CreateMedicalNoteFromReader(reader, labTestsColumnExists, prescriptionColumnExists));
                }

                return medicalNotes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting medical notes for patient: {ex.Message}", ex);
            }
        }

        private async Task<bool> CheckColumnExistsAsync(SqlConnection connection, string tableName, string columnName)
        {
            try
            {
                const string sql = @"
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = @TableName AND COLUMN_NAME = @ColumnName";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@TableName", tableName);
                command.Parameters.AddWithValue("@ColumnName", columnName);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch
            {
                return false; // If we can't check, assume column doesn't exist
            }
        }

        private MedicalNote CreateMedicalNoteFromReader(SqlDataReader reader, bool labTestsColumnExists = true, bool prescriptionColumnExists = true)
        {
            var medicalNote = new MedicalNote
            {
                MedicalNoteId = reader.GetInt32(reader.GetOrdinal("MedicalNoteId")),
                PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                DoctorId = reader.GetInt32(reader.GetOrdinal("DoctorId")),
                AppointmentId = reader.IsDBNull(reader.GetOrdinal("AppointmentId")) ? null : reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Notes = reader.GetString(reader.GetOrdinal("Notes")),
                Diagnosis = reader.IsDBNull(reader.GetOrdinal("Diagnosis")) ? null : reader.GetString(reader.GetOrdinal("Diagnosis")),
                FollowUpInstructions = reader.IsDBNull(reader.GetOrdinal("FollowUpInstructions")) ? null : reader.GetString(reader.GetOrdinal("FollowUpInstructions")),
                NextAppointmentDate = reader.IsDBNull(reader.GetOrdinal("NextAppointmentDate")) ? null : reader.GetDateTime(reader.GetOrdinal("NextAppointmentDate")),
                NoteType = reader.GetString(reader.GetOrdinal("NoteType")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetInt32(reader.GetOrdinal("UpdatedBy")),
                UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedDate")),
                // Set navigation properties to null since we don't have user tables
                Doctor = null,
                Patient = null
            };

            // Handle Prescription column conditionally
            if (prescriptionColumnExists)
            {
                try
                {
                    medicalNote.Prescription = reader.IsDBNull(reader.GetOrdinal("Prescription")) ? null : reader.GetString(reader.GetOrdinal("Prescription"));
                }
                catch (IndexOutOfRangeException)
                {
                    medicalNote.Prescription = null;
                }
            }

            // Handle LabTests column conditionally
            if (labTestsColumnExists)
            {
                try
                {
                    medicalNote.LabTests = reader.IsDBNull(reader.GetOrdinal("LabTests")) ? null : reader.GetString(reader.GetOrdinal("LabTests"));
                }
                catch (IndexOutOfRangeException)
                {
                    medicalNote.LabTests = null;
                }
            }

            return medicalNote;
        }
    }
}
*/

// This file is disabled to avoid namespace conflicts.
// The main MedicalNoteRepository.cs is the active implementation.
// If you need this alternative version, uncomment the code above.
