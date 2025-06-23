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
            try
            {
                var projects = _context.Projects
                                        .OrderByDescending(p => p.ProjectId)
                                        .ToList();

                if (projects == null || !projects.Any())
                {
                    return NotFound(new { message = "Projek tidak ditemukan." });
                }

                return Ok(projects);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil data projek.", error = ex.Message });
            }
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

        [Authorize(Roles = "developer")]
        [HttpPost("create")]
        public IActionResult Create([FromBody] Project project)
        {
            try
            {
                if (project == null)
                {
                    return BadRequest(new { message = "Data projek tidak boleh kosong." });
                }

                _context.Projects.Add(project);
                _context.SaveChanges();

                return Ok(new { message = "Proyek berhasil ditambahkan" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat menambah projek.", error = ex.Message });
            }
        }

        [Authorize(Roles = "developer")]
        [HttpPut("edit/{id}")]
        public IActionResult EditProject(int id, [FromBody] Project updatedProject)
        {
            try
            {
                if (updatedProject == null)
                {
                    return BadRequest(new { message = "Data projek tidak boleh kosong." });
                }

                var project = _context.Projects.FirstOrDefault(p => p.ProjectId == id);
                if (project == null)
                {
                    return NotFound(new { message = $"Projek dengan ID {id} tidak ditemukan." });
                }

                project.NamaProject = updatedProject.NamaProject;
                project.Lokasi = updatedProject.Lokasi;
                project.Deadline = updatedProject.Deadline;
                project.Status = updatedProject.Status;
                project.Foto = updatedProject.Foto;

                _context.SaveChanges();

                return Ok(new { message = "Proyek berhasil diedit", project });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengedit projek.", error = ex.Message });
            }
        }


    }

}
