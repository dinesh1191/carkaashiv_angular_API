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
        var result = _s3UploadService.GeneratePresignedUrl(request.FileName, request.ContentType);
            return Ok(result);
        }

        [HttpDelete("{*key}")]
        //{*key} allows the route to accept slashes inside the key
        // Without *, ASP.NET would break the route
        public async Task<IActionResult>DeleteFile(string key)
        {
            try
            {
                await _s3UploadService.DeleteFileAsync(key);
                return Ok(new {message = "File deleted successfully"});
            }
            catch (Exception ex) { 
            
            return BadRequest(new { message = ex.Message });
            }            
        }
        //[HttpPost("confirm-image")]
        //public async Task<IActionResult> ConfirmImage([FromBody] ConfirmImageRequestDto dto) 
            
        //{
        //    var finalUrl = await _s3UploadService.MoveTempToPartsAsync(dto.Key);
        //    return Ok(new { imageUrl = finalUrl });

        //}

    }
}
