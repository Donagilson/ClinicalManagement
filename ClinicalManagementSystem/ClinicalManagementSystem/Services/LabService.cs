using ClinicalManagementSystem2025.Models;
using ClinicalManagementSystem2025.Repository;

namespace ClinicalManagementSystem2025.Services
{
    public class LabService : ILabService
    {
        private readonly ILabRepository _labRepository;

        public LabService(ILabRepository labRepository)
        {
            _labRepository = labRepository;
        }

        public async Task<LabTest> GetLabTestByIdAsync(int id)
        {
            return await _labRepository.GetLabTestByIdAsync(id);
        }

        public async Task<IEnumerable<LabTest>> GetAllLabTestsAsync()
        {
            return await _labRepository.GetAllLabTestsAsync();
        }

        public async Task<LabTest> CreateLabTestAsync(LabTest labTest)
        {
            return await _labRepository.AddLabTestAsync(labTest);
        }

        public async Task<LabTest> UpdateLabTestAsync(int id, LabTest labTest)
        {
            var existingTest = await _labRepository.GetLabTestByIdAsync(id);
            if (existingTest == null)
            {
                throw new Exception("Lab test not found");
            }

            existingTest.TestName = labTest.TestName;
            existingTest.Description = labTest.Description;
            existingTest.Price = labTest.Price;
            existingTest.NormalRange = labTest.NormalRange;
            existingTest.Unit = labTest.Unit;
            existingTest.DepartmentId = labTest.DepartmentId;

            return await _labRepository.UpdateLabTestAsync(existingTest);
        }

        public async Task<bool> DeleteLabTestAsync(int id)
        {
            return await _labRepository.DeleteLabTestAsync(id);
        }

        public async Task<LabReport> GetLabReportByIdAsync(int id)
        {
            return await _labRepository.GetLabReportByIdAsync(id);
        }

        public async Task<IEnumerable<LabReport>> GetAllLabReportsAsync()
        {
            return await _labRepository.GetAllLabReportsAsync();
        }

        public async Task<IEnumerable<LabReport>> GetLabReportsByPatientAsync(int patientId)
        {
            return await _labRepository.GetLabReportsByPatientAsync(patientId);
        }

        public async Task<IEnumerable<LabReport>> GetPendingLabReportsAsync()
        {
            return await _labRepository.GetPendingLabReportsAsync();
        }

        public async Task<LabReport> CreateLabReportAsync(LabReport labReport)
        {
            return await _labRepository.AddLabReportAsync(labReport);
        }

        public async Task<LabReport> UpdateLabReportAsync(int id, LabReport labReport)
        {
            var existingReport = await _labRepository.GetLabReportByIdAsync(id);
            if (existingReport == null)
            {
                throw new Exception("Lab report not found");
            }

            existingReport.ResultValue = labReport.ResultValue;
            existingReport.ResultUnit = labReport.ResultUnit;
            existingReport.NormalRange = labReport.NormalRange;
            existingReport.Findings = labReport.Findings;
            existingReport.Status = labReport.Status;
            existingReport.Notes = labReport.Notes;
            existingReport.ReportDate = labReport.ReportDate;

            return await _labRepository.UpdateLabReportAsync(existingReport);
        }

        public async Task<bool> DeleteLabReportAsync(int id)
        {
            return await _labRepository.DeleteLabReportAsync(id);
        }
    }
}