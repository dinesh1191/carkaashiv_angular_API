namespace carkaashiv_angular_API.Services
{
    using Amazon.S3;
    using Amazon.S3.Model;
    using Azure;
    using carkaashiv_angular_API.DTOs;
    using System.Net;
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

        public PresignedUrlResponse GeneratePresignedUrl(string fileName,string contentType)
        {
            var bucket = _config["S3:BucketName"];

            var key = $"temp/{Guid.NewGuid()}_{fileName?.Replace(" ","_") }";//normalize file names eg:back light.png → back_light.png

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(10),
                ContentType =  contentType
            };
            var uploadUrl = _s3Client.GetPreSignedURL(request);
            var fileUrl = $"https://{bucket}.s3.ap-south-1.amazonaws.com/{key}";
            return new PresignedUrlResponse
            {
                UploadUrl = uploadUrl,
                FileUrl = fileUrl,
                Key = key
            };
        }
        public async Task<bool> DeleteFileAsync(string key)
        {
            var bucket = _config["S3:BucketName"];
            // Normalize the S3 key: decode URL encoding and remove any leading '/'
            // so it matches the actual S3 object key format (e.g., "temp/file.png").
            // Normalize key received from API route
            key = WebUtility.UrlDecode(key);
            key = key.TrimStart('/');           
            
            //If only filename is provided,assume it belongs to temp/
            if (!key.Contains("/"))
            {
                key = $"temp/{key}";
            }
            
            //Final security check: Only allow deletion inside temp/
            if (!key.StartsWith("temp/")) //Restricting deletion to temp/ protects your data.
            throw new InvalidOperationException("Only temp folder deletion allowed");
            
            
            var request = new DeleteObjectRequest
            { 
                BucketName = bucket,        
                Key = key
            };
            var response = await _s3Client.DeleteObjectAsync(request);
            Console.WriteLine($"s3 delete status:{response.HttpStatusCode}");
           
            return true;
        }        

        public async Task<string> FinalizeImageAsync(string? tempKey, string? existingImageUrl)
        {
            var bucket = _config["S3:BucketName"];
            // Case 1: No new image → keep existing
            if (string.IsNullOrEmpty(tempKey))
                return existingImageUrl ?? "";

            tempKey = WebUtility.UrlDecode(tempKey);
            tempKey = tempKey.TrimStart('/');

            // Case 2: Already in parts/ → no change
            if (!tempKey.StartsWith("temp/"))
                return existingImageUrl ?? "";

            // Build destination key
            var partsKey = tempKey.Replace("temp/", "parts/");
            // Copy temp → parts
            await _s3Client.CopyObjectAsync(new CopyObjectRequest
            {
                SourceBucket = bucket,
                SourceKey = tempKey,
                DestinationBucket = bucket,
                DestinationKey = partsKey
            });

            // Delete temp/ image
            await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = bucket,
                Key = tempKey

            });
            // Delete previous parts image if exists
            if (!string.IsNullOrEmpty(existingImageUrl))
            {
                var oldKey = ExtractKeyFromUrl(existingImageUrl);
               
                oldKey = WebUtility.UrlDecode(oldKey); //decodes url properly a

                if (oldKey.StartsWith("parts/"))
                {
                    await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
                    {
                        BucketName = bucket,
                        Key = oldKey
                    });
                }
            }

            return $"https://{bucket}.s3.ap-south-1.amazonaws.com/{partsKey}";
        }

        private string ExtractKeyFromUrl(string url)
        {
            var uri = new Uri(url);
            return uri.AbsolutePath.TrimStart('/'); //**** https ://bucket.s3.ap-south-1.amazonaws.com/parts/abc.png -> parts / abc.png *******/
        }
    }
}
