using devtrack.AppDBContext;
using devtrack.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "developer,mandor")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var mandorProjectProjects = await _context.MandorProjectProjects
                    .Include(mpp => mpp.MandorProject)
                        .ThenInclude(mp => mp.User)
                    .Include(mpp => mpp.Project)
                    .ToListAsync();

                if (mandorProjectProjects == null ||
                    mandorProjectProjects.Count == 0)
                {
                    return NotFound(new { message = "Tidak ada data di tabel mandor_project_project." });
                }

                return Ok(mandorProjectProjects);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil data.", error = ex.Message });
            }
        }

        [Authorize(Roles = "developer")]
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

        [Authorize]
        [HttpGet("byMandor")]
        public async Task<IActionResult> GetByUser()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(role))
                {
                    return Unauthorized(new { message = "UserId atau Role tidak tersedia dalam token." });
                }

                int userId = int.Parse(userIdClaim);

                List<MandorProjectProject> result = new();

                if (role == "mandor")
                {
                    var mandor = await _context.MandorProjects
                        .FirstOrDefaultAsync(mp => mp.UserId == userId);

                    if (mandor == null)
                    {
                        return NotFound(new { message = $"Mandor dengan userId {userId} tidak ditemukan." });
                    }

                    result = await _context.MandorProjectProjects
                        .Where(mpp => mpp.MandorProyekId == mandor.MandorProyekId)
                        .Include(mpp => mpp.Project)
                        .ToListAsync();
                }
                else if (role == "developer")
                {
                    result = await _context.MandorProjectProjects
                        .Include(mpp => mpp.MandorProject)
                            .ThenInclude(mp => mp.User)
                        .Include(mpp => mpp.Project)
                        .ToListAsync();
                }
                else
                {
                    return Forbid("Role tidak diizinkan.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil data proyek.", error = ex.Message });
            }
        }


        [Authorize(Roles = "developer")]
        [HttpPost("assign")]
        public async Task<IActionResult> Create([FromBody] MandorProjectProject mpp)
        {
            try
            {
                if (mpp == null)
                {
                    return BadRequest(new { message = "Data MandorProjectProject tidak boleh kosong." });
                }

                var mandor = await _context.MandorProjects.FindAsync(mpp.MandorProyekId);
                if (mandor == null)
                {
                    return BadRequest(new { message = $"MandorProjectId {mpp.MandorProyekId} tidak ditemukan." });
                }

                var project = await _context.Projects.FindAsync(mpp.ProjectId);
                if (project == null)
                {
                    return BadRequest(new { message = $"ProjectId {mpp.ProjectId} tidak ditemukan." });
                }

                _context.MandorProjectProjects.Add(mpp);
                mandor.IsWorking = true;
                _context.MandorProjects.Update(mandor);
                await _context.SaveChangesAsync();


                return Ok(new { message = "Mandor berhasil ditugaskan", mpp });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat menyimpan.", error = ex.Message });
            }
        }
    }
}

