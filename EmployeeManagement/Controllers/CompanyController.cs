using EmployeeManagement.Dtos.Company;
using EmployeeManagement.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController(ICompanyService companyService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDto request)
        {
            var response = await companyService.CreateCompanyAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            var response = await companyService.GetAllCompaniesAsync();
            return Ok(response);
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCompanyById(Guid id)
        {
            var response = await companyService.GetCompanyByIdAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await companyService.UpdateCompanyAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id:guid}/deactivate")]
        public async Task<IActionResult> DeactivateCompany(Guid id)
        {
            var response = await companyService.DeActivateCompanyAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{id:guid}/activate")]
        public async Task<IActionResult> ActivateCompany(Guid id)
        {
            var response = await companyService.ActivateCompanyAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
