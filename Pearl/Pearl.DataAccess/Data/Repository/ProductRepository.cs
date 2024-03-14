using Pearl.DataAccess.Data.Repository.IRepository;
using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository
{
    // Implementerar ett repository för Product-entiteter och IProductRepository-gränssnittet
    public class ProductRepository : Repository<Product>, IProductRepository
	{
        // Referens till ApplicationDbContext för databasåtkomst
        private ApplicationDbContext _db;
		public ProductRepository(ApplicationDbContext db) : base(db)
		{
            // Initialiserar den lokala _db-variabeln med det angivna db-objektet
            _db = db;
		}


        // Metod för att uppdatera en produkt i databasen
        public void Update(Product obj)
		{
            // Hämtar produkten från databasen baserat på id
            var objFromDb =_db.Products.FirstOrDefault(u=>u.Id == obj.Id);

            // Uppdatera produktdetaljer om produkten finns i databasen
            if (objFromDb != null)
			{
				//Uppdatera titel, beskrivning, kategori, listpris
				objFromDb.Title = obj.Title;
				objFromDb.Description = obj.Description;
				objFromDb.CategoryId = obj.CategoryId;
				objFromDb.ListPrice = obj.ListPrice;
                // Uppdatera bild-URL om den nya bilden är tillhandahållen
                if (obj.ImageUrl != null)
				{
					objFromDb.ImageUrl = obj.ImageUrl;
				}
			}
		}
	}
}

