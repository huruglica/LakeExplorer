using LakeXplorer.Data;
using LakeXplorer.Models;
using LakeXplorer.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LakeXplorer.Repository
{
    public class LakeRepository : ILakeRepository
    {
        private readonly LakeXplorerDbContext _dbContext;

        public LakeRepository(LakeXplorerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Lake>> GetAll()
        {
            return await _dbContext.Lake.ToListAsync();
        }

        public async Task<Lake> GetByCondition(Expression<Func<Lake, bool>> condition)
        {
            return await _dbContext.Lake
                .Where(condition)
                .Include(x => x.LakeSighting)
                .FirstOrDefaultAsync() ?? throw new Exception("Lake not found");
        }

        public async Task Post(Lake lake)
        {
            await _dbContext.Lake.AddAsync(lake);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Lake lake)
        {
            _dbContext.Lake.Update(lake);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Lake lake)
        {
            _dbContext.Lake.Remove(lake);
            await _dbContext.SaveChangesAsync();
        }
    }
}
