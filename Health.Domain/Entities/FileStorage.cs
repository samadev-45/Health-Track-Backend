
using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class FileStorage : BaseEntity
    {
        public int FileStorageId { get; set; }

        public string FileName { get; set; } = null!;
        public string FileExtension { get; set; } = null!;   
        //varbinary(max)
        public byte[] FileData { get; set; } = Array.Empty<byte>(); 
        public long FileSize { get; set; }                    
        public string? ContentType { get; set; }              

        public int UploadedByUserId { get; set; }
        public User UploadedByUser { get; set; } = null!;
    }
}
