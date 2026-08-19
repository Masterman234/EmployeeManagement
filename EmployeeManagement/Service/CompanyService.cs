using EmployeeManagement.Dtos;
using EmployeeManagement.Dtos.Company;
using EmployeeManagement.Models;
using EmployeeManagement.Repository;

namespace EmployeeManagement.Service
{
    public class CompanyService(ICompanyRepository companyRepository) : ICompanyService
    {
        public async Task<BaseResponseModel<bool>> ActivateCompanyAsync(Guid id)
        {

            if (id == Guid.Empty)
            {
                return BaseResponseModel<bool>.FailureResponse("Invalid company ID");
            }
           
            var company = await companyRepository.GetByIdAsync(id);

            if (company == null)
            {
                return BaseResponseModel<bool>.FailureResponse("Company not found");
            }

            if (company.IsActive)
            {
                return BaseResponseModel<bool>.FailureResponse("Company is already active");
            }

            company.IsActive = true;
            company.ModifiedAt = DateTime.UtcNow;

            await companyRepository.UpdateAsync(company);

            return BaseResponseModel<bool>.SuccessResponse(true, "Company activated successfully.");
        }



        public async Task<BaseResponseModel<CompanyDto>> CreateCompanyAsync(CreateCompanyDto request)
        {
            if (request == null)
            {
                return BaseResponseModel<CompanyDto>.FailureResponse("Request cannot be null");
            }

            var existingCompany = await companyRepository.GetByNameAsync(request.Name);
            if (existingCompany != null)
            {
                return BaseResponseModel<CompanyDto>.FailureResponse("Company with the same name already exists");
            }

            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var createdCompany = await companyRepository.CreateAsync(company);
            
            var companyDto   = new CompanyDto
            {
                Id = createdCompany.Id,
                Name = createdCompany.Name,
                IsActive = createdCompany.IsActive
            };
          

            return BaseResponseModel<CompanyDto>.SuccessResponse(companyDto, "Company created successfully.");
        }



        public async Task<BaseResponseModel<bool>> DeActivateCompanyAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BaseResponseModel<bool>.FailureResponse("Invalid company ID");
            }

            var company = await companyRepository.GetByIdAsync(id);


            if (company == null)
            {
                return BaseResponseModel<bool>.FailureResponse("Company not found.");
            }
           
            if (!company.IsActive)
            {
                return BaseResponseModel<bool>.FailureResponse("Company is already deactivated.");
            }

            company.IsActive = false;
            company.ModifiedAt = DateTime.UtcNow;
            await companyRepository.UpdateAsync(company);
            return BaseResponseModel<bool>.SuccessResponse(true, "Company deactivated successfully.");
        }



        public async Task<BaseResponseModel<IEnumerable<CompanyDto>>> GetAllCompaniesAsync()
        {
            var companies = await companyRepository.GetAllAsync();
            var companyDtos = companies.Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                IsActive = c.IsActive
            }).ToList();
            return BaseResponseModel<IEnumerable<CompanyDto>>.SuccessResponse(companyDtos, "Companies retrieved successfully.");
        }



        public async Task<BaseResponseModel<CompanyDto>> GetCompanyByIdAsync(Guid id)
        {
            var company = await companyRepository.GetByIdAsync(id);

            if (company == null)
            {
                return BaseResponseModel<CompanyDto>.FailureResponse("Company not found");
            }

            if (id == Guid.Empty)
            {
                return BaseResponseModel<CompanyDto>.FailureResponse("Invalid company ID");
            }

            var response = new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
            };
            return BaseResponseModel<CompanyDto>.SuccessResponse(response, "Company retrieved successfully");
        }


        public async Task<BaseResponseModel<CompanyDto>> UpdateCompanyAsync(CompanyDto request)
        {
            // var company = await companyRepository.GetByIdAsync(request.Id);

            if (request == null)
            {
                return BaseResponseModel<CompanyDto>.FailureResponse("Company not found");
            }

            if (request.Id == Guid.Empty)
            {
                return BaseResponseModel<CompanyDto>.FailureResponse("Invalid company ID");
            }

            var company = await companyRepository.GetByIdAsync(request.Id);

            company.Id = request.Id;
            company.Name = request.Name;
            company.ModifiedAt = DateTime.UtcNow;

            await companyRepository.UpdateAsync(company);

            var updatedCompanyDto = new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                IsActive = company.IsActive
            };

            return BaseResponseModel<CompanyDto>.SuccessResponse(updatedCompanyDto, "Company updated successfully");

        }
    }
}
