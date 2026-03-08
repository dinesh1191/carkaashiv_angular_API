namespace carkaashiv_angular_API.Services
{
    using Amazon.S3;
    using Amazon.S3.Model;
    using System.Text.Json;

    public class S3UploadServices
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _config;

        public S3UploadServices(IAmazonS3 s3Client, IConfiguration config)
        {
            _s3Client = s3Client;
            _config = config;
        }

        public string GeneratePresignedUrl(string fileName,string contentType)
        {
            var bucket = _config["S3:BucketName"];
            var key = $"parts/{Guid.NewGuid()}_{fileName}";
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(10),
                ContentType =  contentType
            };
            var uploadUrl = _s3Client.GetPreSignedURL(request);
            var fileUrl = $"https://{bucket}.s3.ap-south-1.amazon.com/{key}";
                        
            return JsonSerializer.Serialize(new
            {
                uploadUrl,
                fileUrl
            });
        }
    }
}
