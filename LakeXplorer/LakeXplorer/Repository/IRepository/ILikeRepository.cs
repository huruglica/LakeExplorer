using LakeXplorer.Models;
using System.Linq.Expressions;

namespace LakeXplorer.Repository.IRepository
{
    public interface ILikeRepository
    {
        Task<Like> GetByCondition(Expression<Func<Like, bool>> condition);
        Task Post(Like like);
        Task Delete(Like like);
    }
}
