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
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var role = User.FindFirst(ClaimTypes.Role).Value;

            if (role == "mandor")
            {
                var mandorId = _context.MandorProjects.FirstOrDefault(m => m.UserId == userId)?.MandorProyekId;
                var ids = _context.MandorProjectProjects.Where(m => m.MandorProyekId == mandorId).Select(m => m.ProjectId);
                return Ok(_context.Riwayats.Where(r => ids.Contains(r.ProjectId)).ToList());
            }

            return Ok(_context.Riwayats.ToList());
        }

        [Authorize(Roles = "developer")]
        [HttpGet("developer")]
        public IActionResult DeveloperRiwayat()
        {
            return Ok(_context.Riwayats.Include(r => r.Project).ToList());
        }

        [Authorize(Roles = "mandor")]
        [HttpGet("mandor")]
        public IActionResult MandorRiwayat()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var mandorId = _context.MandorProjects.FirstOrDefault(m => m.UserId == userId)?.MandorProyekId;
            var projectIds = _context.MandorProjectProjects
                .Where(m => m.MandorProyekId == mandorId)
                .Select(m => m.ProjectId)
                .ToList();

            var riwayat = _context.Riwayats
                .Include(r => r.Project)
                .Where(r => projectIds.Contains(r.ProjectId))
                .ToList();

            return Ok(riwayat);
        }
    }

}
