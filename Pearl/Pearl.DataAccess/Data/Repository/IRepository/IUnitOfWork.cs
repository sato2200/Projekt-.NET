using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository.IRepository
{
    // Interface för ett enhetsverk som hanterar flera repositories
    public interface IUnitOfWork
	{
        // Egenskaper för att få åtkomst till olika repositories
        ICategoryRepository Category { get; }
		IProductRepository Product { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderHeaderRepository OrderHeader { get; }

        // Metod för att spara ändringar i databasen
        void Save();
	}
}
