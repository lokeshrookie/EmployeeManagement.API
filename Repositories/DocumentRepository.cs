using EmployeeManagement.API.Data;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context) { _context = context; }

        public async Task<string> UploadDocumentAsync([FromBody]IFormFile file)
        {

            // extension
            List<string> validExtensions = new List<string>()
            {
                ".jpg",
                ".png",
                ".pdf"
            };
            string extension = Path.GetExtension(file.FileName);

            if (!validExtensions.Contains(extension.ToLower()))
            {
                throw new Exception ($"Extension is not valid. Please upload file with any of these extensions ({string.Join(',',validExtensions)}) ");
            }


            // size
            long size = file.Length;
            if (size > (5 * 1024 * 1024))
            {
                return "Maximum size can be 5MB";
            }

            string fileName = Guid.NewGuid().ToString() + extension;


            // name changing
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");


            using FileStream stream = new FileStream(Path.Combine(path , fileName), FileMode.Create);
            file.CopyTo(stream);

            return fileName;


            //using (var memoryStream = new MemoryStream())
            //{
            //    await file.CopyToAsync(memoryStream);
            //    var document = new Document
            //    {
            //        FileName = file.FileName,
            //        ContentType = file.ContentType,
            //        Data = memoryStream.ToArray(),
            //        UploadedAt = DateTime.UtcNow
            //    };

            //    _context.Documents.Add(document);
            //    await _context.SaveChangesAsync();
            //}
        }
    }
}
