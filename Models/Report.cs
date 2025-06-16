using devtrack.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Report
{
    public int LaporanId { get; set; }
    public DateTime Tanggal { get; set; }
    public string Deskripsi { get; set; }
    public string Material { get; set; }
    public int JumlahPekerja { get; set; }
    public string? Kendala { get; set; }
    public string? Foto { get; set; }

    

    [ForeignKey("ProjectId")]
    [JsonIgnore]
    public Project? Project { get; set; }
    public int ProjectId { get; set; }
}
