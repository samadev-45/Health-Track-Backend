using System;
using System.ComponentModel.DataAnnotations;

namespace Health.Application.DTOs.File
{
    public class UploadFileDto
    {
        
        [Required]
        public string Base64Data { get; set; } = string.Empty;

        
        [Required]
        public string FileName { get; set; } = string.Empty;

       
        public string? ContentType { get; set; }

       
        public long FileSize { get; set; }

        
        public int UploadedByUserId { get; set; }
    }
}
