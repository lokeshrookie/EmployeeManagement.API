namespace EmployeeManagement.API.Models
{
    public class UpdateEmployeeDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Designation { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
