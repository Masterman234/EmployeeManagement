using EmployeeManagement.Dtos;
using EmployeeManagement.Service;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateEmployeeDto request)
    {
        var result = await employeeService.CreateEmployeeAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, EmployeeDto request)
    {
        var result = await employeeService.GetEmployeeByIdAsync(request.Id);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll()
    {
        var result = await employeeService.GetAllEmployeesAsync();
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, EmployeeDto request)
    {
        var result = await employeeService.UpdateEmployeeAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }


    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await employeeService.GetEmployeeByIdAsync(id);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

}
