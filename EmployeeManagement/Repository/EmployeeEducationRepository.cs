using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repository;

public class EmployeeEducationRepository(ApplicationDbContext context) : IEmployeeEducationRepository
{
    public async Task<EmployeeEducation> CreateEmployeeEducationAsync(EmployeeEducation employeeEducation)
    {
        await context.EmployeeEducations.AddAsync(employeeEducation);
        await context.SaveChangesAsync();

        return employeeEducation;
    }

    public async Task<IEnumerable<EmployeeEducation>> CreateEmployeeEducationHistoryAsync(
        IEnumerable<EmployeeEducation> educations)
    {
        await context.EmployeeEducations.AddRangeAsync(educations);

        await context.SaveChangesAsync();

        return educations;
    }

    public async Task<EmployeeEducation?> GetEmployeeEducationByIdAsync(Guid id)
    {
        return await context.EmployeeEducations
            .Include(ee => ee.Qualifications)
            .FirstOrDefaultAsync(ee => ee.Id == id);
    }

    public async Task<IEnumerable<EmployeeEducation>> GetAllEmployeeEducationsAsync()
    {
        return await context.EmployeeEducations
            .Include(ee => ee.Qualifications)
            .ToListAsync();
    }

    public async Task UpdateEmployeeEducationAsync(EmployeeEducation employeeEducation)
    {
        context.EmployeeEducations.Update(employeeEducation);

        await context.SaveChangesAsync();
    }

    public async Task DeleteEmployeeEducationAsync(Guid id)
    {
        var employeeEducation = await context.EmployeeEducations
            .FirstOrDefaultAsync(ee => ee.Id == id);

        if (employeeEducation != null)
        {
            context.EmployeeEducations.Remove(employeeEducation);

            await context.SaveChangesAsync();
        }
    }

    public async Task<(List<EmployeeEducation> Items, int TotalCount)> GetPagedEmployeeEducationsAsync(int pageNumber, int pageSize)
    {
        var query = context.EmployeeEducations
            .Include(e => e.Qualifications)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}