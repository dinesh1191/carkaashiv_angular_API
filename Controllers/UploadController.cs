using Amazon.S3;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Services;
using Microsoft.AspNetCore.Mvc;


namespace carkaashiv_angular_API.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        private readonly S3UploadServices _s3UploadService;
        private readonly IAmazonS3 _s3Client;
        public UploadController(S3UploadServices s3UploadService, IAmazonS3 s3Client)
        {
            _s3UploadService = s3UploadService;
            _s3Client = s3Client ;
        }

        [HttpGet("test-s3")]
        public async Task<IActionResult> TestS3() {
           
            var buckets = await _s3Client.ListBucketsAsync();
          
            return Ok(buckets.Buckets.Select(b => b.BucketName));
        
        }

        [HttpPost("presigned-url")]
        public IActionResult GeneratetUploadUrl([FromBody] UploadRequest request)
        {
            //if(string.IsNullOrWhiteSpace(request.FileName) || 
            //    string.IsNullOrWhiteSpace(request.ContentType))
            //{
            //    return BadRequest("FileName and ContentType are required.");
            //}

            var result = _s3UploadService.GeneratePresignedUrl
                (
                request.FileName,
                request.ContentType
          
                );

            return Ok(result);
        }
    }
}
