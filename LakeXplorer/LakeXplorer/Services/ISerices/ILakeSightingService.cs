using LakeXplorer.Models;
using LakeXplorer.Models.Dto.LakeSightingDtos;

namespace LakeXplorer.Services.ISerices
{
    public interface ILakeSightingService
    {
        Task<List<LakeSightingViewDto>> GetAll();
        Task<LakeSightingViewDto> GetById(int id);
        Task Post(int lakeId, LakeSightingCreateDto lakeSightingCreateDto);
        Task DailyFunFact();
        Task Update(int id, LakeSightingUpdateDto lakeSightingUpdateDto);
        Task Like(int id);
        Task Delete(int id);
        Task Dislike(int id);
        Task Delete(LakeSighting lakeSighting);
    }
}
