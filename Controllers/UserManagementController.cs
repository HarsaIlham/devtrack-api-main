using devtrack.AppDBContext;
using devtrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace devtrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserManagementController(AppDbContext context) => _context = context;

        [Authorize(Roles = "developer")]
        [HttpGet("mandor")]
        public IActionResult GetMandor()
        {
            try
            {
                var mandors = _context.Users
                                           .OrderByDescending(p => p.UserId)
                                           .Where(p => p.RoleId == 1)
                                           .ToList();

                if (mandors == null || !mandors.Any())
                {
                    return NotFound(new { message = "Data mandor tidak ditemukan." });
                }

                return Ok(mandors);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Terjadi kesalahan saat mengambil data mandor.", error = ex.Message });
            }
        }

        [Authorize(Roles = "developer,mandor")]
        [HttpGet("mandor/{id}")]
        public async Task<IActionResult> GetMandorById(int id)
        {
            var mandor = await _context.Users.FindAsync(id);
            if (mandor == null)
            {
                return NotFound($"Mandor dengan id: {id} tidak terdaftar");
            }
            return Ok(mandor);
        }

        [Authorize(Roles = "developer")]
        [HttpPost("add-mandor")]
        public IActionResult AddMandor([FromBody] User mandor)
        {
            if (_context.Users.Any(u => u.Email == mandor.Email))
                return BadRequest("Email sudah digunakan.");

            var role = _context.Roles.FirstOrDefault(r => r.RoleName.ToLower() == "mandor");
            if (role == null) return BadRequest("Role mandor tidak ditemukan");

            mandor.RoleId = role.RoleId;
            mandor.Is_active = true;
            mandor.Password = BCrypt.Net.BCrypt.HashPassword(mandor.Password);
            mandor.Role = null;

            _context.Users.Add(mandor);
            var saveUser = _context.SaveChanges();

            if (saveUser == 0) return BadRequest("Gagal menambahkan akun mandor.");

            _context.MandorProjects.Add(new MandorProject { UserId = mandor.UserId });
            _context.SaveChanges();

            return Ok(new { message = "Akun mandor berhasil dibuat"});
        }

        [Authorize(Roles = "developer")]
        [HttpPut("ToggleStatus/{id}")]
        public async Task<IActionResult> ToogleStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "user tidak ditemukan" });

            user.Is_active = !user.Is_active;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Status mandor berhasil diubah", is_active = user.Is_active });
            }
            catch (DBConcurrencyException)
            {
                return BadRequest(new { message = "Status mandor gagal diubah" });
            }
        }

        [Authorize(Roles = "developer, mandor")]
        [HttpPut("edit-mandor/{id}")]
        public async Task<IActionResult> EditMandor(int id, User updatedMandor)
        {
            var mandor = await _context.Users.FindAsync(id);
            if (mandor == null) return NotFound($"Mandor dengan id: {id} tidak terdaftar");

            mandor.Nama = updatedMandor.Nama;
            mandor.Email = updatedMandor.Email;
            if (!string.IsNullOrEmpty(updatedMandor.Password))
                mandor.Password = BCrypt.Net.BCrypt.HashPassword(updatedMandor.Password);
            mandor.Alamat = updatedMandor.Alamat;
            mandor.No_hp = updatedMandor.No_hp;
            mandor.foto = updatedMandor.foto;

            try
            {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Gagal mengedit akun mandor"});
            }
            return Ok(new { message = "Akun mandor berhasil diedit", mandor });

        }


        [Authorize(Roles = "developer,mandor")]
        [HttpPut("edit-password")]
        public IActionResult EditPassword([FromBody] ChangePasswordDto model)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var mandor = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (mandor == null) return Unauthorized("users tidak ada");

            bool isOldPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.OldPassword, mandor.Password);
            if (!isOldPasswordCorrect)
            {
                return BadRequest(new { message = "Password lama salah" });
            }

            mandor.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _context.SaveChanges();

            return Ok(new { message = "Password berhasil diperbarui" });
        }

        public class ChangePasswordDto
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }
    }
}
