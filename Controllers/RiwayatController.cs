using devtrack.AppDBContext;
using devtrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;



namespace devtrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RiwayatController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RiwayatController(AppDbContext context) => _context = context;

        [Authorize]
        [HttpGet("view")]
        public IActionResult ViewRiwayat()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userIdClaim == null || roleClaim == null)
                {
                    return BadRequest(new { message = "Invalid token" });
                }

                var userId = int.Parse(userIdClaim);
                var role = roleClaim;

                if (role == "mandor")
                {
                    var mandorId = _context.MandorProjects
                                           .FirstOrDefault(m => m.UserId == userId)
                                           ?.MandorProyekId;

                    if (mandorId == null)
                    {
                        return NotFound(new { message = "Mandor tidak ditemukan." });
                    }


                    var projectIds = _context.MandorProjectProjects
                                              .Where(m => m.MandorProyekId == mandorId)
                                              .Select(m => m.ProjectId)
                                              .ToList();

                    if (!projectIds.Any())
                    {
                        return NotFound(new { message = "Mandor tidak memiliki project." });
                    }


                    var riwayat = _context.Riwayats
                                           .Where(r => projectIds.Contains(r.ProjectId))
                                           .Include(r => r.Project)
                                           .ToList();
                    if (!riwayat.Any())
                    {
                        return Ok(new { message = "Tidak ada Riwayat" });
                    }

                    return Ok(riwayat);

                }
                else
                {
                    var allRiwayat = _context.Riwayats.ToList();
                    if (!allRiwayat.Any())
                    {
                        return Ok(new { message = "Tidak ada Riwayat" });
                    }
                    return Ok(allRiwayat);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan.", error = ex.Message });
            }
        }

        [Authorize(Roles = "developer")]
        [HttpPost("add-riwayat")]
        public async Task<IActionResult> AddRiwayat([FromBody] Riwayat newRiwayat)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var project = await _context.Projects.FindAsync(newRiwayat.ProjectId);
                if (project == null)
                    return NotFound($"Project dengan ID {newRiwayat.ProjectId} tidak ditemukan.");

                project.Deadline = DateTime.SpecifyKind(project.Deadline, DateTimeKind.Utc);
                _context.Riwayats.Add(newRiwayat);

                project.Status = "Selesai";
                _context.Projects.Update(project);

                var mandorProjectProjects = await _context.MandorProjectProjects
                    .Where(mpp => mpp.ProjectId == newRiwayat.ProjectId)
                    .Include(mpp => mpp.MandorProject)
                    .ToListAsync();

                foreach (var mandorProjectProject in mandorProjectProjects)
                {
                    if (mandorProjectProject.MandorProject != null)
                    {
                        mandorProjectProject.MandorProject.IsWorking = false;
                        _context.MandorProjects.Update(mandorProjectProject.MandorProject);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Riwayat berhasil ditambahkan, status project diperbarui, dan mandor project di-nonaktifkan.",
                    riwayat = newRiwayat,
                    updatedMandorCount = mandorProjectProjects.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Terjadi kesalahan saat menambahkan riwayat.",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }


    }

}
