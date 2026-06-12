namespace carkaashiv_angular_API.Services
   
{
    using Amazon.S3;
    using Amazon.S3.Model;
    using Azure;
    using carkaashiv_angular_API.DTOs;
    using carkaashiv_angular_API.Interfaces;
    using Microsoft.AspNetCore.Http.HttpResults;
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
              //  ContentType =  contentType
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
            if (string.IsNullOrWhiteSpace(key))
                return false;
            var bucket = _config["S3:BucketName"];
            // Normalize the S3 key: decode URL encoding and remove any leading '/'
                    
            // Normalize key received from API route
            key = WebUtility.UrlDecode(key);
            key = key.TrimStart('/');

            //If full URL is passed → extract key
            if (key.StartsWith("http"))
            {
                key = ExtractKeyFromUrl(key);
            }

            // Security check: Only allow deletion known folders

            // If key is NOT below listed folder → block it(accidental delete protection)
            var folder = key.Split('/')[0];

            if (!AllowedFolders.Contains(folder))
            {
                throw new InvalidOperationException("Invalid S3 key. Deletion not allowed.");
            }
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = bucket,
                    Key = key
                };
                var response = await _s3Client.DeleteObjectAsync(request);
                Console.WriteLine($"S3 delete success: {key} | Status : {response.HttpStatusCode}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"S3 delete failed: {key} | Error:{ex.Message}");
                return false;
            }          
        }        

        public async Task<(string finalUrl, string finalKey)> FinalizeImageAsync(string? tempKey, string destinationFolder, string? existingImageUrl)
        {
            var bucket = _config["S3:BucketName"];

            // Case 1: No new image → keep existing image

            if (string.IsNullOrEmpty(tempKey))
                return 
                    (existingImageUrl ?? "",
                    ExtractKeyFromUrl(existingImageUrl ?? "")
                    );

          // Normalize incoming key (decode URL + remove Leading '/')
            tempKey = WebUtility.UrlDecode(tempKey);
            tempKey = tempKey.TrimStart('/');


            // Case 2: Already a final image (parts/) -> no procesing needed

            if (!tempKey.StartsWith("temp/"))
            {
                return (
                    existingImageUrl ?? "",
                    tempKey
                    );
            }
            //Step 1:Build final key (move from temp -> parts)
            var finalKey = tempKey.Replace("temp/", $"{destinationFolder}/");
            var finalUrl = $"https://{bucket}.s3.ap-south-1.amazonaws.com/{finalKey}";

            //Step 2: Copy image form temp/ -> parts/
            await _s3Client.CopyObjectAsync(new CopyObjectRequest
            {
                SourceBucket = bucket,
                SourceKey = tempKey,
                DestinationBucket = bucket,
                DestinationKey = finalKey
            });

            //Step 3: Delete temp image (cleanup)
            await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = bucket,
                Key = tempKey
            });

            //Step 4: Delete OLD image from parts/ (only if different from new one)
            if (!string.IsNullOrEmpty(existingImageUrl))
            {
                var oldKey = ExtractKeyFromUrl(existingImageUrl);
                oldKey = WebUtility.UrlDecode(oldKey);

                //Important : compare with finalKey(not tempKey)
                if (oldKey.StartsWith("parts/") && oldKey != finalKey)
                {
                    await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
                    {
                        BucketName = bucket,
                        Key = oldKey
                    });
                }
            }
      
            // Final result → return correct order (URL first, then key)
            return (finalUrl,finalKey);
        }

        private string ExtractKeyFromUrl(string url)
        {
            var uri = new Uri(url);
            return uri.AbsolutePath.TrimStart('/'); //**** https ://bucket.s3.ap-south-1.amazonaws.com/parts/abc.png -> parts / abc.png *******/
        }
        private static readonly HashSet<string> AllowedFolders =
         [
            "temp",
            "parts",
            "payments",
            "orders"
         ];
    }

}
