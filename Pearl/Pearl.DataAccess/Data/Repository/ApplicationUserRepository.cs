using Pearl.DataAccess.Data.Repository.IRepository;
using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository
{
    // Implementerar ett repository för ApplicationUser-entiteter och IApplicationUserRepository-gränssnittet
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        // Referens till ApplicationDbContext för databasåtkomst
        private ApplicationDbContext _db;

        // Konstruktor som tar emot ett ApplicationDbContext-objekt
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            // Initialiserar den lokala _db-variabeln med det angivna db-objektet
            _db = db;
        }
    }
}
