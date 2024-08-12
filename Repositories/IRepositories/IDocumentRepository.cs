using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace EmployeeManagement.API.Repositories.IRepositories
{
    public interface IDocumentRepository
    {
        // upload document.
        Task<string> UploadDocumentAsync(IFormFile file); 
    }
}
