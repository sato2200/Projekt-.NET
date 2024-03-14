using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository.IRepository
{
    // Interface för ett repository som hanterar ShoppingCart-entiteter och ärver från IRepository-gränssnittet
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        // Metod för att uppdatera en varukorg
        void Update(ShoppingCart obj);
    }
}
