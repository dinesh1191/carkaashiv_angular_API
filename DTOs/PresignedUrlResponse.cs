namespace carkaashiv_angular_API.DTOs
{
    public class PresignedUrlResponse
    {
        public string UploadUrl { get; set; } = string.Empty;
        public string FileUrl { get; set;} = string.Empty;
        public string Key { get; set;  }= string.Empty;

    }
}
