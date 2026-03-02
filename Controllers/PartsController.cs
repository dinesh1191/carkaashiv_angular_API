using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Interfaces;
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


    public class PartsController : ControllerBase
    {
      
        private readonly IPartService _partService;

        public PartsController(IPartService partService)
        {            
            _partService = partService;
        }

        // GET: api/parts
        /// <summary>
        /// Returns all available parts.
        /// </summary>
        /// <remarks>
        /// Accessible by Admin, Employee and Customer.
        /// </remarks>
        /// <returns>List of parts</returns>
        /// <response code="200">Parts fetched successfully</response>
        [Authorize(Roles="admin,employee,customer")]
        [HttpGet]
        public async Task<IActionResult> GetAllParts()
        {
            var parts = await _partService.GetAllPartsAsync();
            return Ok(ApiResponse<IEnumerable<PartResponseDto>>
                .Ok("Parts fetched successfully", parts));
        }
        /// <summary>
        /// Update a part.
        /// </summary>
        /// <param name="id">Update part</param>
        /// <response code="201">Part Updated successfully</response>
        /// <response code="403">Forbidden for employee</response>

        // GET api/<PartsController>/5 by id
        //Controller route + Action route = final endpoint
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPartById(int id)
        {
            var part = await _partService.GetPartByIdAsync(id);
            if (part == null)

                return NotFound(ApiResponse<object>.Fail("Part not found"));

            return Ok(ApiResponse<PartResponseDto>.Ok("Part fetched successfully", part));
        }




        // POST /api/parts for new part creation on db
        /// <summary>
        /// Creates a new Part.
        /// </summary>
        /// <param name="dto">Part creation details</param>
        /// <response code="201">Part created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="403">Forbidden for customers</response>
        /// <response code="401">Unauthorized</response>
        [Authorize(Roles ="admin,employee")]
        [HttpPost]
        public async Task<IActionResult> CreatePart(PartCreateDto dto)
        {
            var part = await _partService.CreatePartAsync(dto);
            return CreatedAtAction(
                nameof(GetPartById),
                new { id = part.Id },
                ApiResponse<PartResponseDto>
                .Ok("Part added successfully", part));
        }



        /***Update PART***/
        /// <summary>
        /// Updates an existing Part by id
        /// </summary>
        /// <param name="id">Part identifier</param>
        /// <param name="dto">Updates Part details </param>
        /// <response code="200">Part Updated successfully</response>
        /// <response code="404">Part not found</response>
        /// <response code="403">Forbidden for customer</response>
        /// <response code="401">Unauthorised</response>
        /// <returns></returns>
        //Put api/parts/{id} for updating existing part on db
        [Authorize(Roles ="admin,employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePart(int id, PartUpdateDto dto)
        {
            var part = await _partService.UpdatePartAsync(id, dto);
            if (part == null)
                return NotFound(new { message = "Part not found" });
            return Ok(ApiResponse<PartResponseDto>.Ok("Part updated successfully", part));
        }

        /// <summary>
        /// Deletes a part by ID.
        /// </summary>
        /// <param name="id">Part ID</param>
        /// <response code="200">Deleted successfully</response>
        /// <response code="404">Part not found</response>
        /// <response code="403">Forbidden</response>
        //Delete /api/parts/{id}
        [Authorize(Roles ="admin")]
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
                return BadRequest(ApiResponse<object>.Fail("Cannot delete part beacuse it is linked with orders"));
            }          
        }
    }
}
