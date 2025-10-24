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
            _connectionString = configuration.GetConnectionString("MVCConnectionString");
        }

        public async Task<List<Appointment>> GetTodayAppointments(int doctorId)
        {
            var appointments = new List<Appointment>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_GetTodayAppointmentsForDoctor", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DoctorId", doctorId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var appointment = new Appointment
                {
                    AppointmentId = reader.GetInt32("AppointmentId"),
                    PatientId = reader.GetInt32("PatientId"),
                    AppointmentDate = reader.GetDateTime("AppointmentDate"),
                    AppointmentTime = reader.GetDateTime("AppointmentTime"),
                    Status = reader.GetString("Status"),
                    Reason = reader.GetString("Reason"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
                    Patient = new Patient
                    {
                        PatientId = reader.GetInt32("PatientId"),
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName"),
                        Gender = reader.GetString("Gender"),
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Phone = reader.GetString("Phone"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email")
                    }
                };
                appointments.Add(appointment);
            }

            return appointments;
        }

        public async Task<Patient> GetPatientById(int patientId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("SELECT * FROM TblPatients WHERE PatientId = @PatientId", connection);
            command.Parameters.AddWithValue("@PatientId", patientId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Patient
                {
                    PatientId = reader.GetInt32("PatientId"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Gender = reader.GetString("Gender"),
                    DateOfBirth = reader.GetDateTime("DateOfBirth"),
                    Phone = reader.GetString("Phone"),
                    Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                    Address = reader.IsDBNull("Address") ? null : reader.GetString("Address"),
                    BloodGroup = reader.IsDBNull("BloodGroup") ? null : reader.GetString("BloodGroup"),
                    EmergencyContact = reader.IsDBNull("EmergencyContact") ? null : reader.GetString("EmergencyContact")
                };
            }
            return null;
        }

        public async Task<List<Prescription>> GetPatientPrescriptions(int patientId)
        {
            var prescriptions = new List<Prescription>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("SELECT * FROM TblPrescriptions WHERE PatientId = @PatientId ORDER BY PrescriptionDate DESC", connection);
            command.Parameters.AddWithValue("@PatientId", patientId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                prescriptions.Add(new Prescription
                {
                    PrescriptionId = reader.GetInt32("PrescriptionId"),
                    AppointmentId = reader.GetInt32("AppointmentId"),
                    PatientId = reader.GetInt32("PatientId"),
                    DoctorId = reader.GetInt32("DoctorId"),
                    Diagnosis = reader.GetString("Diagnosis"),
                    PrescriptionDate = reader.GetDateTime("PrescriptionDate"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes")
                });
            }

            return prescriptions;
        }

        public async Task<List<LabReport>> GetPatientLabReports(int patientId)
        {
            var labReports = new List<LabReport>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("SELECT * FROM TblLabReports WHERE PatientId = @PatientId ORDER BY TestDate DESC", connection);
            command.Parameters.AddWithValue("@PatientId", patientId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                labReports.Add(new LabReport
                {
                    LabReportId = reader.GetInt32("LabReportId"),
                    PatientId = reader.GetInt32("PatientId"),
                    DoctorId = reader.GetInt32("DoctorId"),
                    TestName = reader.GetString("TestName"),
                    TestDate = reader.GetDateTime("TestDate"),
                    Result = reader.IsDBNull("Result") ? null : reader.GetString("Result"),
                    Status = reader.GetString("Status")
                });
            }

            return labReports;
        }

        public async Task<List<Medicine>> GetActiveMedicines()
        {
            var medicines = new List<Medicine>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("SELECT MedicineId, MedicineName FROM TblMedicines WHERE IsActive = 1", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                medicines.Add(new Medicine
                {
                    MedicineId = reader.GetInt32("MedicineId"),
                    MedicineName = reader.GetString("MedicineName")
                });
            }

            return medicines;
        }

        public async Task<bool> UpdateAppointment(int appointmentId, string symptoms, string diagnosis, string notes)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_AddDoctorNotes", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@AppointmentId", appointmentId);
            command.Parameters.AddWithValue("@Symptoms", symptoms);
            command.Parameters.AddWithValue("@Diagnosis", diagnosis);
            command.Parameters.AddWithValue("@Notes", notes);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<int> CreatePrescription(Prescription prescription)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Debug: Check parameter values
                Console.WriteLine($"Creating prescription - Patient: {prescription.PatientId}, Doctor: {prescription.DoctorId}, Diagnosis: {prescription.Diagnosis}");

                using var command = new SqlCommand(@"
            INSERT INTO TblPrescriptions (AppointmentId, PatientId, DoctorId, Diagnosis, PrescriptionDate, Instructions, Notes)
            VALUES (@AppointmentId, @PatientId, @DoctorId, @Diagnosis, @PrescriptionDate, @Instructions, @Notes);
            SELECT SCOPE_IDENTITY();", connection);

                command.Parameters.AddWithValue("@AppointmentId", prescription.AppointmentId);
                command.Parameters.AddWithValue("@PatientId", prescription.PatientId);
                command.Parameters.AddWithValue("@DoctorId", prescription.DoctorId);
                command.Parameters.AddWithValue("@Diagnosis", prescription.Diagnosis);
                command.Parameters.AddWithValue("@PrescriptionDate", prescription.PrescriptionDate);
                command.Parameters.AddWithValue("@Instructions", prescription.Instructions ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Notes", prescription.Notes ?? (object)DBNull.Value);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    int prescriptionId = Convert.ToInt32(result);
                    Console.WriteLine($"Prescription created successfully with ID: {prescriptionId}");
                    return prescriptionId;
                }
                else
                {
                    Console.WriteLine(" Failed to get prescription ID after insert");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error creating prescription: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return 0;
            }
        }
        public async Task<bool> AddPrescriptionDetails(List<PrescriptionDetail> prescriptionDetails)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            foreach (var detail in prescriptionDetails)
            {
                using var command = new SqlCommand(@"
                    INSERT INTO TblPrescriptionDetails (PrescriptionId, MedicineId, Dosage, Frequency, Duration, Instructions)
                    VALUES (@PrescriptionId, @MedicineId, @Dosage, @Frequency, @Duration, @Instructions)", connection);

                command.Parameters.AddWithValue("@PrescriptionId", detail.PrescriptionId);
                command.Parameters.AddWithValue("@MedicineId", detail.MedicineId);
                command.Parameters.AddWithValue("@Dosage", detail.Dosage);
                command.Parameters.AddWithValue("@Frequency", detail.Frequency);
                command.Parameters.AddWithValue("@Duration", detail.Duration);
                command.Parameters.AddWithValue("@Instructions", detail.Instructions ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
            return true;
        }

        public async Task<bool> CreateLabReport(LabReport labReport)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                INSERT INTO TblLabReports (PatientId, DoctorId, TestName, TestDate, Result, TechnicianId, Status)
                VALUES (@PatientId, @DoctorId, @TestName, @TestDate, @Result, @TechnicianId, @Status)", connection);

            command.Parameters.AddWithValue("@PatientId", labReport.PatientId);
            command.Parameters.AddWithValue("@DoctorId", labReport.DoctorId);
            command.Parameters.AddWithValue("@TestName", labReport.TestName);
            command.Parameters.AddWithValue("@TestDate", labReport.TestDate);
            command.Parameters.AddWithValue("@Result", labReport.Result ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@TechnicianId", labReport.TechnicianId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Status", labReport.Status);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<int> GetDoctorIdByUserId(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_GetDoctorIdByUserId", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);

            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // Get all patients for a specific doctor
        public async Task<List<Patient>> GetDoctorPatients(int doctorId)
        {
            var patients = new List<Patient>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_GetDoctorPatients", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DoctorId", doctorId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                patients.Add(new Patient
                {
                    PatientId = reader.GetInt32("PatientId"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Gender = reader.GetString("Gender"),
                    DateOfBirth = reader.GetDateTime("DateOfBirth"),
                    Phone = reader.GetString("Phone"),
                    Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                    Address = reader.IsDBNull("Address") ? null : reader.GetString("Address"),
                    BloodGroup = reader.IsDBNull("BloodGroup") ? null : reader.GetString("BloodGroup"),
                    EmergencyContact = reader.IsDBNull("EmergencyContact") ? null : reader.GetString("EmergencyContact"),
                    RegistrationDate = reader.GetDateTime("RegistrationDate")
                });
            }

            return patients;
        }

        // Save consultation notes
        public async Task<bool> SaveConsultationNotes(ConsultationNotes consultationNotes)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_SaveConsultationNotes", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@AppointmentId", consultationNotes.AppointmentId);
            command.Parameters.AddWithValue("@PatientId", consultationNotes.PatientId);
            command.Parameters.AddWithValue("@DoctorId", consultationNotes.DoctorId);
            command.Parameters.AddWithValue("@Symptoms", consultationNotes.Symptoms);
            command.Parameters.AddWithValue("@Diagnosis", consultationNotes.Diagnosis);
            command.Parameters.AddWithValue("@TreatmentPlan", consultationNotes.TreatmentPlan ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Notes", consultationNotes.Notes ?? (object)DBNull.Value);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        // Get consultation notes by patient
        public async Task<List<ConsultationNotes>> GetPatientConsultationNotes(int patientId)
        {
            var consultationNotes = new List<ConsultationNotes>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
        SELECT cn.*, u.FullName as DoctorName
        FROM TblConsultationNotes cn
        INNER JOIN TblDoctors d ON cn.DoctorId = d.DoctorId
        INNER JOIN TblUsers u ON d.UserId = u.UserId
        WHERE cn.PatientId = @PatientId
        ORDER BY cn.ConsultationDate DESC", connection);

            command.Parameters.AddWithValue("@PatientId", patientId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                consultationNotes.Add(new ConsultationNotes
                {
                    ConsultationId = reader.GetInt32("ConsultationId"),
                    AppointmentId = reader.GetInt32("AppointmentId"),
                    PatientId = reader.GetInt32("PatientId"),
                    DoctorId = reader.GetInt32("DoctorId"),
                    Symptoms = reader.GetString("Symptoms"),
                    Diagnosis = reader.GetString("Diagnosis"),
                    TreatmentPlan = reader.IsDBNull("TreatmentPlan") ? null : reader.GetString("TreatmentPlan"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
                    ConsultationDate = reader.GetDateTime("ConsultationDate")
                });
            }

            return consultationNotes;
        }

        // NEW METHODS TO IMPLEMENT THE INTERFACE:

        public async Task<List<Appointment>> GetDoctorAppointments(int doctorId)
        {
            var appointments = new List<Appointment>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                SELECT a.*, p.FirstName, p.LastName, p.Gender, p.DateOfBirth, p.Phone, p.Email
                FROM TblAppointments a
                INNER JOIN TblPatients p ON a.PatientId = p.PatientId
                WHERE a.DoctorId = @DoctorId
                ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC", connection);
            command.Parameters.AddWithValue("@DoctorId", doctorId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                appointments.Add(new Appointment
                {
                    AppointmentId = reader.GetInt32("AppointmentId"),
                    PatientId = reader.GetInt32("PatientId"),
                    AppointmentDate = reader.GetDateTime("AppointmentDate"),
                    AppointmentTime = reader.GetDateTime("AppointmentTime"),
                    Status = reader.GetString("Status"),
                    Reason = reader.GetString("Reason"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
                    Patient = new Patient
                    {
                        PatientId = reader.GetInt32("PatientId"),
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName"),
                        Gender = reader.GetString("Gender"),
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Phone = reader.GetString("Phone"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email")
                    }
                });
            }

            return appointments;
        }

        public async Task<List<Appointment>> GetDoctorAppointmentsByDate(int doctorId, DateTime startDate, DateTime endDate)
        {
            var appointments = new List<Appointment>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                SELECT a.*, p.FirstName, p.LastName, p.Gender, p.DateOfBirth, p.Phone, p.Email
                FROM TblAppointments a
                INNER JOIN TblPatients p ON a.PatientId = p.PatientId
                WHERE a.DoctorId = @DoctorId 
                AND a.AppointmentDate BETWEEN @StartDate AND @EndDate
                ORDER BY a.AppointmentDate, a.AppointmentTime", connection);
            command.Parameters.AddWithValue("@DoctorId", doctorId);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                appointments.Add(new Appointment
                {
                    AppointmentId = reader.GetInt32("AppointmentId"),
                    PatientId = reader.GetInt32("PatientId"),
                    AppointmentDate = reader.GetDateTime("AppointmentDate"),
                    AppointmentTime = reader.GetDateTime("AppointmentTime"),
                    Status = reader.GetString("Status"),
                    Reason = reader.GetString("Reason"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
                    Patient = new Patient
                    {
                        PatientId = reader.GetInt32("PatientId"),
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName"),
                        Gender = reader.GetString("Gender"),
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Phone = reader.GetString("Phone"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email")
                    }
                });
            }

            return appointments;
        }

        public async Task<List<Prescription>> GetDoctorPrescriptions(int doctorId)
        {
            var prescriptions = new List<Prescription>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                SELECT p.*, pat.FirstName, pat.LastName
                FROM TblPrescriptions p
                INNER JOIN TblPatients pat ON p.PatientId = pat.PatientId
                WHERE p.DoctorId = @DoctorId
                ORDER BY p.PrescriptionDate DESC", connection);
            command.Parameters.AddWithValue("@DoctorId", doctorId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                prescriptions.Add(new Prescription
                {
                    PrescriptionId = reader.GetInt32("PrescriptionId"),
                    AppointmentId = reader.GetInt32("AppointmentId"),
                    PatientId = reader.GetInt32("PatientId"),
                    DoctorId = reader.GetInt32("DoctorId"),
                    Diagnosis = reader.GetString("Diagnosis"),
                    PrescriptionDate = reader.GetDateTime("PrescriptionDate"),
                    Instructions = reader.IsDBNull("Instructions") ? null : reader.GetString("Instructions"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
                   
                });
            }

            return prescriptions;
        }

        public async Task<List<PrescriptionDetail>> GetPrescriptionDetails(int prescriptionId)
        {
            var details = new List<PrescriptionDetail>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                SELECT pd.*, m.MedicineName, m.Manufacturer
                FROM TblPrescriptionDetails pd
                INNER JOIN TblMedicines m ON pd.MedicineId = m.MedicineId
                WHERE pd.PrescriptionId = @PrescriptionId", connection);
            command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                details.Add(new PrescriptionDetail
                {
                    PrescriptionDetailId = reader.GetInt32("PrescriptionDetailId"),
                    PrescriptionId = reader.GetInt32("PrescriptionId"),
                    MedicineId = reader.GetInt32("MedicineId"),
                    Dosage = reader.GetString("Dosage"),
                    Frequency = reader.GetString("Frequency"),
                    Duration = reader.GetString("Duration"),
                    Instructions = reader.IsDBNull("Instructions") ? null : reader.GetString("Instructions"),
                    Medicine = new Medicine
                    {
                        MedicineId = reader.GetInt32("MedicineId"),
                        MedicineName = reader.GetString("MedicineName"),
                        Manufacturer = reader.IsDBNull("Manufacturer") ? null : reader.GetString("Manufacturer")
                    }
                });
            }

            return details;
        }

        public async Task<List<LabReport>> GetDoctorLabReports(int doctorId)
        {
            var labReports = new List<LabReport>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                SELECT lr.*, p.FirstName, p.LastName
                FROM TblLabReports lr
                INNER JOIN TblPatients p ON lr.PatientId = p.PatientId
                WHERE lr.DoctorId = @DoctorId
                ORDER BY lr.TestDate DESC", connection);
            command.Parameters.AddWithValue("@DoctorId", doctorId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                labReports.Add(new LabReport
                {
                    LabReportId = reader.GetInt32("LabReportId"),
                    PatientId = reader.GetInt32("PatientId"),
                    DoctorId = reader.GetInt32("DoctorId"),
                    TestName = reader.GetString("TestName"),
                    TestDate = reader.GetDateTime("TestDate"),
                    Result = reader.IsDBNull("Result") ? null : reader.GetString("Result"),
                    Status = reader.GetString("Status"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
                   
                });
            }

            return labReports;
        }

        public async Task<List<ConsultationNotes>> GetDoctorConsultationNotes(int doctorId)
        {
            var consultationNotes = new List<ConsultationNotes>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                SELECT cn.*, p.FirstName, p.LastName
                FROM TblConsultationNotes cn
                INNER JOIN TblPatients p ON cn.PatientId = p.PatientId
                WHERE cn.DoctorId = @DoctorId
                ORDER BY cn.ConsultationDate DESC", connection);

            command.Parameters.AddWithValue("@DoctorId", doctorId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                consultationNotes.Add(new ConsultationNotes
                {
                    ConsultationId = reader.GetInt32("ConsultationId"),
                    AppointmentId = reader.GetInt32("AppointmentId"),
                    PatientId = reader.GetInt32("PatientId"),
                    DoctorId = reader.GetInt32("DoctorId"),
                    Symptoms = reader.GetString("Symptoms"),
                    Diagnosis = reader.GetString("Diagnosis"),
                    TreatmentPlan = reader.IsDBNull("TreatmentPlan") ? null : reader.GetString("TreatmentPlan"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
                    ConsultationDate = reader.GetDateTime("ConsultationDate"),
                    
                });
            }

            return consultationNotes;
        }

        public async Task<PatientMedicalHistory> GetPatientMedicalHistory(int patientId)
        {
            var patient = await GetPatientById(patientId);
            var appointments = await GetDoctorAppointmentsByDate(0, DateTime.MinValue, DateTime.MaxValue);
            var prescriptions = await GetPatientPrescriptions(patientId);
            var labReports = await GetPatientLabReports(patientId);
            var consultationNotes = await GetPatientConsultationNotes(patientId);

            // Filter appointments by patient
            var patientAppointments = appointments.Where(a => a.PatientId == patientId).ToList();

            return new PatientMedicalHistory
            {
                Patient = patient,
                Appointments = patientAppointments,
                Prescriptions = prescriptions,
                LabReports = labReports,
                ConsultationNotes = consultationNotes
            };
        }

        public async Task<bool> UpdateAppointmentStatus(int appointmentId, string status)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                UPDATE TblAppointments 
                SET Status = @Status 
                WHERE AppointmentId = @AppointmentId", connection);
            command.Parameters.AddWithValue("@AppointmentId", appointmentId);
            command.Parameters.AddWithValue("@Status", status);

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<DoctorStatistics> GetDoctorStatistics(int doctorId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var statistics = new DoctorStatistics();

            // Get total patients
            using (var command = new SqlCommand(@"
                SELECT COUNT(DISTINCT PatientId) FROM TblAppointments WHERE DoctorId = @DoctorId", connection))
            {
                command.Parameters.AddWithValue("@DoctorId", doctorId);
                statistics.TotalPatients = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            // Get today's appointments
            using (var command = new SqlCommand(@"
                SELECT COUNT(*) FROM TblAppointments 
                WHERE DoctorId = @DoctorId AND CONVERT(DATE, AppointmentDate) = CONVERT(DATE, GETDATE())", connection))
            {
                command.Parameters.AddWithValue("@DoctorId", doctorId);
                statistics.TodayAppointments = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            // Get total prescriptions
            using (var command = new SqlCommand(@"
                SELECT COUNT(*) FROM TblPrescriptions WHERE DoctorId = @DoctorId", connection))
            {
                command.Parameters.AddWithValue("@DoctorId", doctorId);
                statistics.TotalPrescriptions = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            // Get pending lab reports
            using (var command = new SqlCommand(@"
                SELECT COUNT(*) FROM TblLabReports 
                WHERE DoctorId = @DoctorId AND Status IN ('Pending', 'Requested')", connection))
            {
                command.Parameters.AddWithValue("@DoctorId", doctorId);
                statistics.PendingLabReports = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            // Get completed appointments
            using (var command = new SqlCommand(@"
                SELECT COUNT(*) FROM TblAppointments 
                WHERE DoctorId = @DoctorId AND Status = 'Completed'", connection))
            {
                command.Parameters.AddWithValue("@DoctorId", doctorId);
                statistics.CompletedAppointments = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            return statistics;
        }
        public async Task<int> SendPrescriptionToPharmacy(int prescriptionId, int patientId, int doctorId, string notes)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_SendPrescriptionToPharmacy", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
                command.Parameters.AddWithValue("@PatientId", patientId);
                command.Parameters.AddWithValue("@DoctorId", doctorId);
                command.Parameters.AddWithValue("@Notes", notes ?? (object)DBNull.Value);

                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending prescription to pharmacy: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<PharmacyPrescriptionViewModel>> GetPharmacyPrescriptions(string status = null)
        {
            var prescriptions = new List<PharmacyPrescriptionViewModel>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_GetPharmacyPrescriptions", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var prescription = new PharmacyPrescriptionViewModel
                {
                    PharmacyPrescriptionId = reader.GetInt32("PharmacyPrescriptionId"),
                    PrescriptionId = reader.GetInt32("PrescriptionId"),
                    PatientId = reader.GetInt32("PatientId"),
                    PatientName = reader.GetString("PatientName"),
                    PatientPhone = reader.GetString("PatientPhone"),
                    DoctorName = reader.GetString("DoctorName"),
                    Diagnosis = reader.GetString("Diagnosis"),
                    Status = reader.GetString("Status"),
                    RequestDate = reader.GetDateTime("RequestDate"),
                    Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes")
                };

                if (!reader.IsDBNull("DispensedDate"))
                {
                    prescription.DispensedDate = reader.GetDateTime("DispensedDate");
                }

                prescriptions.Add(prescription);
            }

            return prescriptions;
        }
        public async Task<bool> UpdatePharmacyPrescriptionStatus(int pharmacyPrescriptionId, string status, int pharmacistId)
        {
            // Validate status
            var validStatuses = new[] { "Pending", "Processing", "Dispensed", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                throw new ArgumentException("Invalid status value. Must be: Pending, Processing, Dispensed, or Cancelled");
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_UpdatePharmacyPrescriptionStatus", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PharmacyPrescriptionId", pharmacyPrescriptionId);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@PharmacistId", pharmacistId);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error updating pharmacy prescription status: {sqlEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating pharmacy prescription status: {ex.Message}");
                return false;
            }
        }
    }
}