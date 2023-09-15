using LakeXplorer.Models;
using System.Linq.Expressions;

namespace LakeXplorer.Repository.IRepository
{
    public interface ILakeSightingRepository
    {
        Task<List<LakeSighting>> GetAll();
        Task<LakeSighting> GetByCondition(Expression<Func<LakeSighting, bool>> condition);
        Task Post(LakeSighting lakeSighting);
        Task Update(LakeSighting lakeSighting);
        Task Delete(LakeSighting lakeSighting);
    }
}
