using CachingDemo.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CachingDemo.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(w => 
            w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Department
        modelBuilder.Entity<Department>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Department>()
            .Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Department>()
            .Property(d => d.Description)
            .HasMaxLength(500);

        // Configure Employee
        modelBuilder.Entity<Employee>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Employee>()
            .Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Employee>()
            .Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Employee>()
            .Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Employee>()
            .Property(e => e.PhoneNumber)
            .HasMaxLength(20);

        modelBuilder.Entity<Employee>()
            .Property(e => e.Salary)
            .HasColumnType("decimal(18,2)");

        // Configure relationships
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed data
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "IT", Description = "Information Technology" },
            new Department { Id = 2, Name = "HR", Description = "Human Resources" },
            new Department { Id = 3, Name = "Sales", Description = "Sales Department" }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@company.com", PhoneNumber = "123-456-7890", Salary = 50000m, DepartmentId = 1 },
            new Employee { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@company.com", PhoneNumber = "123-456-7891", Salary = 60000m, DepartmentId = 1 },
            new Employee { Id = 3, FirstName = "Mike", LastName = "Johnson", Email = "mike.johnson@company.com", PhoneNumber = "123-456-7892", Salary = 55000m, DepartmentId = 2 }
        );
    }
}
