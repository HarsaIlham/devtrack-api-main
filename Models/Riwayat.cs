﻿using System.Text.Json.Serialization;

namespace devtrack.Models
{
    public class Riwayat
    {
        public int RiwayatId { get; set; }
        public DateTime TanggalSelesai { get; set; }
        public string? Catatan { get; set; }
        public int ProjectId { get; set; }
        [JsonIgnore]
        public Project? Project { get; set; }
    }

}
