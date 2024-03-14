using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pearl.Models;

namespace Pearl.DataAccess.Data
{
    // Definiera ApplicationDbContext, som ärver från IdentityDbContext och parametriseras med IdentityUser
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
	{
        // Konstruktorn för ApplicationDbContext tar emot DbContextOptions<ApplicationDbContext>
        // för att konfigurera databaskontexten.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

        // DbSet som representerar en tabell i databasen för varje entitet som behöver hanteras
        public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        // Metoden OnModelCreating används för att konfigurera modellen när databaskontexten skapas.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            // Anropa basens OnModelCreating-metod för att tillåta Identity-hantering.
            base.OnModelCreating(modelBuilder);

           
			}
		}
}



