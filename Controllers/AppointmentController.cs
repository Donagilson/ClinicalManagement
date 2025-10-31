using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Services;
using ClinicalManagementSystem2025.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

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

        private async Task PopulateDropdowns()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            ViewBag.Patients = new SelectList(patients, "PatientId", "FullName");

            var doctors = await _doctorService.GetAllDoctorsAsync();
            ViewBag.Doctors = new SelectList(doctors, "DoctorId", "DoctorName");
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
                // Check if the selected time slot is available
                var appointmentDateTime = model.AppointmentDate;
                var isAvailable = await _appointmentService.IsDoctorAvailableAsync(model.DoctorId, appointmentDateTime, model.DurationMinutes);

                if (!isAvailable)
                {
                    TempData["ErrorMessage"] = "The selected time slot is already booked. Please choose a different time.";
                    await PopulateDropdowns();
                    return View(model);
                }

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

        // GET: Appointment/ScheduleWithTimeSlots
        public async Task<IActionResult> ScheduleWithTimeSlots()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: Appointment/CheckAvailability
        [HttpPost]
        public async Task<JsonResult> CheckAvailability(int doctorId, DateTime appointmentDate, int durationMinutes = 30)
        {
            var appointmentDateTime = appointmentDate.Date.Add(appointmentDate.TimeOfDay);

            var isAvailable = await _appointmentService.IsDoctorAvailableAsync(doctorId, appointmentDateTime, durationMinutes);

            return Json(new
            {
                isAvailable,
                message = isAvailable ? "Time slot is available" : "Time slot is already booked"
            });
        }

        // GET: Appointment/GetAvailableTimeSlots
        public async Task<JsonResult> GetAvailableTimeSlots(int doctorId, DateTime appointmentDate)
        {
            var allTimeSlots = GenerateTimeSlots();
            var availableSlots = new List<object>();
            var bookedSlots = new List<object>();

            // Get existing appointments for this doctor on this date
            var existingAppointments = await _appointmentService.GetDoctorAppointmentsForDateAsync(doctorId, appointmentDate.Date);

            foreach (var slot in allTimeSlots)
            {
                var slotDateTime = appointmentDate.Date.Add(slot);
                var isAvailable = await _appointmentService.IsDoctorAvailableAsync(doctorId, slotDateTime, 30);

                var slotInfo = new
                {
                    time = slot.ToString(@"hh\:mm"),
                    timeValue = slot.ToString(@"HH\:mm"),
                    isAvailable,
                    isPast = slotDateTime < DateTime.Now,
                    className = GetTimeSlotClass(slotDateTime, isAvailable, existingAppointments)
                };

                if (isAvailable && slotDateTime >= DateTime.Now)
                {
                    availableSlots.Add(slotInfo);
                }
                else
                {
                    bookedSlots.Add(slotInfo);
                }
            }

            return Json(new
            {
                availableSlots,
                bookedSlots,
                totalSlots = allTimeSlots.Count,
                availableCount = availableSlots.Count
            });
        }

        private List<TimeSpan> GenerateTimeSlots()
        {
            var slots = new List<TimeSpan>();
            var startTime = new TimeSpan(9, 0, 0); // 9:00 AM
            var endTime = new TimeSpan(17, 0, 0);  // 5:00 PM
            var interval = new TimeSpan(0, 30, 0); // 30 minutes

            for (var time = startTime; time <= endTime; time = time.Add(interval))
            {
                slots.Add(time);
            }

            return slots;
        }

        private string GetTimeSlotClass(DateTime slotDateTime, bool isAvailable, IEnumerable<Appointment> existingAppointments)
        {
            if (slotDateTime < DateTime.Now)
            {
                return "time-slot past-time";
            }
            else if (!isAvailable)
            {
                return "time-slot booked-time";
            }
            else
            {
                return "time-slot available-time";
            }
        }
    }
}