namespace EmployeeManagement.API.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }

        public string ContentType { get; set; }    

        public DateTime UploadedAt { get; set; }
    }
}