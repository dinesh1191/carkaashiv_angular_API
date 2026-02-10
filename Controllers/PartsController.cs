using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Auth;
using carkaashiv_angular_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

namespace carkaashiv_angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PartsController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly PartService _partService;

        public PartsController(AppDbContext context, PartService partService)
        {
            _context = context;
            _partService = partService;
        }

        // GET: api/parts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TablePart>>> GetAllParts()
        {
            var parts = await _context.tbl_part.ToListAsync();
            return Ok(Models.Auth.ApiResponse<IEnumerable<TablePart>>.Ok("Parts fetched successfully",parts));
         }

        // GET api/<PartsController>/5 by id
        [HttpGet("parts/{id}")]
        public async Task<ActionResult<TablePart>> GetPartById(int id)
        {
            try
            {
                var part = await _context.tbl_part.FindAsync(id);
                if (part == null)
                {
                    return NotFound(ApiResponse<TablePart>.Fail("Part not found"));
                }
                return Ok(ApiResponse<TablePart>.Ok("Part fetched successfully", part));

            }
            catch (Exception ex)            {
               
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        // POST /api/parts for new part creation on db
        [HttpPost]
        public async Task<IActionResult> CreatePart([FromBody] PartCreateDto dto) 
        {
            var part = await _partService.CreatePartAsync(dto);

            return CreatedAtAction(
                nameof(GetPartById),
                new { id = part.PartId },
                new { message = "Part added successfully", data = part }
            );
        }

  
        //Put api/parts/{id} for updating existing part on db
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePart(int id,[FromBody] PartUpdateDto dto)
        {
            var updatePart = await _partService.UpdatePartAsync(id, dto);
            if (updatePart == null)
                return NotFound(new { message = "Part not found" });
            return Ok(new
            {
                message = "Part updated successfully",
                data = updatePart
            });
        }


        //Delete /api/parts/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePart(int id)
        {
            var part = await _context.tbl_part.FindAsync(id);
            if(part == null)
            {
              
               return NotFound(Models.Auth.ApiResponse<object>.Fail("Part not found"));
            }
            try
            {
                _context.tbl_part.Remove(part);
                await _context.SaveChangesAsync();
                return Ok(Models.Auth.ApiResponse<object>.Ok("Part deleted successfully"));
            }
            catch (DbUpdateException )
            {
                return BadRequest(Models.Auth.ApiResponse<object>.Fail("Cannot delete part beacuse it is linked with orders"));
            }
          
        }
    }
}
