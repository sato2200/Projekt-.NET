using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository.IRepository
{
    // Interface för ett repository som hanterar OrderHeader-entiteter och ärver från IRepository-gränssnittet
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        // Metod för att uppdatera en orderheader
        void Update(OrderHeader obj);

        // Metod för att uppdatera status för en order med möjlighet att uppdatera betalningsstatus
        void UpdateStatus(int id, string orderStatus, string? paymentStatus=null);

        // Metod för att uppdatera Stripe-betalnings-ID för en order
        void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
    }
}
