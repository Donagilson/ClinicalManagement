using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public interface IPharmacyInventoryRepository
    {
        Task<int> AddInventoryItemAsync(PharmacyInventory inventory);
        Task<IEnumerable<PharmacyInventory>> GetAllInventoryAsync();
        Task<PharmacyInventory?> GetInventoryByIdAsync(int id);
        Task<PharmacyInventory?> GetInventoryByMedicineIdAsync(int medicineId);
        Task<bool> UpdateInventoryAsync(PharmacyInventory inventory);
        Task<bool> DeleteInventoryAsync(int id);
        Task<bool> UpdateStockAsync(int inventoryId, int newStock);
        Task<IEnumerable<PharmacyInventory>> GetLowStockItemsAsync();
        Task<IEnumerable<PharmacyInventory>> GetExpiredItemsAsync();
    }

    public interface IDispensedMedicationRepository
    {
        Task<int> DispenseMedicationAsync(DispensedMedication dispensed);
        Task<IEnumerable<DispensedMedication>> GetAllDispensedAsync();
        Task<IEnumerable<DispensedMedication>> GetDispensedByPatientIdAsync(int patientId);
        Task<IEnumerable<DispensedMedication>> GetDispensedByMedicineIdAsync(int medicineId);
        Task<DispensedMedication?> GetDispensedByIdAsync(int id);
        Task<bool> UpdateDispensedAsync(DispensedMedication dispensed);
        Task<bool> DeleteDispensedAsync(int id);
        Task<IEnumerable<DispensedMedication>> GetDispensedByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<PharmacyReport> GetMedicineReportAsync(int medicineId, DateTime startDate, DateTime endDate);
    }
}
