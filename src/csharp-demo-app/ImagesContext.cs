using System;
using Microsoft.EntityFrameworkCore;
using OpenTracing.Mock;

namespace csharp_demo_app
{
    public class ImagesContext : DbContext
    {
        private static string host = Environment.GetEnvironmentVariable("DBHOST") ?? "sqlserver";
        private static string dbname = Environment.GetEnvironmentVariable("DBNAME") ?? "Restaurants";
        private static string username = Environment.GetEnvironmentVariable("DBUSER") ?? "restUser";
        private static string password = Environment.GetEnvironmentVariable("DBPASSWORD") ?? "restPassw0rd";

        private static int _counter;
        private string tmpHost;
        
        public ImagesContext()
        {
            tmpHost = host;
        }
        private ImagesContext(string hostName)
        {
            tmpHost = hostName;
        }

        public static ImagesContext GetBalancedContext()
        {
            if (_counter++ % 2 == 0)
                return new ImagesContext(host + "_mirror");
            return new ImagesContext();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($"Data Source={tmpHost};Initial Catalog={dbname};Persist Security Info=True;User ID={username};Password={password}");
        }

        public DbSet<ImagesEntity> ImagesData { get; set; }
    }
}