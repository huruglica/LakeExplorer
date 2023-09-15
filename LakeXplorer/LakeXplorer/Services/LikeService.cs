using LakeXplorer.Models;
using LakeXplorer.Repository.IRepository;
using LakeXplorer.Services.ISerices;
using System.Linq.Expressions;

namespace LakeXplorer.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;

        public LikeService(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        public async Task<Like> GetById(int lakeSightingId, string userId)
        {
            Expression<Func<Like, bool>> condition = x => x.LakeSightingId == lakeSightingId && x.UserId.Equals(userId);
            return await _likeRepository.GetByCondition(condition);
        }

        public async Task Post(int lakeSightingId, string userId)
        {
            var like = new Like
            {
                LakeSightingId = lakeSightingId,
                UserId = userId
            };

            await _likeRepository.Post(like);
        }

        public async Task Delet(int lakeSightingId, string userId)
        {
            var like = await GetById(lakeSightingId, userId);
            await _likeRepository.Delete(like);
        }
    }
}
