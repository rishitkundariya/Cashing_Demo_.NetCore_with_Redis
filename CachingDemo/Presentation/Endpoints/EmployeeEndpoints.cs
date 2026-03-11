using CachingDemo.Application.DTOs;
using CachingDemo.Application.Interfaces;
using CachingDemo.Domain.Models;

namespace CachingDemo.Presentation.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/employees")
            .WithTags("Employees");

        group.MapGet("/", GetAllEmployees)
            .WithName("GetAllEmployees");

        group.MapGet("/{id}", GetEmployeeById)
            .WithName("GetEmployeeById");

        group.MapGet("/by-department/{departmentId}", GetEmployeesByDepartment)
            .WithName("GetEmployeesByDepartment");

        group.MapPost("/", CreateEmployee)
            .WithName("CreateEmployee");

        group.MapPut("/{id}", UpdateEmployee)
            .WithName("UpdateEmployee");

        group.MapDelete("/{id}", DeleteEmployee)
            .WithName("DeleteEmployee");
    }

    private static async Task<IResult> GetAllEmployees(IEmployeeRepository repository)
    {
        var employees = await repository.GetList(e => new {
            e.Id,
            e.FirstName,
            e.LastName,
            e.Email,
            e.PhoneNumber,
            e.Salary,
            e.DepartmentId,
            e.HireDate,
            e.CreatedAt
        });        
        return Results.Ok(employees);
    }

    private static async Task<IResult> GetEmployeeById(int id, IEmployeeRepository repository)
    {
        var employee = await repository.GetFirstOrDefault(e => new EmployeeDTO{
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            PhoneNumber = e.PhoneNumber,
            Salary = e.Salary,
            DepartmentName = e.Department.Name ?? "Not Specified"
        },w=> w.Id == id);

        if (employee == null)
            return Results.NotFound("Employee not found");
        else
            return Results.Ok(employee);
    }

    private static async Task<IResult> GetEmployeesByDepartment(int departmentId, IEmployeeRepository repository)
    {
        var employees = await repository.GetList(s=> new EmployeeDTO{
        Id = s.Id,
        FirstName = s.FirstName,
        LastName = s.LastName,
        Email = s.Email,
        PhoneNumber = s.PhoneNumber,
        Salary = s.Salary,
        DepartmentName = s.Department.Name ?? "Not Specified"
       }, s=> s.Id == departmentId);
        return Results.Ok(employees);
    }

    private static async Task<IResult> CreateEmployee(CreateEmployeeRequest request, IEmployeeRepository repository)
    {
        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber ?? "",
            Salary = request.Salary,
            DepartmentId = request.DepartmentId,
            HireDate = DateTime.UtcNow
        };

        var newEmp = await repository.AddAsync(employee);
        if (!await repository.SaveChangesAsync())
            return Results.BadRequest("Failed to create employee");

        return Results.Created($"/api/employees/{newEmp.Id}", newEmp);
    }

    private static async Task<IResult> UpdateEmployee(int id, UpdateEmployeeRequest request, IEmployeeRepository repository)
    {
        var employee = await repository.GetByIdAsync(id);
        if (employee == null)
            return Results.NotFound("Employee not found");

        if (!string.IsNullOrEmpty(request.FirstName))
            employee.FirstName = request.FirstName;
        if (!string.IsNullOrEmpty(request.LastName))
            employee.LastName = request.LastName;
        if (!string.IsNullOrEmpty(request.Email))
            employee.Email = request.Email;
        if (!string.IsNullOrEmpty(request.PhoneNumber))
            employee.PhoneNumber = request.PhoneNumber;
        if (request.Salary.HasValue)
            employee.Salary = request.Salary.Value;
        if (request.DepartmentId.HasValue)
            employee.DepartmentId = request.DepartmentId.Value;

        employee.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(employee);
        if (!await repository.SaveChangesAsync())
            return Results.BadRequest("Failed to update employee");

        return Results.Ok(employee);
    }

    private static async Task<IResult> DeleteEmployee(int id, IEmployeeRepository repository)
    {
        var employee = await repository.GetByIdAsync(id);
        if (employee == null)
            return Results.NotFound("Employee not found");

        await repository.DeleteAsync(employee);
        if (!await repository.SaveChangesAsync())
            return Results.BadRequest("Failed to delete employee");

        return Results.Ok("Employee deleted successfully");
    }
}
