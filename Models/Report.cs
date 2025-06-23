using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace devtrack.Models
{
    public class Report
    {
        public int LaporanId { get; set; }
        public DateTime Tanggal { get; set; }
        public string Deskripsi { get; set; }
        public string Material { get; set; }
        public string Lokasi { get; set; }
        public int JumlahPekerja { get; set; }
        public string? Kendala {  get; set; }
        public string? Foto {  get; set; }
        public int ProjectId { get; set; }
        public int MandorProyekId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Project? Project { get; set; }
    }

    public class UpdateReportDTO
    {
        public string Deskripsi { get; set; }
        public string Material { get; set; }
        public string Lokasi { get; set; }
        public int JumlahPekerja { get; set; }
        public string? Kendala { get; set; }
        public string? Foto { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
