using Microsoft.AspNetCore.Http;

namespace LakeXplorer.Models.Dto.LakeDtos
{
    public class LakeCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
