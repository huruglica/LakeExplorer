using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LakeXplorer.Models
{
    public class User
    {
        public string Id { get; set; }
        [Column(TypeName = "varchar")]
        [MaxLength(15)]
        public string Username { get; set; }
        [Column(TypeName = "varchar")]
        [MaxLength(120)]
        public string Email { get; set; }
        public byte[] Key { get; set; }
        public byte[] PasswordHash { get; set; }
        [Column(TypeName = "varchar")]
        [MaxLength(10)]
        public string Role { get; set; }

        public virtual ICollection<Lake> Lakes { get; set; } = new List<Lake>();
    }
}
