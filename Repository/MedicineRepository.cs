using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public class MedicineRepository : IMedicineRepository
    {
        private static List<Medicine> _medicines = new List<Medicine>();
        private static int _nextMedicineId = 1;

        public MedicineRepository()
        {
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            if (!_medicines.Any())
            {
                _medicines.AddRange(new[]
                {
                    new Medicine
                    {
                        MedicineId = _nextMedicineId++,
                        MedicineCode = "MED001",
                        MedicineName = "Paracetamol",
                        GenericName = "Acetaminophen",
                        Manufacturer = "Pharma Corp",
                        Category = "Analgesic",
                        MedicineType = "Tablet",
                        DosageForm = "Oral",
                        Strength = "500mg",
                        Description = "Pain relief and fever reducer",
                        UnitPrice = 5.00m,
                        StockQuantity = 100,
                        MinimumStockLevel = 10,
                        ExpiryDate = DateTime.Now.AddYears(2),
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Medicine
                    {
                        MedicineId = _nextMedicineId++,
                        MedicineCode = "MED002",
                        MedicineName = "Amoxicillin",
                        GenericName = "Amoxicillin",
                        Manufacturer = "Medi Labs",
                        Category = "Antibiotic",
                        MedicineType = "Capsule",
                        DosageForm = "Oral",
                        Strength = "250mg",
                        Description = "Antibiotic for bacterial infections",
                        UnitPrice = 15.00m,
                        StockQuantity = 50,
                        MinimumStockLevel = 10,
                        ExpiryDate = DateTime.Now.AddYears(1),
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Medicine
                    {
                        MedicineId = _nextMedicineId++,
                        MedicineCode = "MED003",
                        MedicineName = "Ibuprofen",
                        GenericName = "Ibuprofen",
                        Manufacturer = "Health Pharma",
                        Category = "Anti-inflammatory",
                        MedicineType = "Tablet",
                        DosageForm = "Oral",
                        Strength = "400mg",
                        Description = "Anti-inflammatory and pain relief",
                        UnitPrice = 8.00m,
                        StockQuantity = 25,
                        MinimumStockLevel = 15,
                        ExpiryDate = DateTime.Now.AddYears(3),
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    }
                });
            }
        }

        public async Task<Medicine?> GetMedicineByIdAsync(int id)
        {
            var medicine = _medicines.FirstOrDefault(m => m.MedicineId == id && m.IsActive);
            return await Task.FromResult(medicine);
        }

        public async Task<Medicine?> GetMedicineByCodeAsync(string medicineCode)
        {
            var medicine = _medicines.FirstOrDefault(m => m.MedicineCode == medicineCode && m.IsActive);
            return await Task.FromResult(medicine);
        }

        public async Task<IEnumerable<Medicine>> GetAllMedicinesAsync()
        {
            return await Task.FromResult(_medicines.Where(m => m.IsActive).AsEnumerable());
        }

        public async Task<IEnumerable<Medicine>> SearchMedicinesAsync(string searchTerm)
        {
            var medicines = _medicines.Where(m => m.IsActive && (
                m.MedicineName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.GenericName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.MedicineCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.Manufacturer.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ));
            return await Task.FromResult(medicines.AsEnumerable());
        }

        public async Task<IEnumerable<Medicine>> GetLowStockMedicinesAsync()
        {
            // Get medicines that have inventory with low stock
            var lowStockMedicines = new List<Medicine>();

            foreach (var medicine in _medicines.Where(m => m.IsActive))
            {
                // Check if this medicine has inventory
                var inventoryRepository = new PharmacyInventoryRepository();
                var inventory = await inventoryRepository.GetInventoryByMedicineIdAsync(medicine.MedicineId);

                if (inventory != null && inventory.CurrentStock <= inventory.MinStockLevel)
                {
                    lowStockMedicines.Add(medicine);
                }
            }

            return await Task.FromResult(lowStockMedicines.AsEnumerable());
        }

        public async Task<int> AddMedicineAsync(Medicine medicine)
        {
            medicine.MedicineId = _nextMedicineId++;
            medicine.CreatedDate = DateTime.Now;
            medicine.IsActive = true;
            _medicines.Add(medicine);
            return await Task.FromResult(medicine.MedicineId);
        }

        public async Task<Medicine?> UpdateMedicineAsync(Medicine medicine)
        {
            var existingMedicine = _medicines.FirstOrDefault(m => m.MedicineId == medicine.MedicineId);
            if (existingMedicine != null)
            {
                existingMedicine.MedicineCode = medicine.MedicineCode;
                existingMedicine.MedicineName = medicine.MedicineName;
                existingMedicine.GenericName = medicine.GenericName;
                existingMedicine.Manufacturer = medicine.Manufacturer;
                existingMedicine.MedicineType = medicine.MedicineType;
                existingMedicine.DosageForm = medicine.DosageForm;
                existingMedicine.Strength = medicine.Strength;
                existingMedicine.Description = medicine.Description;
                existingMedicine.Category = medicine.Category;
                existingMedicine.UnitPrice = medicine.UnitPrice;
                existingMedicine.StockQuantity = medicine.StockQuantity;
                existingMedicine.MinimumStockLevel = medicine.MinimumStockLevel;
                existingMedicine.ExpiryDate = medicine.ExpiryDate;
                existingMedicine.RequiresPrescription = medicine.RequiresPrescription;
                existingMedicine.UpdatedDate = DateTime.Now;
                existingMedicine.UpdatedBy = medicine.UpdatedBy;
                return await Task.FromResult(existingMedicine);
            }
            return await Task.FromResult<Medicine?>(null);
        }

        public async Task<bool> DeleteMedicineAsync(int id)
        {
            var medicine = _medicines.FirstOrDefault(m => m.MedicineId == id);
            if (medicine != null)
            {
                medicine.IsActive = false;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<IEnumerable<Medicine>> GetMedicinesByCategoryAsync(string category)
        {
            var medicines = _medicines.Where(m => m.IsActive && m.Category == category);
            return await Task.FromResult(medicines.AsEnumerable());
        }

        public async Task<bool> MedicineExistsAsync(string medicineCode, string medicineName)
        {
            return await Task.FromResult(_medicines.Any(m =>
                (m.MedicineCode == medicineCode || m.MedicineName == medicineName) && m.IsActive));
        }

        public async Task UpdateStockAsync(int medicineId, int quantity)
        {
            var medicine = _medicines.FirstOrDefault(m => m.MedicineId == medicineId);
            if (medicine != null)
            {
                // This would update stock in a real implementation
                // For now, just a placeholder
            }
            await Task.CompletedTask;
        }
    }
}