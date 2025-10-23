using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Services;
using ClinicalManagementSystem2025.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicalManagementSystem2025.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(
            IPatientService patientService,
            IDoctorService doctorService,
            IAppointmentService appointmentService)
        {
            _patientService = patientService;
            _doctorService = doctorService;
            _appointmentService = appointmentService;
        }

        // GET: Appointment/Schedule
        public async Task<IActionResult> Schedule()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: Appointment/Schedule
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    PatientId = model.PatientId,
                    DoctorId = model.DoctorId,
                    AppointmentDate = model.AppointmentDate.Date,
                    AppointmentTime = model.AppointmentDate.TimeOfDay,
                    DurationMinutes = model.DurationMinutes,
                    Status = model.Status,
                    Reason = model.Reason,
                    Notes = model.Notes,
                    CreatedBy = 1, // TODO: Replace with actual logged-in user ID
                    CreatedDate = DateTime.Now
                };

                await _appointmentService.AddAppointmentAsync(appointment);
                TempData["successMessage"] = "Appointment scheduled successfully!";
                return RedirectToAction(nameof(Schedule));
            }

            await PopulateDropdowns();
            return View(model);
        }

        private async Task PopulateDropdowns()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            ViewBag.Patients = new SelectList(patients, "PatientId", "FullName");

            var doctors = await _doctorService.GetAllDoctorsAsync();
            ViewBag.Doctors = new SelectList(doctors, "DoctorId", "DoctorName");
        }
    }
}