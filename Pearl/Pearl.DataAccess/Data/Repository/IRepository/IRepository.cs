using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository.IRepository
{
    // Interface för ett generiskt repository som hanterar grundläggande CRUD-operationer för en given entitet
    public interface IRepository<T> where T : class
	{
        // Hämtar alla entiteter baserat på ett filter och eventuella inkluderade egenskaper
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        // Hämtar en enskild entitet baserat på ett filter och eventuella inkluderade egenskaper
        T Get(Expression<Func<T, bool>> filter, string? includeProperties=null, bool tracked = false);

        // Lägger till en entitet i databasen
        void Add(T entity);

        // Tar bort en enskild entitet från databasen
        void Remove(T entity);

        // Tar bort en samling av entiteter från databasen
        void RemoveRange(IEnumerable<T> entity);
	}

}
