using LakeXplorer.Data;
using LakeXplorer.Models;
using LakeXplorer.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LakeXplorer.Repository
{
    public class LikeRepository : ILikeRepository
    {
        private readonly LakeXplorerDbContext _dbContext;

        public LikeRepository(LakeXplorerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Like> GetByCondition(Expression<Func<Like, bool>> condition)
        {
            return await _dbContext.Like
                .Where(condition)
                .FirstOrDefaultAsync()
                ?? throw new Exception("This user has not liked this LakeSighting");
        } 

        public async Task Post(Like like)
        {
            await _dbContext.Like.AddAsync(like);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Like like)
        {
            _dbContext.Like.Remove(like);
            await _dbContext.SaveChangesAsync();
        }
    }
}
