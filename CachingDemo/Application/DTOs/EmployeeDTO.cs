namespace CachingDemo.Application.DTOs;

public class EmployeeDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
}
