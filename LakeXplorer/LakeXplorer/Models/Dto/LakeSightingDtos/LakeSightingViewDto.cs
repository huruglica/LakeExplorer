using LakeXplorer.Models.Dto.LakeDtos;
using LakeXplorer.Models.Dto.LikeDto;
using LakeXplorer.Models.Dto.UserDtos;

namespace LakeXplorer.Models.Dto.LakeSightingDtos
{
    public class LakeSightingViewDto
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public byte[] Image { get; set; }
        public UserViewDto User { get; set; }
        public ICollection<LikeViewDto> Likes { get; set; } = new List<LikeViewDto>();
    }
}
