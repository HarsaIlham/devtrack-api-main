using System.Text.Json.Serialization;

namespace devtrack.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string NamaProject { get; set; }
        public string Lokasi { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public string? Foto { get; set; }

    }
}
