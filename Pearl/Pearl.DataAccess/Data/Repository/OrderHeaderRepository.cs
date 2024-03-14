using Pearl.DataAccess.Data.Repository.IRepository;
using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository
{
    // Implementerar ett repository för OrderHeader-entiteter och IOrderHeaderRepository-gränssnittet
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        // Referens till ApplicationDbContext för databasåtkomst
        private ApplicationDbContext _db;

        // Konstruktor som tar emot ett ApplicationDbContext-objekt
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            // Initialiserar den lokala _db-variabeln med det angivna db-objektet
            _db = db;
        }


        // Metod för att uppdatera en orderheader i databasen
        public void Update(OrderHeader obj)
        {
            // Uppdaterar den angivna orderheadern i databasen
            _db.OrderHeaders.Update(obj);
        }



        // Metod för att uppdatera status för en order
        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            // Hämtar ordern från databasen baserat på id
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u=>u.Id==id);

            // Uppdatera orderstatus och betalningsstatus (om den är tillhandahållen) om ordern finns i databasen
            if (orderFromDb!=null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if(!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }



        // Metod för att uppdatera Stripe-betalnings-ID för en order
        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            // Hämtar ordern från databasen baserat på id
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);

            // Uppdatera session-ID och betalnings-ID för ordern om de är tillhandahållna
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId= sessionId;
            }

            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                // Uppdatera betalningsdatum till nuvarande tidpunkt
                orderFromDb.PaymentDate=DateTime.Now;
            }
        }
    }
}
