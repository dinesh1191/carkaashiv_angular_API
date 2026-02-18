using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace carkaashiv_angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] //Authorized by default
    public class EmployeesController :ControllerBase
    {
        //private readonly AppContext _context;
        private readonly EmployeeService _employeeService; //Business logic stays in service
        public EmployeesController(EmployeeService employeeService )
        {
            _employeeService = employeeService;
        }
      
        [HttpPut("me")]
        public async Task<IActionResult> UpdateSelf(EmployeeSelfUpdateDto dto)
        {
            var result = await _employeeService.UpdataSelfAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateByAdmin(int id, EmployeeAdminUpdateDto dto)
        {
            var result = await _employeeService.UpdateByAdminAsync(id, dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }

}
