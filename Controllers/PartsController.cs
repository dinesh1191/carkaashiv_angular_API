using carkaashiv_angular_API.Data;
using carkaashiv_angular_API.Models;
using carkaashiv_angular_API.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace carkaashiv_angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PartsController : ControllerBase
    {

        private readonly AppDbContext _context;

        public PartsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<PartsController>
        [HttpGet("getParts")]

        public async Task<ActionResult<IEnumerable<TablePart>>> GetParts()
        {
            var parts = await _context.tbl_part.ToListAsync();
            return Ok(Models.Auth.ApiResponse<IEnumerable<TablePart>>.Ok("Parts fetched successfully",parts));
         }

        // GET api/<PartsController>/5 by id
        [HttpGet("{id}")]
        public async Task<ActionResult<TablePart>> GetPart(int id)
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

        // POST api/<PartsController> for new part creation on db
        [HttpPost("addPart")]
        public async Task<ActionResult<TablePart>> addPart(TablePart part) 
        {

            _context.tbl_part.Add(part);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPart), new {id = part.PartId}, 
                new { message = "Part added sucessfully",data = part });
        }

        // PUT api/<PartsController>/5 for updating existing part on db

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPart(int id ,TablePart part)
        {
            if(id != part.PartId)
            {
                return BadRequest();
            }
            _context.Entry(part).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new {message = "Part updated successfully"});
        }

        // DELETE api/<PartsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePart(int id)
        {
            var part = await _context.tbl_part.FindAsync(id);
            if(part == null)
            {
               // return NotFound();
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
