using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository.IRepository
{
    // Interface för ett repository som hanterar Category-entiteter och ärver från IRepository-gränssnittet
    public interface ICategoryRepository : IRepository<Category>
	{
        // Metod för att uppdatera en kategori
        void Update(Category obj);
		
	}

}
