using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LakeXplorer.Models
{
    public class Lake
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar")]
        [MaxLength(15)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<LakeSighting>? LakeSighting { get; set; } = new List<LakeSighting>();
    }
}
