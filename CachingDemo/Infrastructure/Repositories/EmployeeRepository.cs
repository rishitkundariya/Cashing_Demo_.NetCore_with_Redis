using CachingDemo.Application.DTOs;
using CachingDemo.Application.Interfaces;
using CachingDemo.Domain.Models;
using CachingDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CachingDemo.Infrastructure.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context)
    {
    }

}
