using Pearl.DataAccess.Data.Repository.IRepository;
using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository
{
    // Implementerar ett repository för Category-entiteter och ICategoryRepository-gränssnittet
    public class CategoryRepository : Repository<Category>, ICategoryRepository
	{
        // Referens till ApplicationDbContext för databasåtkomst
        private ApplicationDbContext _db;

        // Konstruktor som tar emot ett ApplicationDbContext-objekt
        public CategoryRepository(ApplicationDbContext db) : base(db)
		{
            // Initialiserar den lokala _db-variabeln med det angivna db-objektet
            _db = db;
		}

        // Metod för att uppdatera en kategori i databasen
        public void Update(Category obj)
		{
            // Uppdaterar den angivna kategorin i databasen
            _db.Categories.Update(obj);
		}
	}
}

