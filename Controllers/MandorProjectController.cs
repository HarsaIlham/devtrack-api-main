using devtrack.AppDBContext;
using devtrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace devtrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MandorProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MandorProjectController(AppDbContext context) => _context = context;

        [Authorize(Roles = "developer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MandorProject>>> GetAll()
        {
            try
            {
                var mandorProjects = await _context.MandorProjects
                    .Include(mp => mp.User) 
                    .ToListAsync();

                if (mandorProjects == null || !mandorProjects.Any())
                {
                    return NotFound(new { message = "Data Mandor Project tidak ditemukan." });
                }

                return Ok(mandorProjects);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil Mandor Project.", error = ex.Message });
            }
        }

        [Authorize(Roles = "developer,mandor")]
        [HttpGet("{id}")]
        public async Task<ActionResult<MandorProject>> GetById(int id)
        {
            try
            {
                var mandorProject = await _context.MandorProjects
                    .Include(mp => mp.User)
                    .FirstOrDefaultAsync(mp => mp.UserId == id);

                if (mandorProject == null)
                {
                    return NotFound(new { message = $"Mandor Project dengan ID {id} tidak ditemukan." });
                }

                return Ok(mandorProject);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil data Mandor Project.", error = ex.Message });
            }
        }
    }
}
