namespace Health.Application.DTOs.File
{
    public class FileDownloadDto
    {
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = "application/octet-stream";
        public byte[] FileBytes { get; set; } = Array.Empty<byte>();
    }
}
