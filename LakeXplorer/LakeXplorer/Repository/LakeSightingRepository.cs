using LakeXplorer.Data;
using LakeXplorer.Models;
using LakeXplorer.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LakeXplorer.Repository
{
    public class LakeSightingRepository : ILakeSightingRepository
    {
        private readonly LakeXplorerDbContext _dbContext;

        public LakeSightingRepository(LakeXplorerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<LakeSighting>> GetAll()
        {
            return await _dbContext.LakeSighting.ToListAsync();
        }

        public async Task<LakeSighting> GetByCondition(Expression<Func<LakeSighting, bool>> condition)
        {
            return await _dbContext.LakeSighting
                .Where(condition)
                .Include(x => x.User)
                .Include(x => x.Likes)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync() ?? throw new Exception("Lake Sighting not found");
        }

        public async Task Post(LakeSighting lakeSighting)
        {
            await _dbContext.LakeSighting.AddAsync(lakeSighting);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(LakeSighting lakeSighting)
        {
            _dbContext.LakeSighting.Update(lakeSighting);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(LakeSighting lakeSighting)
        {
            _dbContext.LakeSighting.Remove(lakeSighting);
            await _dbContext.SaveChangesAsync();
        }
    }
}
