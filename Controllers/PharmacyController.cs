using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ClinicalManagementSystem2025.Controllers
{
    public class PharmacyController : Controller
    {
        private readonly IMedicineRepository _medicineRepository;
        private readonly IPharmacyInventoryRepository _inventoryRepository;
        private readonly IDispensedMedicationRepository _dispensedRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;

        public PharmacyController(
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

        // GET: Pharmacy - Main pharmacy dashboard
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

        // GET: Pharmacy/Details/5
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

        // GET: Pharmacy/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pharmacy/Create
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

        // GET: Pharmacy/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await _medicineRepository.GetMedicineByIdAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        // POST: Pharmacy/Edit/5
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

        // GET: Pharmacy/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var medicine = await _medicineRepository.GetMedicineByIdAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        // POST: Pharmacy/Delete/5
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

        // GET: Pharmacy/Inventory
        public async Task<IActionResult> Inventory()
        {
            Console.WriteLine("Inventory GET action called");
            var inventory = await _inventoryRepository.GetAllInventoryAsync();
            Console.WriteLine($"Retrieved {inventory.Count()} inventory items");

            foreach (var item in inventory)
            {
                Console.WriteLine($"Inventory Item {item.InventoryId}: MedicineId={item.MedicineId}, MedicineName={item.Medicine?.MedicineName}, Stock={item.CurrentStock}");
            }

            return View(inventory);
        }

        // GET: Pharmacy/Inventory/Create
        public async Task<IActionResult> CreateInventory(int medicineId)
        {
            var medicine = await _medicineRepository.GetMedicineByIdAsync(medicineId);
            if (medicine == null)
            {
                return NotFound();
            }

            var inventory = new PharmacyInventory
            {
                MedicineId = medicineId,
                SellingPrice = medicine.UnitPrice
            };

            ViewBag.Medicine = medicine;
            return View(inventory);
        }

        // POST: Pharmacy/Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInventory(PharmacyInventory inventory)
        {
            if (ModelState.IsValid)
            {
                var result = await _inventoryRepository.AddInventoryItemAsync(inventory);
                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Inventory item added successfully!";
                    return RedirectToAction(nameof(Inventory));
                }
            }

            TempData["ErrorMessage"] = "Error adding inventory item.";
            var medicine = await _medicineRepository.GetMedicineByIdAsync(inventory.MedicineId);
            ViewBag.Medicine = medicine;
            return View(inventory);
        }

        // GET: Pharmacy/Dispense
        public async Task<IActionResult> Dispense()
        {
            var medicines = await _medicineRepository.GetAllMedicinesAsync();
            ViewBag.Medicines = medicines;
            return View();
        }

        // POST: Pharmacy/Dispense
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dispense(int MedicineId, int PatientId, int Quantity, decimal UnitPrice, decimal TotalAmount, string Dosage, string Frequency, string Duration, string Instructions, string PaymentMethod, string Notes)
        {
            Console.WriteLine("=== Dispense POST Debug ===");
            Console.WriteLine($"MedicineId: {MedicineId}, PatientId: {PatientId}, Quantity: {Quantity}, Dosage: {Dosage}, Frequency: {Frequency}, Duration: {Duration}");

            try
            {
                // Validate required fields
                if (MedicineId <= 0 || PatientId <= 0 || Quantity <= 0 || string.IsNullOrWhiteSpace(Dosage) || string.IsNullOrWhiteSpace(Frequency) || string.IsNullOrWhiteSpace(Duration))
                {
                    TempData["ErrorMessage"] = "Please fill in all required fields.";
                    var medicines = await _medicineRepository.GetAllMedicinesAsync();
                    ViewBag.Medicines = medicines;
                    return View();
                }

                // Get medicine and inventory
                var medicine = await _medicineRepository.GetMedicineByIdAsync(MedicineId);
                if (medicine == null)
                {
                    TempData["ErrorMessage"] = "Medicine not found.";
                    var medicines = await _medicineRepository.GetAllMedicinesAsync();
                    ViewBag.Medicines = medicines;
                    return View();
                }

                var inventory = await _inventoryRepository.GetInventoryByMedicineIdAsync(MedicineId);
                if (inventory == null)
                {
                    TempData["ErrorMessage"] = "No inventory found for this medicine.";
                    var medicines = await _medicineRepository.GetAllMedicinesAsync();
                    ViewBag.Medicines = medicines;
                    return View();
                }

                if (inventory.CurrentStock < Quantity)
                {
                    TempData["ErrorMessage"] = $"Insufficient stock. Available: {inventory.CurrentStock}, Required: {Quantity}";
                    var medicines = await _medicineRepository.GetAllMedicinesAsync();
                    ViewBag.Medicines = medicines;
                    return View();
                }

                // Create dispensed medication record
                var dispensedMedication = new DispensedMedication
                {
                    MedicineId = MedicineId,
                    PatientId = PatientId,
                    InventoryId = inventory.InventoryId,
                    Quantity = Quantity,
                    UnitPrice = UnitPrice > 0 ? UnitPrice : medicine.UnitPrice,
                    TotalAmount = TotalAmount > 0 ? TotalAmount : (Quantity * medicine.UnitPrice),
                    Dosage = Dosage,
                    Frequency = Frequency,
                    Duration = Duration,
                    Instructions = Instructions,
                    PaymentMethod = PaymentMethod,
                    Notes = Notes,
                    DispensedDate = DateTime.Now,
                    DispensedBy = 1, // TODO: Get current pharmacist user ID
                    PaymentStatus = "Paid"
                };

                var result = await _dispensedRepository.DispenseMedicationAsync(dispensedMedication);

                if (result > 0)
                {
                    // Update inventory stock
                    await _inventoryRepository.UpdateStockAsync(inventory.InventoryId, inventory.CurrentStock - Quantity);

                    Console.WriteLine($"Dispensed medication with ID: {result}");
                    TempData["SuccessMessage"] = "Medication dispensed successfully!";
                    return RedirectToAction(nameof(DispenseHistory));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to dispense medication.";
                    var medicines = await _medicineRepository.GetAllMedicinesAsync();
                    ViewBag.Medicines = medicines;
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Dispense: {ex.Message}");
                TempData["ErrorMessage"] = $"Error dispensing medication: {ex.Message}";
                var medicines = await _medicineRepository.GetAllMedicinesAsync();
                ViewBag.Medicines = medicines;
                return View();
            }
        }

        // GET: Pharmacy/DispenseHistory
        public async Task<IActionResult> DispenseHistory()
        {
            var dispensedMedications = await _dispensedRepository.GetAllDispensedAsync();
            return View(dispensedMedications);
        }

        // GET: Pharmacy/Reports
        public async Task<IActionResult> Reports()
        {
            var medicines = await _medicineRepository.GetAllMedicinesAsync();
            var lowStock = await _inventoryRepository.GetLowStockItemsAsync();

            ViewBag.Medicines = medicines;
            ViewBag.LowStockItems = lowStock;

            return View();
        }

        // POST: Pharmacy/Report
        [HttpPost]
        public async Task<IActionResult> GenerateReport(int medicineId, DateTime startDate, DateTime endDate)
        {
            var report = await _dispensedRepository.GetMedicineReportAsync(medicineId, startDate, endDate);
            var medicine = await _medicineRepository.GetMedicineByIdAsync(medicineId);

            ViewBag.Medicine = medicine;
            ViewBag.Report = report;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View("MedicineReport");
        }

        // GET: Pharmacy/Search
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

        // GET: Pharmacy/LowStock
        public async Task<IActionResult> LowStock()
        {
            Console.WriteLine("=== LowStock GET Debug ===");

            var lowStockMedicines = await _medicineRepository.GetLowStockMedicinesAsync();
            var lowStockInventory = await _inventoryRepository.GetLowStockItemsAsync();

            Console.WriteLine($"Low stock medicines count: {lowStockMedicines.Count()}");
            Console.WriteLine($"Low stock inventory count: {lowStockInventory.Count()}");

            foreach (var medicine in lowStockMedicines)
            {
                Console.WriteLine($"Low stock medicine: {medicine.MedicineName} ({medicine.MedicineCode})");
            }

            foreach (var inventory in lowStockInventory)
            {
                Console.WriteLine($"Low stock inventory: {inventory.Medicine?.MedicineName} - Stock: {inventory.CurrentStock}, Min: {inventory.MinStockLevel}");
            }

            ViewBag.LowStockMedicines = lowStockMedicines;
            ViewBag.LowStockInventory = lowStockInventory;

            return View();
        }

        // GET: Pharmacy/GetMedicineDetails/5
        [HttpGet]
        public async Task<JsonResult> GetMedicineDetails(int medicineId)
        {
            var medicine = await _medicineRepository.GetMedicineByIdAsync(medicineId);
            if (medicine == null)
            {
                return Json(new { success = false, message = "Medicine not found" });
            }

            var inventory = await _inventoryRepository.GetInventoryByMedicineIdAsync(medicineId);

            return Json(new
            {
                success = true,
                medicine = new
                {
                    medicineId = medicine.MedicineId,
                    medicineName = medicine.MedicineName,
                    unitPrice = medicine.UnitPrice,
                    requiresPrescription = medicine.RequiresPrescription,
                    currentStock = inventory?.CurrentStock ?? 0,
                    sellingPrice = inventory?.SellingPrice ?? medicine.UnitPrice
                }
            });
        }

        // GET: Pharmacy/Prescriptions
        public async Task<IActionResult> Prescriptions()
        {
            try
            {
                var prescriptions = await _prescriptionRepository.GetAllPrescriptionsAsync();
                var pendingPrescriptions = prescriptions.Where(p => p != null && (p.Status == "Pending" || p.Status == null)).ToList();

                ViewBag.PendingCount = pendingPrescriptions.Count();
                ViewBag.TotalCount = prescriptions.Count();

                return View(pendingPrescriptions);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading prescriptions: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Pharmacy/PrescriptionDetails/5
        public async Task<IActionResult> PrescriptionDetails(int id)
        {
            try
            {
                var prescription = await _prescriptionRepository.GetPrescriptionByIdAsync(id);
                if (prescription == null)
                {
                    return NotFound();
                }

                var prescriptionDetails = await _prescriptionRepository.GetPrescriptionDetailsAsync(id);
                var medicines = await _medicineRepository.GetAllMedicinesAsync();

                ViewBag.PrescriptionDetails = prescriptionDetails;
                ViewBag.Medicines = medicines;

                return View(prescription);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading prescription details: {ex.Message}";
                return RedirectToAction(nameof(Prescriptions));
            }
        }

        // GET: Pharmacy/AddInventory
        public async Task<IActionResult> AddInventory()
        {
            Console.WriteLine("AddInventory GET called");
            return View(new PharmacyInventory());
        }

        // POST: Pharmacy/AddInventory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInventory(string MedicineName, string MedicineCode, string GenericName, string Category, int CurrentStock, int MinStockLevel, int MaxStockLevel, decimal CostPrice, decimal SellingPrice, string BatchNumber, string Supplier, DateTime? ExpiryDate)
        {
            Console.WriteLine("=== AddInventory POST Debug ===");
            Console.WriteLine($"Received: {MedicineName}, {MedicineCode}, Stock: {CurrentStock}, Cost: {CostPrice}, Selling: {SellingPrice}");

            try
            {
                // First, check if medicine already exists
                var existingMedicine = await _medicineRepository.GetMedicineByCodeAsync(MedicineCode);

                if (existingMedicine == null)
                {
                    Console.WriteLine("Creating new medicine");
                    // Create new medicine
                    var newMedicine = new Medicine
                    {
                        MedicineCode = MedicineCode,
                        MedicineName = MedicineName,
                        GenericName = GenericName,
                        Category = Category,
                        MedicineType = "Tablet", // Default type
                        UnitPrice = SellingPrice,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };

                    var medicineResult = await _medicineRepository.AddMedicineAsync(newMedicine);
                    if (medicineResult > 0)
                    {
                        Console.WriteLine($"Medicine created with ID: {medicineResult}");
                        existingMedicine = newMedicine;
                        existingMedicine.MedicineId = medicineResult;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to create medicine.";
                        return View(new PharmacyInventory());
                    }
                }
                else
                {
                    Console.WriteLine($"Found existing medicine: {existingMedicine.MedicineId}");
                }

                // Now create or update inventory
                var existingInventory = await _inventoryRepository.GetInventoryByMedicineIdAsync(existingMedicine.MedicineId);

                if (existingInventory != null)
                {
                    Console.WriteLine($"Updating existing inventory: {existingInventory.InventoryId}");
                    existingInventory.CurrentStock += CurrentStock;
                    existingInventory.MinStockLevel = MinStockLevel;
                    existingInventory.MaxStockLevel = MaxStockLevel;
                    existingInventory.SellingPrice = SellingPrice;
                    existingInventory.CostPrice = CostPrice;
                    existingInventory.ExpiryDate = ExpiryDate;
                    existingInventory.BatchNumber = BatchNumber;
                    existingInventory.Supplier = Supplier;
                    existingInventory.UpdatedDate = DateTime.Now;

                    await _inventoryRepository.UpdateInventoryAsync(existingInventory);
                    TempData["SuccessMessage"] = "Inventory updated successfully!";
                }
                else
                {
                    Console.WriteLine("Creating new inventory item");
                    var newInventory = new PharmacyInventory
                    {
                        MedicineId = existingMedicine.MedicineId,
                        CurrentStock = CurrentStock,
                        MinStockLevel = MinStockLevel,
                        MaxStockLevel = MaxStockLevel,
                        CostPrice = CostPrice,
                        SellingPrice = SellingPrice,
                        BatchNumber = BatchNumber,
                        Supplier = Supplier,
                        ExpiryDate = ExpiryDate,
                        CreatedDate = DateTime.Now
                    };

                    var result = await _inventoryRepository.AddInventoryItemAsync(newInventory);

                    Console.WriteLine($"AddInventoryItemAsync result: {result}");
                    if (result > 0)
                    {
                        TempData["SuccessMessage"] = "Medicine added to inventory successfully!";
                        return RedirectToAction(nameof(Inventory));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to add inventory item.";
                        return View(new PharmacyInventory());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return View(new PharmacyInventory());
        }

        // GET: Pharmacy/EditInventory/5
        public async Task<IActionResult> EditInventory(int id)
        {
            var inventory = await _inventoryRepository.GetInventoryByIdAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            var medicine = await _medicineRepository.GetMedicineByIdAsync(inventory.MedicineId);
            ViewBag.Medicine = medicine;

            return View(inventory);
        }

        // POST: Pharmacy/EditInventory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInventory(int id, PharmacyInventory inventory)
        {
            if (id != inventory.InventoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    inventory.UpdatedDate = DateTime.Now;
                    var result = await _inventoryRepository.UpdateInventoryAsync(inventory);

                    if (result)
                    {
                        TempData["SuccessMessage"] = "Inventory updated successfully!";
                        return RedirectToAction(nameof(Inventory));
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error updating inventory: {ex.Message}";
                }
            }

            var medicine = await _medicineRepository.GetMedicineByIdAsync(inventory.MedicineId);
            ViewBag.Medicine = medicine;
            return View(inventory);
        }

        // POST: Pharmacy/UpdateStock
        [HttpPost]
        public async Task<JsonResult> UpdateStock(int inventoryId, int newStock)
        {
            try
            {
                var result = await _inventoryRepository.UpdateStockAsync(inventoryId, newStock);
                if (result)
                {
                    var inventory = await _inventoryRepository.GetInventoryByIdAsync(inventoryId);
                    return Json(new
                    {
                        success = true,
                        message = "Stock updated successfully!",
                        currentStock = inventory?.CurrentStock ?? 0
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update stock." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Pharmacy/FulfillPrescription/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FulfillPrescription(int prescriptionId)
        {
            var prescription = await _prescriptionRepository.GetPrescriptionByIdAsync(prescriptionId);
            if (prescription == null)
            {
                TempData["ErrorMessage"] = "Prescription not found.";
                return RedirectToAction(nameof(Prescriptions));
            }

            try
            {
                prescription.Status = "Fulfilled";
                prescription.FulfilledDate = DateTime.Now;

                var updateResult = await _prescriptionRepository.UpdatePrescriptionAsync(prescription);
                if (updateResult)
                {
                    // Get prescription details and create dispensed medications
                    var prescriptionDetails = await _prescriptionRepository.GetPrescriptionDetailsAsync(prescriptionId);
                    if (prescriptionDetails != null && prescriptionDetails.Any())
                    {
                        foreach (var detail in prescriptionDetails)
                        {
                            // Get inventory for this medicine
                            var inventory = await _inventoryRepository.GetInventoryByMedicineIdAsync(detail.MedicineId);
                            if (inventory == null)
                            {
                                TempData["ErrorMessage"] = $"No inventory found for medicine {detail.MedicineName ?? detail.MedicineId.ToString()}.";
                                return RedirectToAction(nameof(Prescriptions));
                            }

                            if (inventory.CurrentStock < detail.Quantity)
                            {
                                TempData["ErrorMessage"] = $"Insufficient stock for {detail.MedicineName ?? detail.MedicineId.ToString()}. Available: {inventory.CurrentStock}, Required: {detail.Quantity}";
                                return RedirectToAction(nameof(Prescriptions));
                            }

                            var dispensedMedication = new DispensedMedication
                            {
                                PatientId = prescription.PatientId,
                                MedicineId = detail.MedicineId,
                                InventoryId = inventory.InventoryId,
                                Quantity = detail.Quantity,
                                UnitPrice = detail.Price,
                                TotalAmount = detail.Quantity * detail.Price,
                                DispensedDate = DateTime.Now,
                                Dosage = detail.Dosage,
                                Frequency = detail.Frequency,
                                DoctorId = prescription.DoctorId,
                                DispensedBy = 1, // TODO: Get current pharmacist user ID
                                Notes = $"Fulfilled from prescription #{prescriptionId}"
                            };

                            await _dispensedRepository.DispenseMedicationAsync(dispensedMedication);

                            // Update inventory stock
                            await _inventoryRepository.UpdateStockAsync(inventory.InventoryId, inventory.CurrentStock - detail.Quantity);
                        }
                    }

                    TempData["SuccessMessage"] = "Prescription fulfilled successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error fulfilling prescription. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error fulfilling prescription: {ex.Message}";
            }

            return RedirectToAction(nameof(Prescriptions));
        }
    }
}