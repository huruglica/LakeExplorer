namespace LakeXplorer.Models.Dto.LakeSightingDtos
{
    public class LakeSightingCreateDto
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
