using LakeXplorer.Models.Dto.LakeDtos;

namespace LakeXplorer.Models.Dto.UserDtos
{
    public class UserViewDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public ICollection<LakeViewDto> Lakes { get; set; } = new List<LakeViewDto>();
    }
}
