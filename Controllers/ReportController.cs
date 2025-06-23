using devtrack.AppDBContext;
using devtrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace devtrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ReportController(AppDbContext context) => _context = context;

        [Authorize(Roles = "mandor")]
        [HttpPost("submit")]
        public IActionResult SubmitReport([FromBody] Report report)
        {
            try
            {
                if (report == null)
                {
                    return BadRequest(new { message = "Data laporan tidak boleh kosong." });
                }

                _context.Reports.Add(report);
                _context.SaveChanges();

                return Ok(new { message = "Laporan berhasil disubmit.", report });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat menyimpan laporan.", error = ex.InnerException.Message });
            }
        }

        [Authorize(Roles = "developer,mandor")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllReports()
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
                List<Report> result = new();

                if (role == "mandor")
                {
                    var mandor = await _context.MandorProjects
                        .FirstOrDefaultAsync(mp => mp.UserId == userId);

                    if (mandor == null)
                    {
                        return NotFound(new { message = $"Mandor dengan userId {userId} tidak ditemukan." });
                    }
                    result = await _context.Reports
                        .Where(r => r.MandorProyekId == mandor.MandorProyekId)
                        .Include(r => r.Project)
                        .OrderByDescending(r => r.UpdatedAt)
                        .ToListAsync();

                }
                else if (role == "developer")
                {
                    result = await _context.Reports
                        .Include(r => r.Project)
                        .ToListAsync();
                }
                else
                {
                    return Forbid("Role tidak diizinkan");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil data proyek.", error = ex.Message });
            }
        }

        [Authorize(Roles = "developer,mandor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var report = await _context.Reports
                                           .Include(r => r.Project)
                                           .FirstOrDefaultAsync(r => r.LaporanId == id);

                if (report == null)
                {
                    return NotFound(new { message = $"Laporan dengan ID {id} tidak ditemukan." });
                }

                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mencari laporan.", error = ex.Message });
            }
        }

        [Authorize(Roles = "developer,mandor")]
        [HttpGet("byProject/{projectId}")]
        public async Task<IActionResult> GetByProjectId(int projectId)
        {
            try
            {
                var reports = await _context.Reports
                                            .Include(r => r.Project)
                                            .Where(r => r.ProjectId == projectId)
                                            .OrderByDescending(r => r.LaporanId)
                                            .ToListAsync();


                return Ok(reports);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil laporan berdasarkan project.", error = ex.Message });
            }
        }


        [Authorize(Roles = "mandor")]
        [HttpPut("edit/{id}")]
        public IActionResult EditReport(int id, [FromBody] UpdateReportDTO updatedReport)
        {
            try
            {
                if (updatedReport == null)
                {
                    return BadRequest(new { message = "Data laporan tidak boleh kosong." });
                }

                var existing = _context.Reports.FirstOrDefault(r => r.LaporanId == id);
                if (existing == null)
                {
                    return NotFound(new { message = $"Laporan dengan ID {id} tidak ditemukan." });
                }

                existing.Deskripsi = updatedReport.Deskripsi;
                existing.Material = updatedReport.Material;
                existing.JumlahPekerja = updatedReport.JumlahPekerja;
                existing.Kendala = updatedReport.Kendala;
                existing.Foto = updatedReport.Foto;
                existing.UpdatedAt = updatedReport.UpdatedAt;

                _context.SaveChanges();

                return Ok(new { message = "Laporan berhasil diedit.", existing });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengedit laporan.", error = ex.Message });
            }
        }


        [Authorize(Roles = "mandor")]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteReport(int id)
        {
            var report = _context.Reports.FirstOrDefault(r => r.LaporanId == id);
            if (report == null) return NotFound();
            _context.Reports.Remove(report);
            _context.SaveChanges();
            return Ok(new { message = "Report deleted" });
        }


    }

}
