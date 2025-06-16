using System.Text.Json.Serialization;

namespace devtrack.Models
{
    public class Project
    {
        public Project()
        {
            Reports = new List<Report>(); // ✅ inisialisasi untuk menghindari null
        }

        public int ProjectId { get; set; }
        public string NamaProject { get; set; }
        public string Lokasi { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public string? Foto { get; set; }
        [JsonIgnore]
        public ICollection<Report> Reports { get; set; }
    }
}
