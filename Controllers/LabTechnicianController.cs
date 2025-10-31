// Controllers/LabTechnicianController.cs
using Microsoft.AspNetCore.Mvc;
using ClinicalManagementSystem2025.Repository;
using ClinicalManagementSystem2025.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace ClinicalManagementSystem2025.Controllers
{
    public class LabTechnicianController : Controller
    {
        private readonly ILabRepository _labRepository;

        public LabTechnicianController(ILabRepository labRepository)
        {
            _labRepository = labRepository;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Lab Technician Dashboard";
            ViewBag.Role = "Lab Technician";
            ViewBag.UserName = Request.Cookies["FullName"];
            return View();
        }

        public async Task<IActionResult> Reports()
        {
            var technicianId = int.Parse(Request.Cookies["UserId"] ?? "0");
            var reports = await _labRepository.GetLabReportsByTechnicianAsync(technicianId);
            return View(reports);
        }

        public async Task<IActionResult> PendingReports()
        {
            var reports = await _labRepository.GetPendingLabReportsAsync();
            return View(reports);
        }

        // New method for pending lab test prescriptions (replaces old PendingReports functionality)
        public async Task<IActionResult> PendingLabTests()
        {
            var pendingPrescriptions = await _labRepository.GetPendingLabTestPrescriptionsAsync();
            return View(pendingPrescriptions);
        }

        public async Task<IActionResult> EditReport(int id)
        {
            var report = await _labRepository.GetLabReportByIdAsync(id);
            if (report == null)
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction("Reports");
            }
            return View(report);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReport(LabReport model)
        {
            if (ModelState.IsValid)
            {
                var updatedReport = await _labRepository.UpdateLabReportAsync(model);
                TempData["SuccessMessage"] = "Report updated successfully.";
                return RedirectToAction("Reports");
            }
            TempData["ErrorMessage"] = "Error updating report.";
            return View("EditReport", model);
        }

        // Lab Test Prescription functionality
        public async Task<IActionResult> PrescribedTests()
        {
            var pendingPrescriptions = await _labRepository.GetPendingLabTestPrescriptionsAsync();
            return View(pendingPrescriptions);
        }

        public async Task<IActionResult> MyAssignedTests()
        {
            var technicianId = int.Parse(Request.Cookies["UserId"] ?? "0");
            var assignedTests = await _labRepository.GetAssignedLabTestPrescriptionsAsync(technicianId);
            return View(assignedTests);
        }

        [HttpPost]
        public async Task<IActionResult> AssignToMe(int prescriptionId)
        {
            var technicianId = int.Parse(Request.Cookies["UserId"] ?? "0");

            var success = await _labRepository.AssignTechnicianToPrescriptionAsync(prescriptionId, technicianId);
            if (success)
            {
                TempData["SuccessMessage"] = "Lab test assigned to you successfully.";
                return RedirectToAction("MyAssignedTests");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to assign lab test.";
                return RedirectToAction("PrescribedTests");
            }
        }

        public async Task<IActionResult> StartTest(int id)
        {
            var prescription = await _labRepository.GetLabTestPrescriptionByIdAsync(id);
            if (prescription == null)
            {
                TempData["ErrorMessage"] = "Lab test prescription not found.";
                return RedirectToAction("MyAssignedTests");
            }

            // Update status to InProgress
            prescription.Status = "InProgress";
            await _labRepository.UpdateLabTestPrescriptionAsync(prescription);

            ViewBag.LabTest = await _labRepository.GetLabTestByIdAsync(prescription.LabTestId);
            return View(prescription);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteTest(int prescriptionId, string resultValue, string resultUnit, string findings, string notes)
        {
            var prescription = await _labRepository.GetLabTestPrescriptionByIdAsync(prescriptionId);
            if (prescription == null)
            {
                TempData["ErrorMessage"] = "Lab test prescription not found.";
                return RedirectToAction("MyAssignedTests");
            }

            // Update prescription status
            prescription.Status = "Completed";
            prescription.CompletedDate = DateTime.Now;
            prescription.Notes = notes;
            await _labRepository.UpdateLabTestPrescriptionAsync(prescription);

            // Create lab report
            var labReport = new LabReport
            {
                ReportCode = $"RPT{prescriptionId:00000}",
                PatientId = prescription.PatientId,
                LabTestId = prescription.LabTestId,
                DoctorId = prescription.DoctorId,
                LabTechnicianId = prescription.AssignedTechnicianId ?? 0,
                CollectionDate = prescription.SampleCollectionDate ?? DateTime.Now,
                ReportDate = DateTime.Now,
                ResultValue = resultValue,
                ResultUnit = resultUnit,
                Findings = findings,
                Status = "Completed",
                Notes = notes,
                CreatedDate = DateTime.Now
            };

            await _labRepository.AddLabReportAsync(labReport);

            TempData["SuccessMessage"] = $"Lab test completed successfully! Report {labReport.ReportCode} sent to Dr. {prescription.DoctorName}.";
            return RedirectToAction("MyAssignedTests");
        }

        public async Task<IActionResult> TestDetails(int id)
        {
            var prescription = await _labRepository.GetLabTestPrescriptionByIdAsync(id);
            if (prescription == null)
            {
                TempData["ErrorMessage"] = "Lab test prescription not found.";
                return RedirectToAction("MyAssignedTests");
            }

            ViewBag.LabTest = await _labRepository.GetLabTestByIdAsync(prescription.LabTestId);
            return View(prescription);
        }

        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("UserName");
            Response.Cookies.Delete("FullName");
            Response.Cookies.Delete("RoleId");

            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login", "Login");
        }
    }
}