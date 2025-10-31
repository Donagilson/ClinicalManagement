// Controllers/DoctorController.cs
using Microsoft.AspNetCore.Mvc;
using ClinicalManagementSystem2025.Repository;
using ClinicalManagementSystem2025.Models;
using System.Text.RegularExpressions;
using System.Linq;

namespace ClinicalManagementSystem2025.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMedicalNoteRepository _medicalNoteRepository;
        private readonly ILabRepository _labRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IMedicineRepository _medicineRepository;

        public DoctorController(IAppointmentRepository appointmentRepository, IDoctorRepository doctorRepository, IPatientRepository patientRepository, IMedicalNoteRepository medicalNoteRepository, ILabRepository labRepository, IPrescriptionRepository prescriptionRepository, IMedicineRepository medicineRepository)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _medicalNoteRepository = medicalNoteRepository;
            _labRepository = labRepository;
            _prescriptionRepository = prescriptionRepository;
            _medicineRepository = medicineRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Doctor Dashboard";
            ViewBag.Role = "Doctor";
            ViewBag.UserName = Request.Cookies["FullName"];

            // Get current doctor's ID from cookies
            var userIdCookie = Request.Cookies["UserId"];
            if (int.TryParse(userIdCookie, out int userId))
            {
                // Get doctor record by UserId
                var doctor = await GetDoctorByUserIdAsync(userId);
                if (doctor != null)
                {
                    // Get today's appointments for this doctor
                    var todaysAppointments = await _appointmentRepository.GetDoctorAppointmentsForDateAsync(doctor.DoctorId, DateTime.Today);

                    // Get tomorrow's appointments for this doctor
                    var tomorrowsAppointments = await _appointmentRepository.GetDoctorAppointmentsForDateAsync(doctor.DoctorId, DateTime.Today.AddDays(1));

                    ViewBag.TodaysAppointments = todaysAppointments;
                    ViewBag.TomorrowsAppointments = tomorrowsAppointments;
                    ViewBag.DoctorInfo = doctor;
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AppointmentsByDate(DateTime date)
        {
            var userIdCookie = Request.Cookies["UserId"];
            if (!int.TryParse(userIdCookie, out int userId))
            {
                TempData["ErrorMessage"] = "Please log in to view appointments.";
                return RedirectToAction("Login", "Login");
            }

            var doctor = await GetDoctorByUserIdAsync(userId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor profile not found.";
                return RedirectToAction("Index");
            }

            var appointments = await _appointmentRepository.GetDoctorAppointmentsForDateAsync(doctor.DoctorId, date.Date);

            ViewBag.DoctorInfo = doctor;
            ViewBag.AppointmentsDate = date.ToString("dddd, MMMM dd, yyyy");
            ViewBag.Appointments = appointments;

            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> PatientDetails(int patientId)
        {
            var userIdCookie = Request.Cookies["UserId"];
            if (!int.TryParse(userIdCookie, out int userId))
            {
                TempData["ErrorMessage"] = "Please log in to view patient details.";
                return RedirectToAction("Login", "Login");
            }

            var doctor = await GetDoctorByUserIdAsync(userId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor profile not found.";
                return RedirectToAction("Index");
            }

            // Get patient details from repository
            var patient = await _patientRepository.GetPatientByIdAsync(patientId);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction("Index");
            }

            // Get today's appointments for this patient and doctor to update status
            var todaysAppointments = await _appointmentRepository.GetDoctorAppointmentsForDateAsync(doctor.DoctorId, DateTime.Today);
            var currentAppointment = todaysAppointments.FirstOrDefault(a =>
                a.PatientId == patientId && (a.Status == "Scheduled" || a.Status == "Confirmed" || a.Status == "InProgress"));

            if (currentAppointment != null)
            {
                // Update appointment status to InProgress when doctor starts viewing patient
                currentAppointment.Status = "InProgress";
                await _appointmentRepository.UpdateAppointmentAsync(currentAppointment);
                ViewBag.CurrentAppointment = currentAppointment;
            }

            ViewBag.PatientId = patientId;
            ViewBag.PatientInfo = patient;
            ViewBag.DoctorInfo = doctor;
            ViewBag.MedicalNotes = await _medicalNoteRepository.GetMedicalNotesByPatientIdAsync(patientId);

            // Get lab test prescriptions for this patient
            var labTestPrescriptions = await _labRepository.GetLabTestPrescriptionsByPatientAsync(patientId);
            ViewBag.LabTestPrescriptions = labTestPrescriptions;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddMedicalNote(int patientId, int? appointmentId = null)
        {
            var userIdCookie = Request.Cookies["UserId"];
            if (!int.TryParse(userIdCookie, out int userId))
            {
                TempData["ErrorMessage"] = "Please log in to add medical notes.";
                return RedirectToAction("Login", "Login");
            }

            var doctor = await GetDoctorByUserIdAsync(userId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor profile not found.";
                return RedirectToAction("Index");
            }

            var medicalNote = new MedicalNote
            {
                PatientId = patientId,
                DoctorId = doctor.DoctorId,
                AppointmentId = appointmentId,
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            };

            // Get patient details
            var patient = await _patientRepository.GetPatientByIdAsync(medicalNote.PatientId);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction("Index");
            }

            ViewBag.PatientId = medicalNote.PatientId;
            ViewBag.PatientInfo = patient;
            ViewBag.DoctorInfo = doctor;
            ViewBag.AppointmentId = medicalNote.AppointmentId;

            return View(medicalNote);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedicalNote(MedicalNote medicalNote)
        {
            var userIdCookie = Request.Cookies["UserId"];
            if (!int.TryParse(userIdCookie, out int userId))
            {
                TempData["ErrorMessage"] = "Please log in to add medical notes.";
                return RedirectToAction("Login", "Login");
            }

            var doctor = await GetDoctorByUserIdAsync(userId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor profile not found.";
                return RedirectToAction("Index");
            }

            // Remove validation for navigation properties
            ModelState.Remove("Patient");
            ModelState.Remove("Doctor");
            ModelState.Remove("Appointment");

            if (ModelState.IsValid)
            {
                try
                {
                    // Set audit fields
                    medicalNote.DoctorId = doctor.DoctorId;
                    medicalNote.CreatedBy = userId;
                    medicalNote.CreatedDate = DateTime.Now;
                    medicalNote.IsActive = true;

                    // Save to database
                    var medicalNoteId = await _medicalNoteRepository.AddMedicalNoteAsync(medicalNote);

                    if (medicalNoteId > 0)
                    {
                        // Update appointment status to "Visited" if appointment exists and medical note was created
                        if (medicalNote.AppointmentId.HasValue && medicalNote.AppointmentId.Value > 0)
                        {
                            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(medicalNote.AppointmentId.Value);
                            if (appointment != null)
                            {
                                // Check if medicine is prescribed (prescription contains actual medicine data)
                                bool hasPrescription = !string.IsNullOrWhiteSpace(medicalNote.Prescription) &&
                                                     medicalNote.Prescription.Trim() != "None" &&
                                                     medicalNote.Prescription.Trim() != "No prescription needed" &&
                                                     medicalNote.Prescription.Trim() != "";

                                if (hasPrescription)
                                {
                                    // Patient was visited and medicine was prescribed - mark as visited
                                    appointment.Status = "Visited";
                                    await _appointmentRepository.UpdateAppointmentAsync(appointment);
                                }
                                else
                                {
                                    // Patient was visited but no medicine prescribed - mark as completed
                                    appointment.Status = "Completed";
                                    await _appointmentRepository.UpdateAppointmentAsync(appointment);
                                }
                            }
                        }

                        // Create prescription if prescription data is provided
                        if (!string.IsNullOrWhiteSpace(medicalNote.Prescription))
                        {
                            await CreatePrescriptionFromMedicalNote(medicalNote, medicalNoteId);
                        }

                        // Create lab test prescriptions if lab tests are specified
                        if (!string.IsNullOrWhiteSpace(medicalNote.LabTests))
                        {
                            await CreateLabTestPrescriptionsFromMedicalNote(medicalNote, medicalNoteId);
                        }

                        TempData["SuccessMessage"] = $"Medical note added successfully! Note ID: MN{medicalNoteId:0000}";
                        return RedirectToAction("PatientDetails", new { patientId = medicalNote.PatientId });
                    }
                    else
                    {
                        throw new Exception("Failed to save medical note to database.");
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error adding medical note: {ex.Message}";
                }
            }

            ViewBag.PatientId = medicalNote.PatientId;
            ViewBag.DoctorInfo = doctor;
            ViewBag.AppointmentId = medicalNote.AppointmentId;

            return View(medicalNote);
        }

        private async Task<Doctor?> GetDoctorByUserIdAsync(int userId)
        {
            var allDoctors = await _doctorRepository.GetAllDoctorsWithDetailsAsync();
            return allDoctors.ToList().FirstOrDefault(d => d.UserId == userId);
        }

        private async Task CreatePrescriptionFromMedicalNote(MedicalNote medicalNote, int medicalNoteId)
        {
            try
            {
                // Check if Prescription is not null or empty
                if (string.IsNullOrWhiteSpace(medicalNote.Prescription))
                {
                    return; // No prescription to process
                }

                // Create prescription record
                var prescription = new Prescription
                {
                    PatientId = medicalNote.PatientId,
                    DoctorId = medicalNote.DoctorId,
                    AppointmentId = medicalNote.AppointmentId ?? 0,
                    Diagnosis = medicalNote.Diagnosis,
                    PrescriptionDate = DateTime.Now,
                    Notes = $"Generated from Medical Note MN{medicalNoteId:0000}"
                };

                var prescriptionId = await _prescriptionRepository.AddPrescriptionAsync(prescription);

                if (prescriptionId != null && prescriptionId.PrescriptionId > 0)
                {
                    // Parse prescription details with intelligent medicine lookup
                    var prescriptionLines = medicalNote.Prescription.Split('\n')
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .Select(line => line.Trim())
                        .ToList();

                    // Get all available medicines for lookup
                    var availableMedicines = await GetAllMedicinesForLookupAsync();

                    foreach (var line in prescriptionLines)
                    {
                        await CreatePrescriptionDetailFromLine(prescriptionId.PrescriptionId, line, availableMedicines);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the medical note creation
                TempData["WarningMessage"] = $"Medical note saved but prescription creation failed: {ex.Message}";
            }
        }

        private async Task<List<Medicine>> GetAllMedicinesForLookupAsync()
        {
            try
            {
                return (await _medicineRepository.GetAllMedicinesAsync()).ToList();
            }
            catch
            {
                return new List<Medicine>();
            }
        }

        private async Task CreatePrescriptionDetailFromLine(int prescriptionId, string prescriptionLine, List<Medicine> availableMedicines)
        {
            try
            {
                // Parse medicine information from the line
                var medicineInfo = ParseMedicineInfo(prescriptionLine);

                // Find matching medicine in database
                var medicine = FindMedicine(medicineInfo.Name, availableMedicines);

                // Extract dosage information
                var dosageInfo = ParseDosageInfo(prescriptionLine);

                // Create prescription detail
                var prescriptionDetail = new PrescriptionDetail
                {
                    PrescriptionId = prescriptionId,
                    MedicineId = medicine?.MedicineId ?? 1, // Fallback to default if not found
                    MedicineName = medicine?.MedicineName ?? medicineInfo.Name, // Store the actual name
                    Dosage = dosageInfo.Dosage ?? "As prescribed",
                    Frequency = dosageInfo.Frequency ?? "As directed",
                    Duration = dosageInfo.Duration ?? "As needed",
                    Instructions = prescriptionLine,
                    Quantity = dosageInfo.Quantity > 0 ? dosageInfo.Quantity : 1,
                    Price = medicine?.UnitPrice ?? 0
                };

                await _prescriptionRepository.AddPrescriptionDetailAsync(prescriptionDetail);
            }
            catch (Exception ex)
            {
                // Log error but continue with other medicines
                Console.WriteLine($"Error creating prescription detail for line '{prescriptionLine}': {ex.Message}");
            }
        }

        private (string Name, int Quantity) ParseMedicineInfo(string line)
        {
            // Common patterns for medicine prescriptions
            var patterns = new[]
            {
                @"^(.+?)\s+(\d+)\s*(?:tablet|capsule|ml|mg|g|units?)",
                @"^(.+?)\s*-\s*(\d+)",
                @"^(\d+)\s*(?:tablet|capsule|ml|mg|g|units?)\s+of\s+(.+)",
                @"^(.+?)\s*$" // Fallback - just the medicine name
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(line, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string medicineName = match.Groups[1].Value.Trim();
                    string quantityStr = match.Groups[2].Value;

                    if (int.TryParse(quantityStr, out int quantity) && quantity > 0)
                    {
                        return (medicineName, quantity);
                    }
                    else
                    {
                        return (medicineName, 1);
                    }
                }
            }

            return (line.Trim(), 1);
        }

        private (string Dosage, string Frequency, string Duration, int Quantity) ParseDosageInfo(string line)
        {
            // Parse dosage patterns like "1 tablet twice daily for 7 days"
            var dosage = "As prescribed";
            var frequency = "As directed";
            var duration = "As needed";
            var quantity = 1;

            // Look for common frequency patterns
            if (Regex.IsMatch(line, @"\b(?:twice|two times|2 times?)\s+daily\b", RegexOptions.IgnoreCase))
                frequency = "Twice daily";
            else if (Regex.IsMatch(line, @"\b(?:thrice|three times|3 times?)\s+daily\b", RegexOptions.IgnoreCase))
                frequency = "Thrice daily";
            else if (Regex.IsMatch(line, @"\b(?:once|one time|1 time?)\s+daily\b", RegexOptions.IgnoreCase))
                frequency = "Once daily";
            else if (Regex.IsMatch(line, @"\b(?:every|each)\s+\d+\s+hours?\b", RegexOptions.IgnoreCase))
                frequency = "Every 8 hours"; // Default, could be more specific

            // Look for duration patterns
            var durationMatch = Regex.Match(line, @"for\s+(\d+)\s+days?", RegexOptions.IgnoreCase);
            if (durationMatch.Success && int.TryParse(durationMatch.Groups[1].Value, out int days))
            {
                duration = $"{days} days";
                quantity = Math.Max(1, days); // Calculate quantity based on duration
            }

            // Look for dosage patterns
            var doseMatch = Regex.Match(line, @"(\d+)\s*(?:tablet|capsule|ml|mg)", RegexOptions.IgnoreCase);
            if (doseMatch.Success)
            {
                dosage = doseMatch.Value.Trim();
            }

            return (dosage, frequency, duration, quantity);
        }

        private Medicine? FindMedicine(string medicineName, List<Medicine> availableMedicines)
        {
            if (string.IsNullOrWhiteSpace(medicineName) || availableMedicines == null || !availableMedicines.Any())
                return null;

            // Exact match first
            var exactMatch = availableMedicines.FirstOrDefault(m =>
                m.MedicineName.Equals(medicineName, StringComparison.OrdinalIgnoreCase));
            if (exactMatch != null)
                return exactMatch;

            // Partial match
            var partialMatch = availableMedicines.FirstOrDefault(m =>
                m.MedicineName.Contains(medicineName, StringComparison.OrdinalIgnoreCase) ||
                medicineName.Contains(m.MedicineName, StringComparison.OrdinalIgnoreCase));
            if (partialMatch != null)
                return partialMatch;

            return null;
        }

        private async Task CreateLabTestPrescriptionsFromMedicalNote(MedicalNote medicalNote, int medicalNoteId)
        {
            try
            {
                // Check if LabTests is not null or empty
                if (string.IsNullOrWhiteSpace(medicalNote.LabTests))
                {
                    return; // No lab tests to process
                }

                // Parse lab tests (basic implementation - you may want to enhance this)
                var labTestNames = medicalNote.LabTests.Split('\n')
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Trim())
                    .ToList();

                // Get available lab tests
                var availableLabTests = await _labRepository.GetAllLabTestsAsync();

                foreach (var testName in labTestNames)
                {
                    // Find matching lab test (basic matching - you may want to enhance this)
                    var labTest = availableLabTests?.FirstOrDefault(lt =>
                        lt.TestName != null && (lt.TestName.Contains(testName, StringComparison.OrdinalIgnoreCase) ||
                        testName.Contains(lt.TestName, StringComparison.OrdinalIgnoreCase)));

                    if (labTest != null)
                    {
                        // Create lab test prescription instead of direct lab report
                        var prescription = new LabTestPrescription
                        {
                            AppointmentId = medicalNote.AppointmentId ?? 0,
                            PatientId = medicalNote.PatientId,
                            DoctorId = medicalNote.DoctorId,
                            LabTestId = labTest.LabTestId,
                            PrescriptionDate = DateTime.Now,
                            ClinicalNotes = $"Prescribed in Medical Note MN{medicalNoteId:0000}. Original: {testName}",
                            Priority = "Normal",
                            Status = "Prescribed",
                            Notes = $"Auto-generated from Medical Note MN{medicalNoteId:0000}",
                            CreatedDate = DateTime.Now
                        };

                        await _labRepository.AddLabTestPrescriptionAsync(prescription);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the medical note creation
                TempData["WarningMessage"] = $"Medical note saved but lab test prescription creation failed: {ex.Message}";
            }
        }

        // GET: Doctor/LabTests - View available lab tests for prescription
        [HttpGet]
        public async Task<IActionResult> LabTests()
        {
            var userIdCookie = Request.Cookies["UserId"];
            if (!int.TryParse(userIdCookie, out int userId))
            {
                TempData["ErrorMessage"] = "Please log in to view lab tests.";
                return RedirectToAction("Login", "Login");
            }

            var doctor = await GetDoctorByUserIdAsync(userId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor profile not found.";
                return RedirectToAction("Index");
            }

            var labTests = await _labRepository.GetAllLabTestsAsync();
            ViewBag.DoctorInfo = doctor;

            return View(labTests);
        }

        // GET: Doctor/PrescribeLabTest - Prescribe lab test for patient
        [HttpGet]
        public async Task<IActionResult> PrescribeLabTest(int patientId, int? appointmentId = null)
        {
            var userIdCookie = Request.Cookies["UserId"];
            if (!int.TryParse(userIdCookie, out int userId))
            {
                TempData["ErrorMessage"] = "Please log in to prescribe lab tests.";
                return RedirectToAction("Login", "Login");
            }

            var doctor = await GetDoctorByUserIdAsync(userId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor profile not found.";
                return RedirectToAction("Index");
            }

            var patient = await _patientRepository.GetPatientByIdAsync(patientId);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction("Index");
            }

            var labTests = await _labRepository.GetAllLabTestsAsync();

            ViewBag.PatientId = patientId;
            ViewBag.PatientInfo = patient;
            ViewBag.DoctorInfo = doctor;
            ViewBag.AppointmentId = appointmentId;
            ViewBag.LabTests = labTests;

            return View();
        }

        // POST: Doctor/PrescribeLabTest - Save lab test prescription
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrescribeLabTest(int patientId, int labTestId, string clinicalNotes, string priority, int? appointmentId)
        {
            var userIdCookie = Request.Cookies["UserId"];
            if (!int.TryParse(userIdCookie, out int userId))
            {
                TempData["ErrorMessage"] = "Please log in to prescribe lab tests.";
                return RedirectToAction("Login", "Login");
            }

            var doctor = await GetDoctorByUserIdAsync(userId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor profile not found.";
                return RedirectToAction("Index");
            }

            if (labTestId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a valid lab test.";
                return RedirectToAction("PrescribeLabTest", new { patientId, appointmentId });
            }

            try
            {
                var prescription = new LabTestPrescription
                {
                    AppointmentId = appointmentId ?? 0,
                    PatientId = patientId,
                    DoctorId = doctor.DoctorId,
                    LabTestId = labTestId,
                    PrescriptionDate = DateTime.Now,
                    ClinicalNotes = clinicalNotes,
                    Priority = priority ?? "Normal",
                    Status = "Prescribed",
                    CreatedDate = DateTime.Now
                };

                await _labRepository.AddLabTestPrescriptionAsync(prescription);

                TempData["SuccessMessage"] = "Lab test prescribed successfully.";
                return RedirectToAction("PatientDetails", new { patientId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error prescribing lab test: {ex.Message}";
                return RedirectToAction("PrescribeLabTest", new { patientId, appointmentId });
            }
        }

        // GET: Doctor/MyLabTestPrescriptions - View doctor's prescribed lab tests
        [HttpGet]
        public async Task<IActionResult> MyLabTestPrescriptions()
        {
            var userIdCookie = Request.Cookies["UserId"];
            if (!int.TryParse(userIdCookie, out int userId))
            {
                TempData["ErrorMessage"] = "Please log in to view lab test prescriptions.";
                return RedirectToAction("Login", "Login");
            }

            var doctor = await GetDoctorByUserIdAsync(userId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor profile not found.";
                return RedirectToAction("Index");
            }

            var prescriptions = await _labRepository.GetLabTestPrescriptionsByDoctorAsync(doctor.DoctorId);
            ViewBag.DoctorInfo = doctor;

            return View(prescriptions);
        }

        // GET: Doctor/LabReports - View doctor's lab reports
        [HttpGet]
        public async Task<IActionResult> LabReports()
        {
            var userIdCookie = Request.Cookies["UserId"];
            if (!int.TryParse(userIdCookie, out int userId))
            {
                TempData["ErrorMessage"] = "Please log in to view lab reports.";
                return RedirectToAction("Login", "Login");
            }

            var doctor = await GetDoctorByUserIdAsync(userId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor profile not found.";
                return RedirectToAction("Index");
            }

            var labReports = await _labRepository.GetLabReportsByDoctorAsync(doctor.DoctorId);
            ViewBag.DoctorInfo = doctor;

            return View(labReports);
        }

        // GET: Doctor/LabReportDetails - View specific lab report
        [HttpGet]
        public async Task<IActionResult> LabReportDetails(int id)
        {
            var userIdCookie = Request.Cookies["UserId"];
            if (!int.TryParse(userIdCookie, out int userId))
            {
                TempData["ErrorMessage"] = "Please log in to view lab report details.";
                return RedirectToAction("Login", "Login");
            }

            var doctor = await GetDoctorByUserIdAsync(userId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor profile not found.";
                return RedirectToAction("Index");
            }

            var labReport = await _labRepository.GetLabReportByIdAsync(id);
            if (labReport == null)
            {
                TempData["ErrorMessage"] = "Lab report not found.";
                return RedirectToAction("LabReports");
            }

            // Ensure doctor can only view their own reports
            if (labReport.DoctorId != doctor.DoctorId)
            {
                TempData["ErrorMessage"] = "You don't have permission to view this lab report.";
                return RedirectToAction("LabReports");
            }

            // Get additional details
            var labTest = await _labRepository.GetLabTestByIdAsync(labReport.LabTestId);
            var patient = await _patientRepository.GetPatientByIdAsync(labReport.PatientId);

            ViewBag.DoctorInfo = doctor;
            ViewBag.LabTest = labTest;
            ViewBag.Patient = patient;

            return View(labReport);
        }
    }
}