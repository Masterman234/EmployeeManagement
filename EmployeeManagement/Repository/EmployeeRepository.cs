using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repository;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }   
    public async Task<Employee> CreateEmployeeAsync(Employee employee)
    {
        var result = await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<bool> DeleteEmployeeAsync(Guid id)
    {
       var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return false;
        }
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;

    }

    public async Task<Employee> ExistByEmailAsync(string email)
    {
       return await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<Employee> ExistByPhoneAsync(string phone)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.Telephone == phone);
    }

    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        return await _context.Employees.ToListAsync();
    }

    public Task<Employee> GetEmployeeByIdAsync(Guid id)
    {
        return _context.Employees
            .Include(e => e.Company)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> UpdateEmployeeAsync(Employee employee)
    {
       var existingEmployee = await _context.Employees.FindAsync(employee.Id);
        if (existingEmployee == null)
        {
            return false;
        }
        _context.Employees.Update(existingEmployee);
        await _context.SaveChangesAsync();
        return true;
    }
}
