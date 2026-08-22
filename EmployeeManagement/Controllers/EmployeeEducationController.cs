using EmployeeManagement.Dtos;
using EmployeeManagement.Dtos.EmployeeEduDto;
using EmployeeManagement.Service;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeEducationController(IEmployeeEducationService employeeEducationService) : ControllerBase
{
    [HttpPost("CreateEmployeeEducation")]
    public async Task<IActionResult> Create(CreateEmployeeEducationDto request)
    {
        var result = await employeeEducationService.CreateEmployeeEducationAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }


    [HttpPost("CreateEmployeeEducationHistory")]
    public async Task<IActionResult> CreateHistory([FromBody] CreateEmployeeEducationHistoryDto request)
    {
        var result = await employeeEducationService.CreateEmployeeEducationHistoryAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("GetEmployeeEducationById/{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await employeeEducationService.GetEmployeeEducationByIdAsync(id);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("GetAllEmployeeEducations")]
    public async Task<IActionResult> GetAll()
    {
        var result = await employeeEducationService.GetAllEmployeeEducationsAsync();
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPut("UpdateEmployeeEducation/{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, UpdateEmployeeEducationDto request)
    {
        request.Id = id;
        var result = await employeeEducationService.UpdateEmployeeEducationAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpDelete("Delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await employeeEducationService.DeleteEmployeeEducationAsync(id);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}