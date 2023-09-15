using LakeXplorer.Models.Dto.LakeSightingDtos;

namespace LakeXplorer.Models.Dto.LakeDtos
{
    public class LakeViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public ICollection<LakeSightingViewDto> LakeSighting { get; set; } = new List<LakeSightingViewDto>();
    }
}
