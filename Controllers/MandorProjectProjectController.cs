using devtrack.AppDBContext;
using devtrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace devtrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MandorProjectProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MandorProjectProjectController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var mandorProjectProjects = await _context.MandorProjectProjects
                .Include(mpp => mpp.MandorProject)
                .ThenInclude(mp => mp.User)
                .Include(mpp => mpp.Project)
                .ToListAsync();

            if (mandorProjectProjects.Count == 0)
            {
                return NotFound("Tidak ada data di tabel mandor_project_project");
            }

            return Ok(mandorProjectProjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var mandorProjectProject = await _context.MandorProjectProjects
                .Include(mpp => mpp.MandorProject)
                .ThenInclude(mp => mp.User)
                .Include(mpp => mpp.Project)
                .FirstOrDefaultAsync(mpp => mpp.Id == id);

            if (mandorProjectProject == null)
            {
                return NotFound($"Relasi mandor dan project dengan Id {id} tidak ditemukan.");
            }

            return Ok(mandorProjectProject);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignMandor(int mandorProyekId, int projectId)
        {
            var mandor = await _context.MandorProjects.FindAsync(mandorProyekId);
            if (mandor == null)
            {
                return NotFound($"Mandor dengan Id {mandorProyekId} tidak ditemukan.");
            }

            if (mandor.IsWorking)
            {
                return BadRequest("Mandor sedang bekerja pada project lain dan tidak dapat ditugaskan.");
            }

            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound($"Projek dengan Id {projectId} tidak ditemukan.");
            }

            var mandorProjectProject = new MandorProjectProject
            {
                MandorProyekId = mandorProyekId,
                ProjectId = projectId
            };
            _context.MandorProjectProjects.Add(mandorProjectProject);
            mandor.IsWorking = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Mandor berhasil ditugaskan pada project." });
        }
    }
}

