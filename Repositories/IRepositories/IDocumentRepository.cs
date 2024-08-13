using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace EmployeeManagement.API.Repositories.IRepositories
{
    public interface IDocumentRepository
    {
        // upload document.
        Task<string> UploadDocumentAsync(IFormFile file);
        Task<IEnumerable<string>> GetAllDocumentsAsync();


        // returns the list of all uploaded documents.
        //Task<IEnumerable<string>> GetAllDocumentsListAsync();
        //Task<IActionResult> GetAllDocumentsListAsync();

        Task<byte[]> GetDocumentByName(string name);
    }
}
