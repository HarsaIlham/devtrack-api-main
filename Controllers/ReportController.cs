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
            _context.Reports.Add(report);
            _context.SaveChanges();
            return Ok(report);
        }

        [Authorize(Roles = "developer")]
        [HttpGet("all")]
        public IActionResult AllReports()
        {
            var reports = _context.Reports.Include(r => r.Project).ToList();
            return Ok(reports);
        }

        [Authorize(Roles = "mandor")]
        [HttpPut("edit/{id}")]
        public IActionResult EditReport(int id, [FromBody] Report updatedReport)
        {
            var existing = _context.Reports.FirstOrDefault(r => r.LaporanId == id);
            if (existing == null) return NotFound();

            existing.Tanggal = updatedReport.Tanggal;
            existing.Deskripsi = updatedReport.Deskripsi;
            existing.Material = updatedReport.Material;
            existing.JumlahPekerja = updatedReport.JumlahPekerja;
            existing.Kendala = updatedReport.Kendala;
            existing.Foto = updatedReport.Foto;
            _context.SaveChanges();

            return Ok(existing);
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
