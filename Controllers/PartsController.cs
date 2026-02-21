using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Shared;
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
      
        private readonly PartService _partService;

        public PartsController(PartService partService)
        {            
            _partService = partService;
        }

        // GET: api/parts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Part>>> GetAllParts()
        {

            var parts = await _partService.GetAllPartsAsync();
            return Ok(ApiResponse<IEnumerable<PartResponseDto>>
                    .Ok("Parts fetched successfully", parts));
           
         }

        // GET api/<PartsController>/5 by id
        //Controller route + Action route = final endpoint
        [HttpGet("{id}")]
        public async Task<ActionResult<Part>> GetPartById(int id)
        {
            var part = await _partService.GetPartByIdAsync(id);
            if (part == null)
            {
                return NotFound(ApiResponse<object>.Fail("Part not found"));
            }
            return Ok(ApiResponse<PartResponseDto>.Ok("Part fetched successfully", part));
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
           
            try
            {
                var deleted = await _partService.DeletePartAsync(id);
                if (!deleted)
                    return NotFound(ApiResponse<object>.Fail("Part not found"));
                return Ok(ApiResponse<object>.Ok("Part deleted successfully"));            
              }
            catch (DbUpdateException )
            {
                return BadRequest(
                    ApiResponse<object>.Fail("Cannot delete part beacuse it is linked with orders"));
            }          
        }
    }
}
