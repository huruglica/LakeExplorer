namespace LakeXplorer.Models
{
    public class Like
    {
        public string UserId { get; set; }
        public int LakeSightingId { get; set; }

        public virtual User User { get; set; }
        public virtual LakeSighting LakeSighting { get; set; }
    }
}
