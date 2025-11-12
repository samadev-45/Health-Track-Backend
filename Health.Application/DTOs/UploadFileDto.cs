using System;
using System.ComponentModel.DataAnnotations;

namespace Health.Application.DTOs
{
    public class UploadFileDto
    {
        /// <summary>
        /// The Base64-encoded file content.
        /// </summary>
        [Required]
        public string Base64Data { get; set; } = string.Empty;

        /// <summary>
        /// Original file name (e.g., "report.pdf").
        /// </summary>
        [Required]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// MIME content type (e.g., "application/pdf", "image/png").
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// File size in bytes (optional, for validation).
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// The ID of the user who uploaded the file.
        /// </summary>
        public int UploadedByUserId { get; set; }
    }
}
