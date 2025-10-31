using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public class PharmacyInventoryRepository : IPharmacyInventoryRepository
    {
        private static List<PharmacyInventory> _inventory = new List<PharmacyInventory>();
        private static int _nextInventoryId = 1;

        public PharmacyInventoryRepository()
        {
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            if (!_inventory.Any())
            {
                _inventory.AddRange(new[]
                {
                    new PharmacyInventory
                    {
                        InventoryId = _nextInventoryId++,
                        MedicineId = 1,
                        CurrentStock = 100,
                        MinStockLevel = 10,
                        MaxStockLevel = 500,
                        ExpiryDate = DateTime.Now.AddYears(2),
                        BatchNumber = "B001",
                        Supplier = "Pharma Corp",
                        CostPrice = 3.50m,
                        SellingPrice = 5.00m,
                        CreatedDate = DateTime.Now
                    },
                    new PharmacyInventory
                    {
                        InventoryId = _nextInventoryId++,
                        MedicineId = 2,
                        CurrentStock = 50,
                        MinStockLevel = 10,
                        MaxStockLevel = 200,
                        ExpiryDate = DateTime.Now.AddYears(1),
                        BatchNumber = "B002",
                        Supplier = "Medi Labs",
                        CostPrice = 12.00m,
                        SellingPrice = 15.00m,
                        CreatedDate = DateTime.Now
                    },
                    new PharmacyInventory
                    {
                        InventoryId = _nextInventoryId++,
                        MedicineId = 3,
                        CurrentStock = 25,
                        MinStockLevel = 15,
                        MaxStockLevel = 100,
                        ExpiryDate = DateTime.Now.AddYears(3),
                        BatchNumber = "B003",
                        Supplier = "Health Pharma",
                        CostPrice = 6.00m,
                        SellingPrice = 8.00m,
                        CreatedDate = DateTime.Now
                    }
                });
            }
        }

        public async Task<int> AddInventoryItemAsync(PharmacyInventory inventory)
        {
            Console.WriteLine("=== Repository AddInventoryItemAsync ===");
            Console.WriteLine($"Adding inventory: MedicineId={inventory.MedicineId}, Stock={inventory.CurrentStock}, Cost={inventory.CostPrice}, Selling={inventory.SellingPrice}");

            inventory.InventoryId = _nextInventoryId++;
            inventory.CreatedDate = DateTime.Now;
            _inventory.Add(inventory);

            Console.WriteLine($"Added inventory with ID: {inventory.InventoryId}");
            Console.WriteLine($"Total inventory items now: {_inventory.Count}");

            return await Task.FromResult(inventory.InventoryId);
        }

        public async Task<IEnumerable<PharmacyInventory>> GetAllInventoryAsync()
        {
            Console.WriteLine("=== Repository GetAllInventoryAsync ===");
            Console.WriteLine($"Returning {_inventory.Count} inventory items");

            // Load Medicine navigation properties for each inventory item
            foreach (var inventory in _inventory)
            {
                if (inventory.Medicine == null)
                {
                    var medicineRepository = new MedicineRepository();
                    inventory.Medicine = await medicineRepository.GetMedicineByIdAsync(inventory.MedicineId);
                    Console.WriteLine($"Loaded medicine for inventory {inventory.InventoryId}: {inventory.Medicine?.MedicineName}");
                }
            }
            return await Task.FromResult(_inventory.AsEnumerable());
        }

        public async Task<PharmacyInventory?> GetInventoryByIdAsync(int id)
        {
            var inventory = _inventory.FirstOrDefault(i => i.InventoryId == id);
            if (inventory != null && inventory.Medicine == null)
            {
                var medicineRepository = new MedicineRepository();
                inventory.Medicine = await medicineRepository.GetMedicineByIdAsync(inventory.MedicineId);
            }
            return await Task.FromResult(inventory);
        }

        public async Task<PharmacyInventory?> GetInventoryByMedicineIdAsync(int medicineId)
        {
            var inventory = _inventory.FirstOrDefault(i => i.MedicineId == medicineId);
            if (inventory != null && inventory.Medicine == null)
            {
                var medicineRepository = new MedicineRepository();
                inventory.Medicine = await medicineRepository.GetMedicineByIdAsync(inventory.MedicineId);
            }
            return await Task.FromResult(inventory);
        }

        public async Task<bool> UpdateInventoryAsync(PharmacyInventory inventory)
        {
            var existingInventory = _inventory.FirstOrDefault(i => i.InventoryId == inventory.InventoryId);
            if (existingInventory != null)
            {
                existingInventory.CurrentStock = inventory.CurrentStock;
                existingInventory.MinStockLevel = inventory.MinStockLevel;
                existingInventory.MaxStockLevel = inventory.MaxStockLevel;
                existingInventory.ExpiryDate = inventory.ExpiryDate;
                existingInventory.BatchNumber = inventory.BatchNumber;
                existingInventory.Supplier = inventory.Supplier;
                existingInventory.CostPrice = inventory.CostPrice;
                existingInventory.SellingPrice = inventory.SellingPrice;
                existingInventory.UpdatedDate = DateTime.Now;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> DeleteInventoryAsync(int id)
        {
            var inventory = _inventory.FirstOrDefault(i => i.InventoryId == id);
            if (inventory != null)
            {
                _inventory.Remove(inventory);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> UpdateStockAsync(int inventoryId, int newStock)
        {
            var inventory = _inventory.FirstOrDefault(i => i.InventoryId == inventoryId);
            if (inventory != null)
            {
                inventory.CurrentStock = newStock;
                inventory.UpdatedDate = DateTime.Now;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<IEnumerable<PharmacyInventory>> GetLowStockItemsAsync()
        {
            var lowStockItems = _inventory.Where(i => i.CurrentStock <= i.MinStockLevel);
            // Load Medicine navigation properties for each low stock item
            foreach (var inventory in lowStockItems)
            {
                if (inventory.Medicine == null)
                {
                    var medicineRepository = new MedicineRepository();
                    inventory.Medicine = await medicineRepository.GetMedicineByIdAsync(inventory.MedicineId);
                }
            }
            return await Task.FromResult(lowStockItems.AsEnumerable());
        }

        public async Task<IEnumerable<PharmacyInventory>> GetExpiredItemsAsync()
        {
            var expiredItems = _inventory.Where(i =>
                i.ExpiryDate != null &&
                i.ExpiryDate is DateTime &&
                ((DateTime)i.ExpiryDate) < DateTime.Now);

            // Load Medicine navigation properties for each expired item
            foreach (var inventory in expiredItems)
            {
                if (inventory.Medicine == null)
                {
                    var medicineRepository = new MedicineRepository();
                    inventory.Medicine = await medicineRepository.GetMedicineByIdAsync(inventory.MedicineId);
                }
            }
            return await Task.FromResult(expiredItems.AsEnumerable());
        }
    }
}
