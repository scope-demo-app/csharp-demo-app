using System;
using Microsoft.EntityFrameworkCore;

namespace csharp_demo_app
{
    public class ImagesContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var host = Environment.GetEnvironmentVariable("DBHOST") ?? "sqlserver";
            var dbname = Environment.GetEnvironmentVariable("DBNAME") ?? "Restaurants";
            var username = Environment.GetEnvironmentVariable("DBUSER") ?? "restUser";
            var password = Environment.GetEnvironmentVariable("DBPASSWORD") ?? "restPassw0rd";
            optionsBuilder.UseSqlServer($"Data Source={host};Initial Catalog={dbname};Persist Security Info=True;User ID={username};Password={password}");
        }

        public DbSet<ImagesEntity> ImagesData { get; set; }
    }
}