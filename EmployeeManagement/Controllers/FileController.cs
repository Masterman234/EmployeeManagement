using EmployeeManagement.Enums;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileController(IFileService fileService) : ControllerBase
    {
        

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<BaseResponseModel<FileResponseDto>>> Upload(
            [FromForm] UploadFileDto request)
        {
            var result = await fileService.UploadFileAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BaseResponseModel<FileResponseDto>>> GetById(Guid id)
        {
            var result = await fileService.GetFileByIdAsync(id);

            if (!result.Success)
            {
                return result.ErrorType == ErrorType.NotFound ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<BaseResponseModel<bool>>> Delete(Guid id)
        {
            var result = await fileService.DeleteFileAsync(id);

            if (!result.Success)
            {
                return result.ErrorType == ErrorType.NotFound ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
    }
}