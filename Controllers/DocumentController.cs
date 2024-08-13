using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories.IRepositories;
using EmployeeManagement.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        //private readonly IEmployeeRepository _employeeRepository;
        private readonly IDocumentRepository _documentRepository;

        public DocumentController(IDocumentRepository documentRepository) 
        {
            _documentRepository = documentRepository;
        }


        /**
         *  File will be saved with its name. If a file name already exists, then old file will be replaced.
         */
        //[Consumes("multipart/form-data")]
        [HttpPost]
        public async Task<IActionResult> UploadDocumentAsync( IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No File Uploaded");
            }
            Console.WriteLine($"Received content type: {file.ContentType}");

            String fileName = null;
            try
            {
               fileName =  await _documentRepository.UploadDocumentAsync(file);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok(fileName + " Uploaded Successfully");
        }


        //  reutrns file names
        [HttpGet("GetAllFileNames")]
        public async Task<IActionResult> GetAllDocumentNames()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            // Check if the directory exists
            if (!Directory.Exists(path))
            {
                return NotFound("Uploads directory does not exist.");
            }

            IEnumerable<string> files = null;

            try
            {
               files = await _documentRepository.GetAllDocumentsAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok(files);
        }


        // Donload file with its name.
        [HttpGet("GetByName")]
        public async Task<IActionResult> GetDocumentByNameAsync(string name)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(path))
            {
                return NotFound("Uploads directory does not exist.");
            }
            byte[] file = null;

            try
            {
                 file = await _documentRepository.GetDocumentByName(name);

                Console.WriteLine(file);
            }

            
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


            return File(file, "application/octet-stream", name);
            //return Ok(file);


        }


        //// returns list of files
        //[HttpGet("GetAllFiles")]
        //public async Task<IActionResult> AllDocumentsListAsync()
        //{
        //    string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        //    // Check if the directory exists
        //    if (!Directory.Exists(path))
        //    {
        //        return NotFound("Uploads directory does not exist.");
        //    }


        //    IEnumerable<string> files = null;
        //    try
        //    {
        //        files = await _documentRepository.GetAllDocumentsListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //    return Ok(files);
        //}
    }
}
