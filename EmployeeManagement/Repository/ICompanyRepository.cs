using EmployeeManagement.Models;

namespace EmployeeManagement.Repository
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetAllCompanyAsync();
        Task<Company?> GetCompanyByIdAsync(Guid id);
        Task<Company?> GetCompanyByNameAsync(string name);

        Task<bool> ExistCompanyAsync(Guid id);
        Task<bool> ExistsByCompanyNameAsync(string name);

        Task<Company> CreateCompanyAsync(Company company);
        Task UpdateCompanyAsync(Company company);

    }
}
