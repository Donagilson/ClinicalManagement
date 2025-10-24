using Microsoft.AspNetCore.Mvc;
using ClinicalManagementSystem2025.Models;
using Microsoft.AspNetCore.Http;
using ClinicalManagementSystem2025.Services;
using ClinicalManagementSystem2025.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicalManagementSystem2025.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // Temporary hardcoded values for testing - replace with actual authentication later
        private int GetDoctorId()
        {
            // For testing, return a valid doctor ID from your database
            // Check your TblDoctors table and use an existing DoctorId
            return 1; // Change this to match an actual doctor ID in your database
        }

        private string GetDoctorName()
        {
            // For testing, return a doctor name
            return "John Smith";
        }

        // GET: Doctor/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var doctorId = GetDoctorId();
                var appointments = await _doctorService.GetTodayAppointments(doctorId);

                ViewBag.DoctorName = GetDoctorName();
                ViewBag.Today = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
                ViewBag.DoctorId = doctorId;

                return View(appointments);
            }
            catch (Exception ex)
            {
                // If there's an error, show empty dashboard
                ViewBag.DoctorName = GetDoctorName();
                ViewBag.Today = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
                ViewBag.Error = "Unable to load appointments: " + ex.Message;

                return View(new List<Appointment>());
            }
        }

        // GET: Doctor/PatientDetails/5
        public async Task<IActionResult> PatientDetails(int patientId, int appointmentId)
        {
            try
            {
                var doctorId = GetDoctorId();
                var patient = await _doctorService.GetPatientDetails(patientId);

                if (patient == null)
                {
                    TempData["Error"] = "Patient not found.";
                    return RedirectToAction("Dashboard");
                }

                var prescriptions = await _doctorService.GetPatientPrescriptions(patientId);
                var labReports = await _doctorService.GetPatientLabReports(patientId);
                var medicines = await _doctorService.GetActiveMedicines();

                ViewBag.Prescriptions = prescriptions;
                ViewBag.LabReports = labReports;
                ViewBag.Medicines = medicines;
                ViewBag.AppointmentId = appointmentId;
                ViewBag.DoctorId = doctorId;
                ViewBag.DoctorName = GetDoctorName();

                return View(patient);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading patient details: " + ex.Message;
                return RedirectToAction("Dashboard");
            }
        }

        // POST: Doctor/SaveConsultation
        [HttpPost]
        public async Task<IActionResult> SaveConsultation(int appointmentId, int patientId, string symptoms, string diagnosis, string notes)
        {
            try
            {
                var doctorId = GetDoctorId();
                var result = await _doctorService.SaveConsultation(appointmentId, symptoms, diagnosis, notes, doctorId);

                if (result)
                {
                    return Json(new { success = true, message = "Consultation saved successfully." });
                }
                return Json(new { success = false, message = "Failed to save consultation." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: Doctor/PrescribeMedicines
        [HttpPost]
        public async Task<IActionResult> PrescribeMedicines(int appointmentId, int patientId, string diagnosis,
            List<int> medicineIds, List<string> dosages, List<string> frequencies, List<string> durations, List<string> instructions)
        {
            try
            {
                var doctorId = GetDoctorId();
                var prescription = new Prescription
                {
                    AppointmentId = appointmentId,
                    PatientId = patientId,
                    DoctorId = doctorId,
                    Diagnosis = diagnosis,
                    PrescriptionDate = DateTime.Now
                };

                var prescriptionDetails = new List<PrescriptionDetail>();
                for (int i = 0; i < medicineIds.Count; i++)
                {
                    prescriptionDetails.Add(new PrescriptionDetail
                    {
                        MedicineId = medicineIds[i],
                        Dosage = dosages[i],
                        Frequency = frequencies[i],
                        Duration = durations[i],
                        Instructions = instructions[i] ?? ""
                    });
                }

                var result = await _doctorService.PrescribeMedicines(prescription, prescriptionDetails);
                if (result)
                {
                    return Json(new { success = true, message = "Medicines prescribed successfully." });
                }
                return Json(new { success = false, message = "Failed to prescribe medicines." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: Doctor/RequestLabTest
        [HttpPost]
        public async Task<IActionResult> RequestLabTest(int patientId, string testName, string description, string notes)
        {
            try
            {
                var doctorId = GetDoctorId();
                var labReport = new LabReport
                {
                    PatientId = patientId,
                    DoctorId = doctorId,
                    TestName = testName,
                    TestDate = DateTime.Now,
                    Status = "Requested",
                    Notes = notes ?? ""
                };

                var result = await _doctorService.RequestLabTest(labReport);
                if (result)
                {
                    return Json(new { success = true, message = "Lab test requested successfully." });
                }
                return Json(new { success = false, message = "Failed to request lab test." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // GET: Doctor/TestData - For testing purposes
        public IActionResult TestData()
        {
            ViewBag.DoctorName = GetDoctorName();
            ViewBag.DoctorId = GetDoctorId();
            return View();
        }

        [HttpGet]
        public IActionResult CreatePrescription(int patientId, int appointmentId)
        {
            var viewModel = new PrescriptionViewModel
            {
                PatientId = patientId,
                AppointmentId = appointmentId
            };

            ViewBag.Medicines = _doctorService.GetActiveMedicines().Result; // Load medicines for dropdown
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrescription(PrescriptionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var doctorId = GetDoctorId();

                    // Convert ViewModel to Domain Model
                    var prescription = new Prescription
                    {
                        AppointmentId = viewModel.AppointmentId,
                        PatientId = viewModel.PatientId,
                        DoctorId = doctorId,
                        Diagnosis = viewModel.Diagnosis,
                        Instructions = viewModel.Instructions,
                        Notes = viewModel.Notes,
                        PrescriptionDate = DateTime.Now
                    };

                    // Convert Medicine items to PrescriptionDetails
                    var prescriptionDetails = new List<PrescriptionDetail>();
                    foreach (var medicine in viewModel.Medicines)
                    {
                        prescriptionDetails.Add(new PrescriptionDetail
                        {
                            MedicineId = medicine.MedicineId,
                            Dosage = medicine.Dosage,
                            Frequency = medicine.Frequency,
                            Duration = medicine.Duration,
                            Instructions = medicine.Instructions
                        });
                    }

                    // Save prescription
                    var result = await _doctorService.PrescribeMedicines(prescription, prescriptionDetails);
                    if (result)
                    {
                        // Get the last prescription ID (you might need to modify your service to return the ID)
                        var prescriptions = await _doctorService.GetPatientPrescriptions(viewModel.PatientId);
                        var latestPrescription = prescriptions.OrderByDescending(p => p.PrescriptionDate).FirstOrDefault();

                        if (latestPrescription != null)
                        {
                            // Send to pharmacy
                            var pharmacyResult = await _doctorService.SendPrescriptionToPharmacy(
                                latestPrescription.PrescriptionId,
                                viewModel.PatientId,
                                doctorId,
                                "Auto-generated from doctor prescription"
                            );

                            if (pharmacyResult > 0)
                            {
                                TempData["Success"] = "Prescription created and sent to pharmacy successfully!";
                            }
                            else
                            {
                                TempData["Success"] = "Prescription created but failed to send to pharmacy!";
                            }
                        }
                        else
                        {
                            TempData["Success"] = "Prescription created successfully!";
                        }

                        return RedirectToAction("PatientDetails", new { patientId = viewModel.PatientId, appointmentId = viewModel.AppointmentId });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to save prescription to database.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating prescription: " + ex.Message);
                }
            }

            ViewBag.Medicines = await _doctorService.GetActiveMedicines();
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult RequestLabTest(int patientId, int appointmentId)
        {
            var viewModel = new LabRequestViewModel
            {
                PatientId = patientId,
                AppointmentId = appointmentId
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RequestLabTest(LabRequestViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var doctorId = GetDoctorId();

                    var labReport = new LabReport
                    {
                        PatientId = viewModel.PatientId,
                        DoctorId = doctorId,
                        TestName = viewModel.TestName,
                        TestDate = DateTime.Now,
                        Status = "Requested",
                        Notes = viewModel.Notes,
                        Result = viewModel.Description // Using Result field for description
                    };

                    var result = await _doctorService.RequestLabTest(labReport);
                    if (result)
                    {
                        TempData["Success"] = "Lab test requested successfully!";
                        return RedirectToAction("PatientDetails", new { patientId = viewModel.PatientId, appointmentId = viewModel.AppointmentId });
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error requesting lab test: " + ex.Message);
                }
            }
            return View(viewModel);
        }

        // GET: Doctor/Patients - Show doctor's patients
        public async Task<IActionResult> Patients()
        {
            try
            {
                var doctorId = GetDoctorId();
                var patients = await _doctorService.GetDoctorPatients(doctorId);

                ViewBag.DoctorName = GetDoctorName();
                return View(patients);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading patients: " + ex.Message;
                return View(new List<Patient>());
            }
        }

        // GET: Doctor/Appointments - Show doctor's appointments
        public async Task<IActionResult> Appointments()
        {
            try
            {
                var doctorId = GetDoctorId();
                var appointments = await _doctorService.GetTodayAppointments(doctorId);

                ViewBag.DoctorName = GetDoctorName();
                ViewBag.Today = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
                return View(appointments);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading appointments: " + ex.Message;
                return View(new List<Appointment>());
            }
        }

        // GET: Doctor/Prescriptions - Show doctor's prescriptions
        public async Task<IActionResult> Prescriptions()
        {
            try
            {
                var doctorId = GetDoctorId();

                // var prescriptions = await _doctorService.GetDoctorPrescriptions(doctorId);

                ViewBag.DoctorName = GetDoctorName();
                return View(new List<Prescription>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading prescriptions: " + ex.Message;
                return View(new List<Prescription>());
            }
        }

        // GET: Doctor/LabReports - Show doctor's lab reports
        public async Task<IActionResult> LabReports()
        {
            try
            {
                var doctorId = GetDoctorId();

                // var labReports = await _doctorService.GetDoctorLabReports(doctorId);

                ViewBag.DoctorName = GetDoctorName();
                return View(new List<LabReport>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading lab reports: " + ex.Message;
                return View(new List<LabReport>());
            }
        }

        // Update SaveConsultation to use ConsultationNotes model
        [HttpPost]
        public async Task<IActionResult> SaveConsultation(ConsultationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var doctorId = GetDoctorId();

                    var consultationNotes = new ConsultationNotes
                    {
                        AppointmentId = viewModel.AppointmentId,
                        PatientId = viewModel.PatientId,
                        DoctorId = doctorId,
                        Symptoms = viewModel.Symptoms,
                        Diagnosis = viewModel.Diagnosis,
                        TreatmentPlan = viewModel.TreatmentPlan,
                        Notes = viewModel.Notes,
                        ConsultationDate = DateTime.Now
                    };

                    var result = await _doctorService.SaveConsultationNotes(consultationNotes);
                    if (result)
                    {
                        return Json(new { success = true, message = "Consultation notes saved successfully." });
                    }
                    return Json(new { success = false, message = "Failed to save consultation notes." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error: " + ex.Message });
                }
            }

            // Return validation errors
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Validation failed", errors = errors });
        }

        // GET: Doctor/AllPatients - Show all patients with medical history
        public async Task<IActionResult> AllPatients()
        {
            try
            {
                var doctorId = GetDoctorId();
                var patients = await _doctorService.GetDoctorPatients(doctorId);

                // Get medical history for each patient
                var patientsWithHistory = new List<PatientMedicalHistoryViewModel>();
                foreach (var patient in patients)
                {
                    var prescriptions = await _doctorService.GetPatientPrescriptions(patient.PatientId);
                    var labReports = await _doctorService.GetPatientLabReports(patient.PatientId);
                    var consultationNotes = await _doctorService.GetPatientConsultationNotes(patient.PatientId);

                    patientsWithHistory.Add(new PatientMedicalHistoryViewModel
                    {
                        Patient = patient,
                        Prescriptions = prescriptions,
                        LabReports = labReports,
                        ConsultationNotes = consultationNotes,
                        LastVisit = await GetPatientLastConsultation(patient.PatientId, doctorId)
                    });
                }

                ViewBag.DoctorName = GetDoctorName();
                return View(patientsWithHistory);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading patients: " + ex.Message;
                return View(new List<PatientMedicalHistoryViewModel>());
            }
        }

        // GET: Doctor/AllAppointments - Show all appointments with status
        public async Task<IActionResult> AllAppointments()
        {
            try
            {
                var doctorId = GetDoctorId();

                // Get appointments from last 30 days
                var startDate = DateTime.Now.AddDays(-30);
                var endDate = DateTime.Now.AddDays(30);

                var appointments = await GetAppointmentsByDateRange(doctorId, startDate, endDate);

                // Group by status for statistics
                var statusStats = appointments
                    .GroupBy(a => a.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionary(x => x.Status, x => x.Count);

                ViewBag.StatusStats = statusStats;
                ViewBag.DoctorName = GetDoctorName();
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                return View(appointments);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading appointments: " + ex.Message;
                return View(new List<Appointment>());
            }
        }

        // GET: Doctor/AllPrescriptions - Show all prescriptions
        public async Task<IActionResult> AllPrescriptions()
        {
            try
            {
                var doctorId = GetDoctorId();

                // Get all patients first
                var patients = await _doctorService.GetDoctorPatients(doctorId);
                var allPrescriptions = new List<Prescription>();

                // Get prescriptions for each patient
                foreach (var patient in patients)
                {
                    var patientPrescriptions = await _doctorService.GetPatientPrescriptions(patient.PatientId);
                    allPrescriptions.AddRange(patientPrescriptions);
                }

                // Order by date
                allPrescriptions = allPrescriptions
                    .OrderByDescending(p => p.PrescriptionDate)
                    .ToList();

                ViewBag.DoctorName = GetDoctorName();
                ViewBag.TotalPrescriptions = allPrescriptions.Count;
                return View(allPrescriptions);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading prescriptions: " + ex.Message;
                return View(new List<Prescription>());
            }
        }

        // GET: Doctor/AllLabReports - Show all lab reports
        public async Task<IActionResult> AllLabReports()
        {
            try
            {
                var doctorId = GetDoctorId();

                // Get all patients first
                var patients = await _doctorService.GetDoctorPatients(doctorId);
                var allLabReports = new List<LabReport>();

                // Get lab reports for each patient
                foreach (var patient in patients)
                {
                    var patientLabReports = await _doctorService.GetPatientLabReports(patient.PatientId);
                    allLabReports.AddRange(patientLabReports);
                }

                // Order by date and group by status
                allLabReports = allLabReports
                    .OrderByDescending(l => l.TestDate)
                    .ToList();

                var statusStats = allLabReports
                    .GroupBy(l => l.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionary(x => x.Status, x => x.Count);

                ViewBag.StatusStats = statusStats;
                ViewBag.DoctorName = GetDoctorName();
                ViewBag.TotalReports = allLabReports.Count;

                return View(allLabReports);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading lab reports: " + ex.Message;
                return View(new List<LabReport>());
            }
        }

        // Helper method to get appointments by date range
        private async Task<List<Appointment>> GetAppointmentsByDateRange(int doctorId, DateTime startDate, DateTime endDate)
        {
            try
            {
                // This is a simplified implementation
                // You'll need to create a proper repository method for this
                var allAppointments = await _doctorService.GetTodayAppointments(doctorId);
                return allAppointments
                    .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenByDescending(a => a.AppointmentTime)
                    .ToList();
            }
            catch
            {
                return new List<Appointment>();
            }
        }

        // Helper method to get patient's last consultation
        private async Task<DateTime?> GetPatientLastConsultation(int patientId, int doctorId)
        {
            try
            {
                // Get the most recent completed appointment for this patient with this doctor
                var appointments = await _doctorService.GetTodayAppointments(doctorId);
                var lastConsultation = appointments
                    .Where(a => a.PatientId == patientId && a.Status == "Completed")
                    .OrderByDescending(a => a.AppointmentDate)
                    .FirstOrDefault();

                return lastConsultation?.AppointmentDate;
            }
            catch
            {
                return null;
            }
        }
    }
}