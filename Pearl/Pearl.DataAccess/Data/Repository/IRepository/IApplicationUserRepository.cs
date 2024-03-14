using Pearl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pearl.DataAccess.Data.Repository.IRepository
{
    // Interface för ett repository som hanterar ApplicationUser-entiteter och ärver från IRepository-gränssnittet
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        // Inga ytterligare metoder definierade här eftersom interfacet enbart är en specialiserad version av IRepository
    }
}
