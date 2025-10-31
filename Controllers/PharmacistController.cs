using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalManagementSystem2025.Controllers
{
    public class PharmacistController : Controller
    {
        private readonly IMedicineRepository _medicineRepository;
        private readonly IPharmacyInventoryRepository _inventoryRepository;
        private readonly IDispensedMedicationRepository _dispensedRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;

        public PharmacistController(
            IMedicineRepository medicineRepository,
            IPharmacyInventoryRepository inventoryRepository,
            IDispensedMedicationRepository dispensedRepository,
            IPrescriptionRepository prescriptionRepository)
        {
            _medicineRepository = medicineRepository;
            _inventoryRepository = inventoryRepository;
            _dispensedRepository = dispensedRepository;
            _prescriptionRepository = prescriptionRepository;
        }

        // GET: Pharmacist - Main Dashboard
        public async Task<IActionResult> Index()
        {
            var medicines = await _medicineRepository.GetAllMedicinesAsync();
            var lowStockMedicines = await _medicineRepository.GetLowStockMedicinesAsync();
            var lowStockInventory = await _inventoryRepository.GetLowStockItemsAsync();

            // Get prescription statistics for dashboard
            var allPrescriptions = await _prescriptionRepository.GetAllPrescriptionsAsync();
            var pendingPrescriptions = allPrescriptions.Where(p => p != null && (p.Status == "Pending" || p.Status == null)).ToList();
            var fulfilledToday = allPrescriptions.Where(p => p != null && p.Status == "Fulfilled" && p.FulfilledDate.HasValue && p.FulfilledDate.Value.Date == DateTime.Today).ToList();

            ViewBag.LowStockMedicines = lowStockMedicines;
            ViewBag.LowStockInventory = lowStockInventory;
            ViewBag.PendingPrescriptionsCount = pendingPrescriptions.Count;
            ViewBag.FulfilledTodayCount = fulfilledToday.Count;
            ViewBag.LowStockCount = (lowStockMedicines?.Count() ?? 0) + (lowStockInventory?.Count() ?? 0);

            return View(medicines);
        }

        // GET: Pharmacist/Medicines - Alternative dashboard view
        public async Task<IActionResult> Medicines()
        {
            return await Index();
        }

        // GET: Pharmacist/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var medicine = await _medicineRepository.GetMedicineByIdAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }

            var inventory = await _inventoryRepository.GetInventoryByMedicineIdAsync(id);
            var dispensedHistory = await _dispensedRepository.GetDispensedByMedicineIdAsync(id);

            ViewBag.Inventory = inventory;
            ViewBag.DispensedHistory = dispensedHistory;

            return View(medicine);
        }

        // GET: Pharmacist/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pharmacist/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicine medicine)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _medicineRepository.AddMedicineAsync(medicine);
                    if (result > 0)
                    {
                        TempData["SuccessMessage"] = "Medicine added successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }

                // If we reach here, there was a validation error
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                TempData["ErrorMessage"] = $"Error adding medicine: {string.Join(", ", errors.Select(e => e.ErrorMessage))}";
                return View(medicine);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(medicine);
            }
        }

        // GET: Pharmacist/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await _medicineRepository.GetMedicineByIdAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        // POST: Pharmacist/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medicine medicine)
        {
            if (id != medicine.MedicineId)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _medicineRepository.UpdateMedicineAsync(medicine);
                    if (result != null)
                    {
                        TempData["SuccessMessage"] = "Medicine updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }

                // If we reach here, there was a validation error
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                TempData["ErrorMessage"] = $"Error updating medicine: {string.Join(", ", errors.Select(e => e != null ? e.ErrorMessage : "Unknown error"))}";
                return View(medicine);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(medicine);
            }
        }

        // GET: Pharmacist/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var medicine = await _medicineRepository.GetMedicineByIdAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        // POST: Pharmacist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _medicineRepository.DeleteMedicineAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Medicine deleted successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Error deleting medicine.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Pharmacist/Search
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var medicines = await _medicineRepository.SearchMedicinesAsync(searchTerm);
            ViewBag.SearchTerm = searchTerm;
            return View("Index", medicines);
        }

        // GET: Pharmacist/Prescriptions - Redirect to Pharmacy
        public async Task<IActionResult> Prescriptions()
        {
            return RedirectToAction("Prescriptions", "Pharmacy");
        }

        // GET: Pharmacist/Inventory - Redirect to Pharmacy
        public async Task<IActionResult> Inventory()
        {
            return RedirectToAction("Inventory", "Pharmacy");
        }

        // GET: Pharmacist/AddInventory - Redirect to Pharmacy
        public async Task<IActionResult> AddInventory(int? medicineId)
        {
            return RedirectToAction("AddInventory", "Pharmacy", new { medicineId });
        }

        // GET: Pharmacist/Dispense - Redirect to Pharmacy
        public async Task<IActionResult> Dispense()
        {
            return RedirectToAction("Dispense", "Pharmacy");
        }

        // GET: Pharmacist/DispenseHistory - Redirect to Pharmacy
        public async Task<IActionResult> DispenseHistory()
        {
            return RedirectToAction("DispenseHistory", "Pharmacy");
        }

        // GET: Pharmacist/LowStock
        public async Task<IActionResult> LowStock()
        {
            var lowStockMedicines = await _medicineRepository.GetLowStockMedicinesAsync();
            var lowStockInventory = await _inventoryRepository.GetLowStockItemsAsync();

            ViewBag.LowStockMedicines = lowStockMedicines;
            ViewBag.LowStockInventory = lowStockInventory;

            return View();
        }
    }
}
