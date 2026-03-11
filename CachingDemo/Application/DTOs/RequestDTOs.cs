namespace CachingDemo.Application.DTOs;

public class CreateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateDepartmentRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class CreateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public decimal Salary { get; set; }
    public int DepartmentId { get; set; }
}

public class UpdateEmployeeRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public decimal? Salary { get; set; }
    public int? DepartmentId { get; set; }
}
