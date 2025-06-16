namespace devtrack.Models
{
    public class MandorProject
    {
        public int MandorProyekId { get; set; }
        public int UserId { get; set; }
        public bool IsWorking { get; set; }
        public User User { get; set; }
    }


}
