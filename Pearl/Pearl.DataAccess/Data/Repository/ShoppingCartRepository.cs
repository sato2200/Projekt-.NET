using Pearl.DataAccess.Data.Repository.IRepository;
using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository
{
    // Implementerar ett repository för ShoppingCart-entiteter och IShoppingCartRepository-gränssnittet
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        // Referens till ApplicationDbContext för databasåtkomst
        private ApplicationDbContext _db;

        // Konstruktor som tar emot ett ApplicationDbContext-objekt
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            // Initialiserar den lokala _db-variabeln med det angivna db-objektet
            _db = db;
        }



        // Metod för att uppdatera en varukorg i databasen
        public void Update(ShoppingCart obj)
        {
            // Uppdaterar den angivna varukorgen i databasen
            _db.ShoppingCarts.Update(obj);
        }
    }
}
