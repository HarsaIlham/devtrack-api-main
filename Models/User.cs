using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace devtrack.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Nama { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string Alamat {  get; set; }
        public string No_hp { get; set; }
        public string? foto { get; set; }
        public bool Is_active { get; set; }
        [JsonIgnore]
        public Role? Role { get; set; }
    }



}
