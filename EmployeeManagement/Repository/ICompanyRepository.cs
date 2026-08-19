using EmployeeManagement.Models;

namespace EmployeeManagement.Repository
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetAllAsync();
        Task<Company?> GetByIdAsync(Guid id);
        Task<Company?> GetByNameAsync(string name);

        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByNameAsync(string name);

        Task<Company> CreateAsync(Company company);
        Task UpdateAsync(Company company);

    }
}
