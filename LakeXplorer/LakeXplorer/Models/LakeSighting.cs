namespace LakeXplorer.Models
{
    public class LakeSighting
    {
        public int Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }
        public int LakeId { get; set; }
        public string FunFact { get; set; }

        public virtual User User { get; set; }
        public virtual Lake Lake { get; set; }
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
