using Microsoft.EntityFrameworkCore;

namespace csharp_demo_app
{
    public class ImagesContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Restaurants;Persist Security Info=True;User ID=restUser;Password=restPassw0rd");
        }

        public DbSet<ImagesEntity> ImagesData { get; set; }
    }
    
}