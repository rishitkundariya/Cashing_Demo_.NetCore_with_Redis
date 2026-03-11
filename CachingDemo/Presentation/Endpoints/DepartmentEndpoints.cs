using CachingDemo.Application.DTOs;
using CachingDemo.Application.Interfaces;
using CachingDemo.Domain.Models;

namespace CachingDemo.Presentation.Endpoints;

public static class DepartmentEndpoints
{
    public static void MapDepartmentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/departments")
            .WithTags("Departments");

        group.MapGet("/", GetAllDepartments)
            .WithName("GetAllDepartments");

        group.MapGet("/{id}", GetDepartmentById)
            .WithName("GetDepartmentById");

        group.MapGet("/with-employees", GetDepartmentsWithEmployees)
            .WithName("GetDepartmentsWithEmployees");

        group.MapPost("/", CreateDepartment)
            .WithName("CreateDepartment");

        group.MapPut("/{id}", UpdateDepartment)
            .WithName("UpdateDepartment");

        group.MapDelete("/{id}", DeleteDepartment)
            .WithName("DeleteDepartment");
    }

    private static async Task<IResult> GetAllDepartments(IDepartmentRepository repository)
    {
        var departments = await repository.GetList(e=> new DepartmentDTO{
            Id = e.Id,
            Name = e.Name,
            Description = e.Description
        });
        
        return Results.Ok(departments);
    }

    private static async Task<IResult> GetDepartmentById(int id, IDepartmentRepository repository)
    {
        var department = await repository.GetFirstOrDefault(e=> new DepartmentDTO{
            Id = e.Id,
            Name = e.Name,
            Description = e.Description
        }, e => e.Id == id);
        if (department == null)
            return Results.NotFound("Department not found");
        else
            return Results.Ok(department);
    }

    private static async Task<IResult> GetDepartmentsWithEmployees(IDepartmentRepository repository)
    {
        var departments = await repository.GetList(e => new
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            Employees = e.Employees.Select(emp => new
            {
                emp.Id,
                emp.FirstName,
                emp.LastName,
                emp.Email,
                emp.Salary
            }).ToList(),
            EmployeeCount= e.Employees.Count()
        });
        return Results.Ok(departments);
    }



    private static async Task<IResult> CreateDepartment(CreateDepartmentRequest request, IDepartmentRepository repository)
    {
        var department = new Department
        {
            Name = request.Name,
            Description = request.Description ?? ""
        };

        var newDept = await repository.AddAsync(department);
        if (!await repository.SaveChangesAsync())
            return Results.BadRequest("Failed to create department");

        return Results.Created($"/api/departments/{newDept.Id}", newDept);
    }

    private static async Task<IResult> UpdateDepartment(int id, UpdateDepartmentRequest request, IDepartmentRepository repository)
    {
        var department = await repository.GetByIdAsync(id);
        if (department == null)
            return Results.NotFound("Department not found");

        if (!string.IsNullOrEmpty(request.Name))
            department.Name = request.Name;
        if (request.Description != null)
            department.Description = request.Description;
        
        department.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(department);
        if (!await repository.SaveChangesAsync())
            return Results.BadRequest("Failed to update department");

        return Results.Ok(department);
    }

    private static async Task<IResult> DeleteDepartment(int id, IDepartmentRepository repository)
    {
        var department = await repository.GetByIdAsync(id);
        if (department == null)
            return Results.NotFound("Department not found");

        await repository.DeleteAsync(department);
        if (!await repository.SaveChangesAsync())
            return Results.BadRequest("Failed to delete department");

        return Results.Ok("Department deleted successfully");
    }
}
