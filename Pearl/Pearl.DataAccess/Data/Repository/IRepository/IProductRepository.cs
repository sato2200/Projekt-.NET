using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository.IRepository
{
    // Interface för ett repository som hanterar Product-entiteter och ärver från IRepository-gränssnittet
    public interface IProductRepository : IRepository<Product>
	{
        // Metod för att uppdatera en produkt
        void Update(Product obj);
	}
}
