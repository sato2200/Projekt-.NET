using Pearl.DataAccess.Data.Repository.IRepository;
using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository
{
    // Implementerar ett enhetsverk för att hantera flera repositories och IUnitOfWork-gränssnittet
    public class UnitOfWork : IUnitOfWork
    {
        // Referens till ApplicationDbContext för databasåtkomst
        private ApplicationDbContext _db;


        // Egenskaper för att få åtkomst till olika repositories
        public ICategoryRepository Category { get; private set; }
		public IProductRepository Product { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }



        // Konstruktor som tar emot ett ApplicationDbContext-objekt
        public UnitOfWork(ApplicationDbContext db)
		{
            // Initialiserar den lokala _db-variabeln med det angivna db-objektet
            _db = db;


            // Instansierar repositories och tilldelar dem till egenskaperna
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            Category = new CategoryRepository(_db);
			Product = new ProductRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
        }



        // Metod för att spara ändringar i databasen
        public void Save()
		{
            // Sparar ändringar i databasen
            _db.SaveChanges();
		}
	}
}
