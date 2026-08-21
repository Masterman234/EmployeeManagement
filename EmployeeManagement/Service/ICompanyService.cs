using EmployeeManagement.Dtos;
using EmployeeManagement.Dtos.Company;

namespace EmployeeManagement.Service
{
    public interface ICompanyService
    {
        Task<BaseResponseModel<CompanyDto>> CreateCompanyAsync(CreateCompanyDto request);
        Task<BaseResponseModel<CompanyDto>> GetCompanyByIdAsync(Guid id);
        Task<BaseResponseModel<IEnumerable<CompanyDto>>> GetAllCompaniesAsync();
        Task<BaseResponseModel<CompanyDto>> UpdateCompanyAsync(CompanyDto request);
        Task<BaseResponseModel<bool>> ActivateCompanyAsync(Guid id);
        Task<BaseResponseModel<bool>> DeActivateCompanyAsync(Guid id);
        Task<BaseResponseModel<CompanyDto>> GetCompanyByNameAsync(string name);

    }
}
