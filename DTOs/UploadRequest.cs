using System.ComponentModel.DataAnnotations;

namespace carkaashiv_angular_API.DTOs
   
{
    public class UploadRequest
    {
        [Required]
        [MaxLength(200)] //Prevents weird filenames like 5MB long attack strings.
        public string FileName { get; set; } = string.Empty;
        [Required]
        public string ContentType { get; set; } = string.Empty;
    }
}
