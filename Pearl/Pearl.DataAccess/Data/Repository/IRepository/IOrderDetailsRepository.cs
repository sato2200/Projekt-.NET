using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository.IRepository
{
    // Interface för ett repository som hanterar OrderDetail-entiteter och ärver från IRepository-gränssnittet
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        // Metod för att uppdatera en orderdetalj
        void Update(OrderDetail obj);
    }
}
