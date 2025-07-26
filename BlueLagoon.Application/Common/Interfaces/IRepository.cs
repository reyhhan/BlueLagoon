using System.Linq.Expressions;

namespace BlueLagoon.Application.Common.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = false);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        bool Any(Expression<Func<T, bool>> filter);
        void Add(T villa);
        void Delete(T villa);
    }
}
