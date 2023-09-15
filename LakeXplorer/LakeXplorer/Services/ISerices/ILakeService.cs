using LakeXplorer.Models.Dto.LakeDtos;
using LakeXplorer.Models.Dto.LakeSightingDtos;

namespace LakeXplorer.Services.ISerices
{
    public interface ILakeService
    {
        Task<List<LakeViewDto>> GetAll();
        Task<LakeViewDto> GetById(int id);
        Task Post(LakeCreateDto lakeCreateDto);
        Task Update(int id, LakeUpdateDto lakeUpdateDto);
        Task AddLakeSighting(int id, LakeSightingCreateDto lakeSightingCreateDto);
        Task Delete(int id);
    }
}
