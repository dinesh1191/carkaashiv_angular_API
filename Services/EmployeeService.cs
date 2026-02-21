using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;

namespace carkaashiv_angular_API.Services
{
    public class EmployeeService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;

            //var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
        }
        [Authorize]
        public async Task<ApiResponse<EmployeeResponseDto>> UpdataSelfAsync(EmployeeSelfUpdateDto dto)
        {
            var userIdClaim = _httpContextAccessor.HttpContext!.User.FindFirst("userId")?.Value;


            if (string.IsNullOrEmpty(userIdClaim))
                return ApiResponse<EmployeeResponseDto>.Fail("Unauthorised");

            int userId = int.Parse(userIdClaim);


            var employee = await _context.tbl_emp.FindAsync(userId);
            if (employee == null)
                return ApiResponse<EmployeeResponseDto>.Fail("Employee not found ");
            // Update only allowed fields
            if (!string.IsNullOrEmpty(dto.Email))
            {
                employee.Email = dto.Email;

                if (!dto.Email.EndsWith("@kaashiv.com"))
                    return ApiResponse<EmployeeResponseDto>
                        .Fail("Only @kaashiv.com emails allowed");
            }

            if (!string.IsNullOrEmpty(dto.Password))
            {
                var emphashPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                employee.EmpPasswordHash = emphashPassword;// updated password hash
            }
            if (!string.IsNullOrEmpty(dto.Phone))
            {

                employee.Phone = dto.Phone;
            }

            await _context.SaveChangesAsync();

            var responseDto = new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Phone = employee.Phone,


            };
            return ApiResponse<EmployeeResponseDto>
            .Ok("Profile updated successfully", responseDto);
        }
        public async Task<ApiResponse<EmployeeResponseDto>> UpdateByAdminAsync(int id, EmployeeAdminUpdateDto dto)

        {
            var employee = await _context.tbl_emp.FindAsync(id);
            if (employee == null)
                return ApiResponse<EmployeeResponseDto>.Fail("Employee not found");
            //Admin can update thes fields
            if (!string.IsNullOrEmpty(dto.Name))

                employee.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Email))
                employee.Email = dto.Email;

            if (!dto.Email!.EndsWith("@kaashiv.com"))
                return ApiResponse<EmployeeResponseDto>
                    .Fail("Only @kaashiv.com emails allowed");

            if (!string.IsNullOrEmpty(dto.Role))
                employee.Role = dto.Role;

            if (string.IsNullOrEmpty(dto.Password))
                employee.EmpPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            await _context.SaveChangesAsync();
            var responseDto = new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Phone = employee.Phone,
                Role = employee.Role
            };
            return ApiResponse<EmployeeResponseDto>
                .Ok("Employee updated successfully", responseDto);
        }


        public async Task<ApiResponse<object>> RegisterEmployeeAsync(RegisterEmployeeDto dto)
        {
            // Check if email already exists
            if (await _context.tbl_emp.AnyAsync(x => x.Email == dto.Email))
                return ApiResponse<object>.Fail("Email already exists");

            var emp = new Employee
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                Role = dto.Role,
                EmpPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _context.tbl_emp.AddAsync(emp);
            await _context.SaveChangesAsync();

            return ApiResponse<object>.Ok("Employee registered successfully");
        }
    }





    
}
