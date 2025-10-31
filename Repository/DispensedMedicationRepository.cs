using ClinicalManagementSystem2025.Models;

namespace ClinicalManagementSystem2025.Repository
{
    public class DispensedMedicationRepository : IDispensedMedicationRepository
    {
        private static List<DispensedMedication> _dispensedMedications = new List<DispensedMedication>();
        private static int _nextDispensedId = 1;

        public DispensedMedicationRepository()
        {
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            if (!_dispensedMedications.Any())
            {
                _dispensedMedications.AddRange(new[]
                {
                    new DispensedMedication
                    {
                        DispensedId = _nextDispensedId++,
                        MedicineId = 1,
                        PatientId = 1,
                        InventoryId = 1,
                        PrescriptionId = 1,
                        DoctorId = 1,
                        Quantity = 20,
                        UnitPrice = 5.00m,
                        TotalAmount = 100.00m,
                        Instructions = "Take 1 tablet every 6 hours as needed",
                        Dosage = "1 tablet",
                        Frequency = "Every 6 hours",
                        Duration = "5 days",
                        DispensedBy = 1,
                        DispensedDate = DateTime.Now.AddDays(-1),
                        PaymentStatus = "Paid",
                        PaymentMethod = "Cash"
                    },
                    new DispensedMedication
                    {
                        DispensedId = _nextDispensedId++,
                        MedicineId = 2,
                        PatientId = 2,
                        InventoryId = 2,
                        PrescriptionId = 2,
                        DoctorId = 2,
                        Quantity = 15,
                        UnitPrice = 15.00m,
                        TotalAmount = 225.00m,
                        Instructions = "Take 1 capsule 3 times daily with food",
                        Dosage = "1 capsule",
                        Frequency = "3 times daily",
                        Duration = "7 days",
                        DispensedBy = 1,
                        DispensedDate = DateTime.Now.AddHours(-2),
                        PaymentStatus = "Paid",
                        PaymentMethod = "Card"
                    }
                });
            }
        }

        public async Task<int> DispenseMedicationAsync(DispensedMedication dispensed)
        {
            dispensed.DispensedId = _nextDispensedId++;
            dispensed.DispensedDate = DateTime.Now;
            _dispensedMedications.Add(dispensed);
            return await Task.FromResult(dispensed.DispensedId);
        }

        public async Task<IEnumerable<DispensedMedication>> GetAllDispensedAsync()
        {
            return await Task.FromResult(_dispensedMedications.AsEnumerable());
        }

        public async Task<IEnumerable<DispensedMedication>> GetDispensedByPatientIdAsync(int patientId)
        {
            var dispensed = _dispensedMedications.Where(d => d.PatientId == patientId);
            return await Task.FromResult(dispensed.AsEnumerable());
        }

        public async Task<IEnumerable<DispensedMedication>> GetDispensedByMedicineIdAsync(int medicineId)
        {
            var dispensed = _dispensedMedications.Where(d => d.MedicineId == medicineId);
            return await Task.FromResult(dispensed.AsEnumerable());
        }

        public async Task<DispensedMedication?> GetDispensedByIdAsync(int id)
        {
            var dispensed = _dispensedMedications.FirstOrDefault(d => d.DispensedId == id);
            return await Task.FromResult(dispensed);
        }

        public async Task<bool> UpdateDispensedAsync(DispensedMedication dispensed)
        {
            var existingDispensed = _dispensedMedications.FirstOrDefault(d => d.DispensedId == dispensed.DispensedId);
            if (existingDispensed != null)
            {
                existingDispensed.Quantity = dispensed.Quantity;
                existingDispensed.UnitPrice = dispensed.UnitPrice;
                existingDispensed.TotalAmount = dispensed.TotalAmount;
                existingDispensed.Instructions = dispensed.Instructions;
                existingDispensed.Dosage = dispensed.Dosage;
                existingDispensed.Frequency = dispensed.Frequency;
                existingDispensed.Duration = dispensed.Duration;
                existingDispensed.PaymentStatus = dispensed.PaymentStatus;
                existingDispensed.PaymentMethod = dispensed.PaymentMethod;
                existingDispensed.Notes = dispensed.Notes;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> DeleteDispensedAsync(int id)
        {
            var dispensed = _dispensedMedications.FirstOrDefault(d => d.DispensedId == id);
            if (dispensed != null)
            {
                _dispensedMedications.Remove(dispensed);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<IEnumerable<DispensedMedication>> GetDispensedByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var dispensed = _dispensedMedications.Where(d =>
                d.DispensedDate.Date >= startDate.Date &&
                d.DispensedDate.Date <= endDate.Date);
            return await Task.FromResult(dispensed.AsEnumerable());
        }

        public async Task<PharmacyReport> GetMedicineReportAsync(int medicineId, DateTime startDate, DateTime endDate)
        {
            var dispensedItems = _dispensedMedications.Where(d =>
                d.MedicineId == medicineId &&
                d.DispensedDate.Date >= startDate.Date &&
                d.DispensedDate.Date <= endDate.Date);

            var report = new PharmacyReport
            {
                MedicineId = medicineId,
                TotalDispensed = dispensedItems.Sum(d => d.Quantity),
                TotalRevenue = dispensedItems.Sum(d => d.TotalAmount),
                LowStockCount = 0, // This would need to be calculated based on inventory
                AveragePrice = dispensedItems.Any() ? dispensedItems.Average(d => d.UnitPrice) : 0
            };

            return await Task.FromResult(report);
        }
    }
}
