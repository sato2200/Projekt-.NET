using Pearl.DataAccess.Data.Repository.IRepository;
using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository
{
    // Implementerar ett repository för OrderDetail-entiteter och IOrderDetailRepository-gränssnittet
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        // Referens till ApplicationDbContext för databasåtkomst
        private ApplicationDbContext _db;

        // Konstruktor som tar emot ett ApplicationDbContext-objekt
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            // Initialiserar den lokala _db-variabeln med det angivna db-objektet
            _db = db;
        }


        // Metod för att uppdatera en orderdetalj i databasen
        public void Update(OrderDetail obj)
        {
            // Uppdaterar den angivna orderdetaljen i databasen
            _db.OrderDetails.Update(obj);
        }
    }
}
