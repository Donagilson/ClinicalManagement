namespace ClinicalManagementSystem2025.Models
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public decimal ConsultationFee { get; set; }
        public TimeSpan AvailableFrom { get; set; }
        public TimeSpan AvailableTo { get; set; }
    }
}