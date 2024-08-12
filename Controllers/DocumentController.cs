using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories.IRepositories;
using EmployeeManagement.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        //private readonly IEmployeeRepository _employeeRepository;
        private readonly IDocumentRepository _documentRepository;

        public DocumentController(IDocumentRepository documentRepository) 
        {
            _documentRepository = documentRepository;
        }


        //[Consumes("multipart/form-data")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocumentAsync( IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No File Uploaded");
            }
            Console.WriteLine($"Received content type: {file.ContentType}");

            try
            {
                await _documentRepository.UploadDocumentAsync(file);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok("File Uploaded Successfully");
        }
    }
}
