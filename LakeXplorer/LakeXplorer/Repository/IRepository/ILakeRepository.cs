using LakeXplorer.Models;
using System.Linq.Expressions;

namespace LakeXplorer.Repository.IRepository
{
    public interface ILakeRepository
    {
        Task<List<Lake>> GetAll();
        Task<Lake> GetByCondition(Expression<Func<Lake, bool>> condition);
        Task Post(Lake lake);
        Task Update(Lake lake);
        Task Delete(Lake lake);
    }
}
