using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Repository;
using ClinicalManagementSystem2025.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalManagementSystem2025.Controllers
{
    public class ReceptionistController : Controller
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ILogger<ReceptionistController> _logger;

        public ReceptionistController(
            IPatientRepository patientRepository,
            IAppointmentService appointmentService,
            IDoctorRepository doctorRepository,
            ILogger<ReceptionistController> logger)
        {
            _patientRepository = patientRepository;
            _appointmentService = appointmentService;
            _doctorRepository = doctorRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var todaysAppointments = await _appointmentService.GetTodaysAppointmentsAsync();
                ViewBag.TodaysAppointments = todaysAppointments;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading today's appointments");
                TempData["ErrorMessage"] = "Error loading appointments. Please try again.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult AddPatient(string? phone = null)
        {
            var patient = new Patient();
            if (!string.IsNullOrEmpty(phone))
            {
                patient.Phone = phone;
            }
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPatient(Patient patient)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(patient.FirstName))
                {
                    ModelState.AddModelError("FirstName", "First Name is required");
                }
                if (string.IsNullOrWhiteSpace(patient.LastName))
                {
                    ModelState.AddModelError("LastName", "Last Name is required");
                }
                if (string.IsNullOrWhiteSpace(patient.Phone))
                {
                    ModelState.AddModelError("Phone", "Phone Number is required");
                }
                if (string.IsNullOrWhiteSpace(patient.Gender))
                {
                    ModelState.AddModelError("Gender", "Gender is required");
                }
                if (patient.DateOfBirth == default || patient.DateOfBirth > DateTime.Now)
                {
                    ModelState.AddModelError("DateOfBirth", "Please select a valid Date of Birth");
                }

                if (!ModelState.IsValid)
                {
                    return View(patient);
                }

                // Note: Allowing multiple patients with same phone number as per requirements

                // Set registration date
                patient.RegistrationDate = DateTime.Now;
                patient.IsActive = true;

                // Add to database
                var patientId = await _patientRepository.AddPatientAsync(patient);

                if (patientId > 0)
                {
                    TempData["SuccessMessage"] = $"Patient {patient.FirstName} {patient.LastName} added successfully!";
                    return RedirectToAction("PatientDetails", new { id = patientId });
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add patient. Please try again.";
                    return View(patient);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding patient");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(patient);
            }
        }

        // Get availability slots for a doctor on a given date
        [HttpGet]
        public async Task<IActionResult> GetAvailabilitySlots(int doctorId, DateTime date, int durationMinutes = 30)
        {
            try
            {
                if (doctorId <= 0)
                    return Json(new { success = false, message = "Invalid doctor" });

                var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
                if (doctor == null)
                    return Json(new { success = false, message = "Doctor not found" });

                // Fallback hours if none configured: 9am-5pm
                var start = doctor.AvailableFrom ?? new TimeSpan(9, 0, 0);
                var end = doctor.AvailableTo ?? new TimeSpan(17, 0, 0);
                if (end <= start) end = start.Add(TimeSpan.FromHours(8));

                var slots = new List<object>();
                var day = date.Date;
                for (var t = start; t < end; t = t.Add(TimeSpan.FromMinutes(durationMinutes)))
                {
                    var slotStart = day.Add(t);
                    var isAvailable = await _appointmentService.IsDoctorAvailableAsync(doctorId, slotStart, durationMinutes);
                    slots.Add(new
                    {
                        time24 = t.ToString(@"hh\:mm"),
                        time12 = DateTime.Today.Add(t).ToString("hh:mm tt"),
                        available = isAvailable
                    });
                }

                return Json(new { success = true, slots });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing availability slots for doctor {DoctorId} on {Date}", doctorId, date);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PatientDetails(int id)
        {
            try
            {
                var patient = await _patientRepository.GetPatientByIdAsync(id);
                if (patient == null)
                {
                    TempData["ErrorMessage"] = "Patient not found";
                    return RedirectToAction("Index");
                }
                return View(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading patient details for ID: {id}");
                TempData["ErrorMessage"] = $"Error loading patient: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditPatient(int id)
        {
            try
            {
                var patient = await _patientRepository.GetPatientByIdAsync(id);
                if (patient == null)
                {
                    TempData["ErrorMessage"] = "Patient not found";
                    return RedirectToAction(nameof(Index));
                }
                return View(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form for patient {PatientId}", id);
                TempData["ErrorMessage"] = "Error loading edit form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(Patient patient)
        {
            try
            {
                if (patient.PatientId <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid patient.";
                    return RedirectToAction(nameof(Index));
                }

                // Reuse the same basic validations
                if (string.IsNullOrWhiteSpace(patient.FirstName))
                {
                    ModelState.AddModelError("FirstName", "First Name is required");
                }
                if (string.IsNullOrWhiteSpace(patient.LastName))
                {
                    ModelState.AddModelError("LastName", "Last Name is required");
                }
                if (string.IsNullOrWhiteSpace(patient.Phone))
                {
                    ModelState.AddModelError("Phone", "Phone Number is required");
                }
                if (string.IsNullOrWhiteSpace(patient.Gender))
                {
                    ModelState.AddModelError("Gender", "Gender is required");
                }
                if (patient.DateOfBirth == default || patient.DateOfBirth > DateTime.Now)
                {
                    ModelState.AddModelError("DateOfBirth", "Please select a valid Date of Birth");
                }

                if (!ModelState.IsValid)
                {
                    return View(patient);
                }

                var ok = await _patientRepository.UpdatePatientAsync(patient);
                if (ok)
                {
                    TempData["SuccessMessage"] = "Patient details updated successfully.";
                    return RedirectToAction(nameof(PatientDetails), new { id = patient.PatientId });
                }

                TempData["ErrorMessage"] = "No changes were saved.";
                return View(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient {PatientId}", patient.PatientId);
                TempData["ErrorMessage"] = $"Error updating patient: {ex.Message}";
                return View(patient);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchPatient(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return Json(new { success = false, message = "Please enter a search term" });
                }

                var patients = await _patientRepository.SearchPatientsAsync(searchTerm);
                return Json(new
                {
                    success = true,
                    patients = patients.ToList().Select(p => new
                    {
                        patientId = p.PatientId,
                        fullName = p.FullName,
                        phone = p.Phone,
                        email = p.Email ?? "N/A",
                        gender = p.Gender ?? "N/A",
                        age = p.Age
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching patients with term: {searchTerm}");
                return Json(new { success = false, message = $"Error searching patients: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientByPhone(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                {
                    return Json(new { success = false, message = "Please enter a phone number" });
                }

                var patients = await _patientRepository.GetPatientsByPhoneAsync(phone);

                if (patients.Any())
                {
                    return Json(new
                    {
                        success = true,
                        patients = patients.ToList().Select(p => new
                        {
                            patientId = p.PatientId,
                            fullName = p.FullName,
                            phone = p.Phone,
                            email = p.Email ?? "N/A",
                            gender = p.Gender ?? "N/A",
                            age = p.Age,
                            registrationDate = p.RegistrationDate.ToString("yyyy-MM-dd")
                        })
                    });
                }

                return Json(new { success = false, message = "No patients found with this phone number" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting patient by phone: {phone}");
                return Json(new { success = false, message = $"Error searching patient: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ScheduleAppointmentForPhone(string phone, int patientId)
        {
            try
            {
                var patient = await _patientRepository.GetPatientByIdAsync(patientId);
                if (patient == null)
                {
                    TempData["ErrorMessage"] = "Patient not found";
                    return RedirectToAction("Index");
                }

                // Get available doctors for scheduling
                var doctors = await _doctorRepository.GetAvailableDoctorsAsync();
                var departments = await _doctorRepository.GetAllDepartmentsAsync();

                ViewBag.Patient = patient;
                ViewBag.Departments = departments;
                ViewBag.Doctors = doctors;
                ViewBag.MinDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.MaxDate = DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd");

                var appointment = new Appointment
                {
                    PatientId = patientId,
                    AppointmentDate = DateTime.Today,
                    AppointmentTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(1)),
                    Status = "Scheduled"
                };

                // If there are doctors, pre-select the first one as a default for the form
                if (doctors != null && doctors.Any())
                {
                    appointment.DoctorId = doctors.First().DoctorId;
                }

                return View("ScheduleAppointment", appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error scheduling appointment for phone: {phone}, patientId: {patientId}");
                TempData["ErrorMessage"] = $"Error scheduling appointment: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduleAppointment(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get patient and doctor details first
                    var appointmentPatient = await _patientRepository.GetPatientByIdAsync(appointment.PatientId);
                    var appointmentDoctor = await _doctorRepository.GetDoctorByIdAsync(appointment.DoctorId);

                    if (appointmentPatient == null)
                    {
                        TempData["ErrorMessage"] = "Patient not found. Please try again.";
                        return RedirectToAction(nameof(Index));
                    }

                    if (appointmentDoctor == null)
                    {
                        TempData["ErrorMessage"] = "Doctor not found. Please select a valid doctor.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Check doctor availability before adding the appointment
                    var appointmentDateTime = appointment.AppointmentDate.Date + appointment.AppointmentTime;
                    var isAvailable = await _appointmentService.IsDoctorAvailableAsync(appointment.DoctorId, appointmentDateTime, appointment.DurationMinutes);

                    if (!isAvailable)
                    {
                        ModelState.AddModelError(string.Empty, "The selected doctor is not available at the chosen time. Please choose a different time slot.");
                    }
                    else
                    {
                        // Set appointment details
                        appointment.DoctorName = appointmentDoctor.DoctorName;
                        appointment.Specialization = appointmentDoctor.Specialization;
                        appointment.CreatedBy = HttpContext.Session.GetInt32("UserId") ?? 1; // Get from session or default

                        // Ensure required audit fields
                        if (appointment.DurationMinutes <= 0)
                        {
                            appointment.DurationMinutes = 30;
                        }
                        appointment.CreatedDate = DateTime.Now;

                        // Schedule the appointment
                        var appointmentId = await _appointmentService.AddAppointmentAsync(appointment);

                        if (appointmentId <= 0)
                        {
                            throw new Exception("Insert returned 0. Please verify DB connection string and table schema matches.");
                        }

                        // Create detailed success message
                        var appointmentCode = $"APT{appointmentId:00000}";
                        var formattedDate = appointment.AppointmentDate.ToString("dddd, MMMM dd, yyyy");
                        var formattedTime = DateTime.Today.Add(appointment.AppointmentTime).ToString("hh:mm tt");

                        // Set fallback success message for dashboard
                        TempData["SuccessMessage"] = $"🎉 Appointment Successfully Scheduled! Code: {appointmentCode}";
                        TempData["AppointmentDetails"] = $"Patient: {appointmentPatient.FullName} | Doctor: Dr. {appointmentDoctor.DoctorName} | Date: {formattedDate} at {formattedTime}";

                        // Redirect to success page with appointment details
                        return RedirectToAction("AppointmentSuccess", new { id = appointmentId });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error scheduling appointment for patient {PatientId}", appointment.PatientId);
                    TempData["ErrorMessage"] = $"Appointment not scheduled: {ex.Message}";
                }
            }
            else
            {
                _logger.LogWarning("ModelState is invalid for ScheduleAppointment for patient {PatientId}. Errors: {Errors}",
                    appointment.PatientId,
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).ToList().Select(e => e != null ? e.ErrorMessage : "Unknown error")));
            }

            // If ModelState is not valid or an error occurred, re-populate ViewBag data and return the view
            var patient = await _patientRepository.GetPatientByIdAsync(appointment.PatientId);
            var departments = await _doctorRepository.GetAllDepartmentsAsync();
            var doctors = await _doctorRepository.GetAvailableDoctorsAsync();

            ViewBag.Patient = patient;
            ViewBag.Departments = departments;
            ViewBag.Doctors = doctors;
            ViewBag.MinDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.MaxDate = DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd");

            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> AppointmentSuccess(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Appointment not found.";
                    return RedirectToAction(nameof(Index));
                }

                var patient = await _patientRepository.GetPatientByIdAsync(appointment.PatientId);
                var doctor = await _doctorRepository.GetDoctorByIdAsync(appointment.DoctorId);

                ViewBag.Patient = patient;
                ViewBag.Doctor = doctor;

                return View(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error displaying appointment success for appointment {AppointmentId}", id);
                TempData["ErrorMessage"] = "Error loading appointment details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointmentDetails(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return Json(new { success = false, message = "Appointment not found" });
                }

                var patient = await _patientRepository.GetPatientByIdAsync(appointment.PatientId);
                var doctor = await _doctorRepository.GetDoctorByIdAsync(appointment.DoctorId);

                var appointmentDetails = new
                {
                    success = true,
                    appointment = new
                    {
                        appointmentId = appointment.AppointmentId,
                        appointmentCode = appointment.AppointmentCode,
                        appointmentDate = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                        appointmentTime = DateTime.Today.Add(appointment.AppointmentTime).ToString("hh:mm tt"),
                        status = appointment.Status,
                        reason = appointment.Reason,
                        notes = appointment.Notes,
                        durationMinutes = appointment.DurationMinutes
                    },
                    patient = new
                    {
                        patientId = patient?.PatientId,
                        fullName = patient?.FullName,
                        phone = patient?.Phone,
                        email = patient?.Email,
                        age = patient?.Age,
                        gender = patient?.Gender,
                        bloodGroup = patient?.BloodGroup
                    },
                    doctor = new
                    {
                        doctorId = doctor?.DoctorId,
                        doctorName = doctor?.DoctorName,
                        specialization = doctor?.Specialization,
                        qualification = doctor?.Qualification,
                        consultationFee = doctor?.ConsultationFee,
                        availableFrom = doctor?.AvailableFrom.HasValue == true ? DateTime.Today.Add(doctor.AvailableFrom.Value).ToString("hh:mm tt") : "",
                        availableTo = doctor?.AvailableTo.HasValue == true ? DateTime.Today.Add(doctor.AvailableTo.Value).ToString("hh:mm tt") : ""
                    }
                };

                return Json(appointmentDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointment details for appointment {AppointmentId}", id);
                return Json(new { success = false, message = "Error retrieving appointment details" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorsByDepartment(int departmentId)
        {
            try
            {
                var doctors = await _doctorRepository.GetDoctorsByDepartmentAsync(departmentId);
                return Json(new
                {
                    success = true,
                    doctors = (doctors ?? Enumerable.Empty<DoctorDto>()).ToList().Select(d => new
                    {
                        doctorId = d.DoctorId,
                        doctorName = d.DoctorName,
                        specialization = d.Specialization,
                        consultationFee = d.ConsultationFee,
                        availableFrom = d.AvailableFrom != default(TimeSpan) ? DateTime.Today.Add(d.AvailableFrom).ToString("hh:mm tt") : "",
                        availableTo = d.AvailableTo != default(TimeSpan) ? DateTime.Today.Add(d.AvailableTo).ToString("hh:mm tt") : ""
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors by department {DepartmentId}", departmentId);
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListPatients()
        {
            try
            {
                var patients = await _patientRepository.GetAllAsync();
                return View(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading patients list");
                TempData["ErrorMessage"] = "Error loading patients. Please try again.";
                return View(new List<Patient>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchPatientById(int id)
        {
            try
            {
                var patient = await _patientRepository.GetPatientByIdAsync(id);
                if (patient != null)
                {
                    return Json(new
                    {
                        success = true,
                        patient = new
                        {
                            patientId = patient.PatientId,
                            fullName = patient.FullName,
                            phone = patient.Phone,
                            email = patient.Email ?? "N/A",
                            gender = patient.Gender,
                            age = patient.Age,
                            registrationDate = patient.RegistrationDate.ToString("yyyy-MM-dd")
                        }
                    });
                }
                return Json(new { success = false, message = "Patient not found with this ID" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching patient by ID: {id}");
                return Json(new { success = false, message = $"Error searching patient: {ex.Message}" });
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SearchPatientByIdOrPhone(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return Json(new { success = false, message = "Please enter a search term" });
                }

                // Normalize CS-formatted MMR ID (e.g., CS01) to numeric for search
                var term = searchTerm.Trim();
                var upper = term.ToUpperInvariant();
                if (upper.StartsWith("CS"))
                {
                    var csDigits = new string(term.Where(char.IsDigit).ToArray());
                    if (!string.IsNullOrEmpty(csDigits))
                    {
                        term = csDigits;
                    }
                }
                var patients = new List<Patient>();

                if (int.TryParse(term, out int patientId))
                {
                    var patientById = await _patientRepository.GetPatientByIdAsync(patientId);
                    if (patientById != null)
                    {
                        patients.Add(patientById);
                    }
                }

                var patientsByPhone = await _patientRepository.GetPatientsByPhoneAsync(term);
                patients.AddRange(patientsByPhone.ToList().Where(p => p != null && !patients.Any(e => e.PatientId == p.PatientId)));

                var phoneDigits = new string(term.Where(char.IsDigit).ToArray());
                if (!patients.Any() && phoneDigits.Length >= 7)
                {
                    var all = await _patientRepository.GetAllAsync();
                    var more = all.ToList().Where(p =>
                    {
                        var d = new string((p.Phone ?? string.Empty).Where(char.IsDigit).ToArray());
                        return !string.IsNullOrEmpty(d) && (d == phoneDigits || d.EndsWith(phoneDigits));
                    });
                    patients.AddRange(more);
                }

                if (!patients.Any())
                {
                    var searchByNameEmail = await _patientRepository.SearchPatientsAsync(term);
                    patients.AddRange(searchByNameEmail.ToList());
                }

                patients = patients
                    .GroupBy(p => p.PatientId)
                    .Select(g => g.First())
                    .ToList();

                if (patients.Any())
                {
                    return Json(new
                    {
                        success = true,
                        patients = patients.Select(p => new
                        {
                            patientId = p.PatientId,
                            fullName = p.FullName,
                            phone = p.Phone,
                            email = p.Email ?? "N/A",
                            gender = p.Gender,
                            age = p.Age,
                            registrationDate = p.RegistrationDate.ToString("yyyy-MM-dd"),
                            address = p.Address ?? "N/A",
                            bloodGroup = p.BloodGroup ?? "N/A",
                            emergencyContact = p.EmergencyContact ?? "N/A"
                        })
                    });
                }

                return Json(new { success = false, message = "No patients found with this ID, phone, name, or email" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching patient by ID or phone: {searchTerm}");
                return Json(new { success = false, message = $"Error searching patient: {ex.Message}" });
            }
        }

        // GET variant of the same search to simplify AJAX usage from views
        [HttpGet]
        public async Task<IActionResult> SearchPatientByIdOrPhoneGet(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return Json(new { success = false, message = "Please enter a search term" });
                }

                // Normalize CS-formatted MMR ID (e.g., CS01) to numeric for search
                var term = searchTerm.Trim();
                var upper = term.ToUpperInvariant();
                if (upper.StartsWith("CS"))
                {
                    var csDigits = new string(term.Where(char.IsDigit).ToArray());
                    if (!string.IsNullOrEmpty(csDigits))
                    {
                        term = csDigits;
                    }
                }
                var patients = new List<Patient>();

                if (int.TryParse(term, out int patientId))
                {
                    var patientById = await _patientRepository.GetPatientByIdAsync(patientId);
                    if (patientById != null)
                    {
                        patients.Add(patientById);
                    }
                }

                var patientsByPhone = await _patientRepository.GetPatientsByPhoneAsync(term);
                patients.AddRange(patientsByPhone.ToList().Where(p => p != null && !patients.Any(e => e.PatientId == p.PatientId)));

                var phoneDigits = new string(term.Where(char.IsDigit).ToArray());
                if (!patients.Any() && phoneDigits.Length >= 7)
                {
                    var all = await _patientRepository.GetAllAsync();
                    var more = all.ToList().Where(p =>
                    {
                        var d = new string((p.Phone ?? string.Empty).Where(char.IsDigit).ToArray());
                        return !string.IsNullOrEmpty(d) && (d == phoneDigits || d.EndsWith(phoneDigits));
                    });
                    patients.AddRange(more);
                }

                if (!patients.Any())
                {
                    var searchByNameEmail = await _patientRepository.SearchPatientsAsync(term);
                    patients.AddRange(searchByNameEmail.ToList());
                }

                patients = patients
                    .GroupBy(p => p.PatientId)
                    .Select(g => g.First())
                    .ToList();

                if (patients.Any())
                {
                    return Json(new
                    {
                        success = true,
                        patients = patients.Select(p => new
                        {
                            patientId = p.PatientId,
                            fullName = p.FullName,
                            phone = p.Phone,
                            email = p.Email ?? "N/A",
                            gender = p.Gender,
                            age = p.Age,
                            registrationDate = p.RegistrationDate.ToString("yyyy-MM-dd"),
                            address = p.Address ?? "N/A",
                            bloodGroup = p.BloodGroup ?? "N/A",
                            emergencyContact = p.EmergencyContact ?? "N/A"
                        })
                    });
                }

                return Json(new { success = false, message = "No patients found with this ID, phone, name, or email" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching (GET) patient by ID or phone: {searchTerm}");
                return Json(new { success = false, message = $"Error searching patient: {ex.Message}" });
            }
        }

        // Debug endpoint for testing patient search
        [HttpGet]
        public async Task<IActionResult> TestPatientSearch(string? searchTerm = "1")
        {
            try
            {
                var patients = new List<Patient>();

                // Try to parse as ID first
                if (int.TryParse(searchTerm?.Trim(), out int patientId))
                {
                    var patientById = await _patientRepository.GetPatientByIdAsync(patientId);
                    if (patientById != null)
                    {
                        patients.Add(patientById);
                    }
                }

                // Also search by phone number
                var patientsByPhone = await _patientRepository.GetPatientsByPhoneAsync(searchTerm?.Trim() ?? "");
                patients.AddRange(patientsByPhone.ToList().Where(p => p != null && !patients.Any(existing => existing.PatientId == p.PatientId)));

                return Json(new
                {
                    success = true,
                    searchTerm = searchTerm,
                    totalFound = patients.Count,
                    patients = patients.Select(p => new
                    {
                        patientId = p.PatientId,
                        fullName = p.FullName,
                        phone = p.Phone,
                        email = p.Email ?? "N/A",
                        gender = p.Gender,
                        age = p.Age
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    searchTerm = searchTerm
                });
            }
        }

        // Simple test endpoint to verify search is working
        [HttpGet]
        public async Task<IActionResult> TestSearch()
        {
            try
            {
                // Test with a simple patient ID search
                var patients = await _patientRepository.GetAllAsync();
                var patient = patients?.FirstOrDefault();

                if (patient != null)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Search endpoint is working",
                        samplePatient = new
                        {
                            patientId = patient.PatientId,
                            fullName = patient.FullName,
                            phone = patient.Phone
                        }
                    });
                }

                return Json(new { success = false, message = "No patients in database for testing" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
//Latest code