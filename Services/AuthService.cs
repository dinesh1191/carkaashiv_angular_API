using Azure;
using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Auth;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace carkaashiv_angular_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;

        public AuthService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<bool> RegisterUserAsync(RegisterUserDto dto)
        {
            if (await _db.tbl_user.AnyAsync(x => x.Phone == dto.phone))
            {
                return false;
            }
            var user = new User
            {
                Name = dto.Name,
                Phone = dto.phone,
                Email = dto.Email,
                Role = dto.Role
            };
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.password);           
            await _db.tbl_user.AddAsync(user);
            await _db.SaveChangesAsync();
            return true;
        }



        public async Task<bool> RegisterEmployeeAsync(RegisterEmployeeDto dto)
        {
            if (await _db.tbl_emp.AnyAsync(x => x.Email == dto.Email))
                return false;
            var emp = new Employee
            {

                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                Role = dto.Role
            };           
            emp.EmpPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            await _db.tbl_emp.AddAsync(emp);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
          
            if (string.IsNullOrWhiteSpace(request.Username) ||
                    (string.IsNullOrWhiteSpace(request.Password)))
                {
               
                return new AuthResult
                    {
                        Success = false,
                        Message = "Invalid Input"
                    };

                }

                bool isEmail = request.Username.Contains("@");
                if (isEmail)
                {
                    var employee = await _db.tbl_emp.FirstOrDefaultAsync(e => e.Email == request.Username);
                    if (employee == null || !BCrypt.Net.BCrypt.Verify(request.Password, employee.EmpPasswordHash))
                    {
                        return new AuthResult
                        {
                            Success = false,
                            Message = "Invalid credentials"

                        };
                    }
                    var responseDto = new LoginResponseDto
                    {
                        Id = employee.Id,                       
                        Email = employee.Email,
                        Role = employee.Role,
                    };
                    return new AuthResult
                    {
                        Success = true,
                        Message = "Login sucessful",
                        Data = responseDto,
                        Token = null // taken generated in controller
                    };
                }
                            
                     else

                     {
                //======Customer login flow=======
                var customer = await _db.tbl_user
                    .FirstOrDefaultAsync(c => c.Phone == request.Username);

                if (customer == null || !BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash))
                {
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Invalid Credentials"
                    };

                }
                var responseDto = new LoginResponseDto
                {
                    Id = customer.Id,                    
                    Email = customer.Email,
                    Role = customer.Role,
                };
                return new AuthResult
                {
                    Success = true,
                    Message = "Login Sucessful",
                    Data = responseDto,
                };

            }
        }
        public interface IAuthService
        {
            Task<object?> GetCurrentUserAsync(ClaimsPrincipal user);

        }

        public async Task<object?> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            var userClaimId = user.FindFirst("userId")?.Value;
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userClaimId))
                return null;

            int userId = int.Parse(userClaimId);

            if (role == "customer")
            {
                return await _db.tbl_user
                    .Where(c => c.Id == userId)
                    .Select(c => new { c.Id, c.Name, c.Email, c.Role })
                    .FirstOrDefaultAsync();
            }
            else
            {
                return await _db.tbl_emp
                    .Where(e => e.Id == userId)
                    .Select(e => new { e.Id, e.Name, e.Email, e.Role })
                    .FirstOrDefaultAsync();
            }
        }
    


}

}

