using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company> CreateCompanyAsync(Company company)
        {
            var result = await _context.Companies.AddAsync(company);
            _context.SaveChanges();
            return company;
        }

        public async Task<bool> ExistCompanyAsync(Guid id)
        {
            return await _context.Companies
                .AnyAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByCompanyNameAsync(string name)
        {
            return await _context.Companies.AnyAsync(c => c.Name == name);
        }

        public async Task<List<Company>> GetAllCompanyAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company?> GetCompanyByIdAsync(Guid id)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Company?> GetCompanyByNameAsync(string name)
        {
            return await _context.Companies
                .Include(c => c.Employees)
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            await _context.SaveChangesAsync();
        }
    }
}
