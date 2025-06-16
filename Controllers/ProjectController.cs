using devtrack.AppDBContext;
using devtrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace devtrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProjectController(AppDbContext context) => _context = context;

        [Authorize(Roles = "developer")]
        [HttpGet]
        public IActionResult GetAllProjects()
        {
            var projects = _context.Projects.OrderByDescending(p => p.ProjectId).Where(p => p.Status == "Belum Mulai" || p.Status == "Sedang Berjalan").ToList();
            return Ok(projects);
        }

        [Authorize(Roles = "developer")]
        [HttpGet("{id}")]
        public IActionResult GetProject(int id)
        {
            var project = _context.Projects.SingleOrDefault(p => p.ProjectId == id);
            if (project == null)
            {
                return NotFound(new { message = "Project tidak ditemukan" });
            }
            return Ok(project);

        }

        //[Authorize(Roles = "developer")]
        [HttpPost("create")]
        public IActionResult Create([FromBody] Project project)
        {
            
            _context.Projects.Add(project);
            _context.SaveChanges();
            return Ok(project);
        }

        //[Authorize(Roles = "developer")]
        [HttpPut("edit/{id}")]
        public IActionResult EditProject(int id, [FromBody] Project updatedProject)
        {
            var project = _context.Projects.FirstOrDefault(p => p.ProjectId == id);
            if (project == null) return NotFound();

            project.NamaProject = updatedProject.NamaProject;
            project.Lokasi = updatedProject.Lokasi;
            project.Deadline = updatedProject.Deadline;
            project.Status = updatedProject.Status;
            project.Foto = updatedProject.Foto;
            _context.SaveChanges();

            return Ok(project);
        }

        //[Authorize(Roles = "developer")]
        [HttpPut("delete/{id}")]
        public IActionResult DeleteProject(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.ProjectId == id);
            if (project == null) return NotFound();
            _context.Projects.Update(project);
            _context.SaveChanges();
            return Ok(new { message = "Project deleted" });
        }

    }

}
