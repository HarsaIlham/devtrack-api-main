using devtrack.AppDBContext;
using devtrack.Models;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MandorProject>>> GetAll()
        {
            var mandorProjects = await _context.MandorProjects
                .Include(mp => mp.User) // Jika ingin menyertakan data User
                .ToListAsync();

            return Ok(mandorProjects);
        }
    }
}
