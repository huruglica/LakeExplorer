using LakeXplorer.Models;

namespace LakeXplorer.Services.ISerices
{
    public interface ILikeService
    {
        Task<Like> GetById(int lakeSightingId, string userId);
        Task Post(int lakeSightingId, string userId);
        Task Delet(int lakeSightingId, string userId);
    }
}
