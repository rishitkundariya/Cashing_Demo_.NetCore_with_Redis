using CachingDemo.Application.Interfaces;
using CachingDemo.Application.DTOs;
using CachingDemo.Domain.Models;
using CachingDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CachingDemo.Infrastructure.Repositories;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext context) : base(context)
    {
    }

}
