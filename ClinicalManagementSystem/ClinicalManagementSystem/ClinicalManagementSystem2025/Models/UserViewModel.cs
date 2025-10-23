
namespace ClinicalManagementSystem2025.Models
{
    internal class UserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public int RoleId { get; set; }
        public int? DepartmentId { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }
    }
}