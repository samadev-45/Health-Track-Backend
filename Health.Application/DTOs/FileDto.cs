using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs
{
    public class FileDto
    {
        public int FileStorageId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string? ContentType { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
    }
}
