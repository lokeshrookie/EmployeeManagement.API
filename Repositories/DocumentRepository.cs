using EmployeeManagement.API.Data;
using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Diagnostics.Eventing.Reader;
using System.IO.Compression;

namespace EmployeeManagement.API.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context) { _context = context; }

        public async Task<string> UploadDocumentAsync([FromBody]IFormFile file)
        {

            // extensions
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

            // maximum size 
            const long maxFileSize = 5 * 1024 * 1024; // 5mb.
            Console.WriteLine(size );

            if (size > maxFileSize)
            {
                throw new Exception("Document size can not exceed 5mb");
            }

            string fileName = file.FileName;

            // create unique name for the uploaded file.
            //string fileName = Guid.NewGuid().ToString() + extension;

            //Create path to save documents. "/Uploads"
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");


            using FileStream stream = new FileStream(Path.Combine(path , fileName), FileMode.Create);
            file.CopyTo(stream);

            return fileName;
        }


        // returns the names of all uploaded documents.

        public async Task<IEnumerable<string>> GetAllDocumentsAsync()
        {
            string path =  Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(path))
            {
                throw new Exception("Uploads directory does not exist");
            }
            
            // get complete path of all files.
            var files = Directory.GetFiles(path);
            
            // create a list to store the names of files 
            var fileList = new List<string>();

            foreach (var file in files)
            {
                fileList.Add(Path.GetFileName(file));  
                //string fileName = Guid.NewGuid().ToString();
            }

            return  fileList;
        }


        #region GetAllDocumentsListAsync

        //// returns the list of all uploaded documents.
        //public async Task<IActionResult> GetAllDocumentsListAsync()
        //{
        //    string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        //    // already check in controller

        //    //if (!Directory.Exists(path))
        //    //{
        //    //    throw new Exception("Uploads directory does not exist");
        //    //}

        //    // get all files in the directory
        //    var files = Directory.GetFiles(path); // this will get file names including path.

        //    if (files.Length == 0)
        //    {
        //        throw new Exception("No Documents found in the Uploads folder");
        //    }

        //    // Zip all the files.
        //    using var memoryStream = new MemoryStream();
        //    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //    {
        //        foreach (var filePath in files)
        //        {
        //            var fileName = Path.GetFileName(filePath);
        //            var fileBytes = System.IO.File.ReadAllBytes(filePath);
        //            var entry = archive.CreateEntry(fileName);

        //            using var entryStream = entry.Open();
        //            entryStream.Write(fileBytes, 0, fileBytes.Length);
        //        }
        //    }

        //    memoryStream.Seek(0, SeekOrigin.Begin);

        //    return await File(memoryStream.ToArray(), "application/zip", "AllFiles.zip");

        //}

        #endregion GetAllDocumentsListAsync


        public async Task<byte[]> GetDocumentByName(string name)
        {
            // directory path
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(path))
            {
                throw new Exception("Uploads directory does not exist");
            }

            string filePath = Path.Combine(path, name);
           
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            var memoryStrem = new MemoryStream();

            //var file = Path.Combine()


            return fileBytes;


        }




    }
}
