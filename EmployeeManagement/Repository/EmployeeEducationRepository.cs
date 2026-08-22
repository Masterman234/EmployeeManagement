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
            .FirstOrDefaultAsync(ee => ee.Id == id);
    }

    public async Task<IEnumerable<EmployeeEducation>> GetAllEmployeeEducationsAsync()
    {
        return await context.EmployeeEducations.ToListAsync();
    }

    public async Task UpdateEmployeeEducationAsync(EmployeeEducation employeeEducation)
    {
        context.EmployeeEducations.Update(employeeEducation);
        await context.SaveChangesAsync();
    }

    public async Task DeleteEmployeeEducationAsync(Guid id)
    {
        var employeeEducation = await context.EmployeeEducations.FirstOrDefaultAsync(ee => ee.Id == id);
        if (employeeEducation != null)
        {
            context.EmployeeEducations.Remove(employeeEducation);
            await context.SaveChangesAsync();
        }
    }


}