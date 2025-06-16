namespace devtrack.Models
{
    public class MandorProjectProject
    {
        public int Id { get; set; }
        public int MandorProyekId { get; set; }
        public MandorProject MandorProject { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }


}
