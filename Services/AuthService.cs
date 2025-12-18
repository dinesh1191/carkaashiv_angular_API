using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Interfaces;
using carkaashiv_angular_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace carkaashiv_angular_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<TableUser> _userHasher = new();
        private readonly PasswordHasher<TableEmployee> _empHasher = new();


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
            var user = new TableUser
            {
                Name = dto.Name,
                Phone = dto.phone,
                Email = dto.Email,
                Role = dto.Role
            };
            user.PasswordHash = _userHasher.HashPassword(user, dto.password);
            await _db.tbl_user.AddAsync(user);
            await _db.SaveChangesAsync();
            return true;
        }



        public async Task<bool> RegisterEmployeeAsync(RegisterEmployeeDto dto)
        {
            if (await _db.tbl_emp.AnyAsync(x => x.Email == dto.Email))
                return false;
            var emp = new TableEmployee
            {
                
                Name = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                Role = dto.Role
            };
            emp.EmpPasswordHash = _empHasher.HashPassword(emp, dto.Password);
            await _db.tbl_emp.AddAsync(emp);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
