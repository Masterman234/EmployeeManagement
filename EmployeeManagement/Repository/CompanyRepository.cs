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

        public async Task<Company> CreateAsync(Company company)
        {
            var result = await _context.Companies.AddAsync(company);
            _context.SaveChanges();
            return company;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Companies
                .AnyAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Companies.AnyAsync(c => c.Name == name);
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company?> GetByIdAsync(Guid id)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Company?> GetByNameAsync(string name)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task UpdateAsync(Company company)
        {
            await _context.SaveChangesAsync();
        }
    }
}
