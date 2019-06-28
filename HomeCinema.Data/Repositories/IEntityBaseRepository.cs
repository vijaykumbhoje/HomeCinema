using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Entities;

namespace HomeCinema.Data.Repositories
{
    public interface IEntityBaseRepository<T> where T: class, IEntityBase, new ()  
    {
        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] IncludeProperties);

        IQueryable<T> All { get; }

        IQueryable<T> GetAll();

        T GetSingle(int Id);

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        void Add(T entity);

        void Edit(T entity);

        void Delete(T entity);
    }
}
