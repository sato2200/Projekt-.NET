using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Pearl.DataAccess.Data;
using Pearl.DataAccess.Data.Repository.IRepository;

namespace Pearl.DataAccess.Data.Repository
{
    // Generiskt repository som hanterar CRUD-operationer för en given entitet
    public class Repository<T> : IRepository<T> where T : class
	{
        // Referens till DbContext
        private readonly ApplicationDbContext _db;
        // DbSet för den generiska typen
        internal DbSet<T> dbSet;



        // Konstruktor som tar emot ett ApplicationDbContext-objekt
        public Repository(ApplicationDbContext db)
		{
			_db = db;
            // Initialisering av DbSet
            this.dbSet = _db.Set<T>();
            // Inkludera kopplade entiteter vid första instansen
            _db.Products.Include(u => u.Category).Include(u=>u.CategoryId);
		}



        // Lägger till en entitet i DbSet
        public void Add(T entity)
		{
			dbSet.Add(entity);
		}


        // Hämtar en entitet baserat på ett filter och eventuella inkluderade egenskaper
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
		{
            IQueryable<T> query;
            if (tracked)
            {
                query = dbSet;
            }
            else
            {
                // Om spårning inte är aktiverat, använd AsNoTracking för att förbättra prestanda
                query = dbSet.AsNoTracking();
            }

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {                
				foreach (var includeProp in includeProperties
                     .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // Inkludera associerade entiteter baserat på inkluderade egenskaper
                    query = query.Include(includeProp);
				}
			}
            // Returnera den första matchande entiteten
            return query.FirstOrDefault();
		}



        // Hämtar alla entiteter baserat på ett filter och eventuella inkluderade egenskaper
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties=null)
		{
			//Filter för att hämta cart från EN viss användare
			IQueryable<T> query = dbSet;
            if (filter != null)
            {
                // Applicera filter om det finns
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeProp in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
                    // Inkludera associerade entiteter baserat på inkluderade egenskaper
                    query = query.Include(includeProp);
				}
			}
            // Returnera en lista av matchande entiteter
            return query.ToList();
		}



        // Tar bort en enskild entitet från DbSet
        public void Remove(T entity)
		{
			dbSet.Remove(entity);
		}



        // Tar bort en samling av entiteter från DbSet
        public void RemoveRange(IEnumerable<T> entity)
		{
			dbSet.RemoveRange(entity);
		}
	}
}
