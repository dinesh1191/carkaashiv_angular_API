using Azure;

namespace carkaashiv_angular_API.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(int userId, string role, string email);
        void SetJwtCookie(string token);
        void ClearJwtCookie(HttpResponse Response);
    }
}
