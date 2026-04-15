//using Microsoft.EntityFrameworkCore;
//using ApexOMS_Web.Models;

//namespace ApexOMS_Web.Data
//{
//    public class ApexDbContext : DbContext
//    {
//        public ApexDbContext(DbContextOptions<ApexDbContext> options) : base(options)
//        {
//        }

//        // This links your C# model to the SQL table
//        public DbSet<InventoryOrder> InventoryOrders { get; set; }
//        public DbSet<User> Users { get; set; }
//        public DbSet<ArticleLast> ArticleLasts { get; set; }



//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            // Map the C# class to the exact SQL table name
//            modelBuilder.Entity<InventoryOrder>().ToTable("tbl_invent_order");

//            // Define the Primary Key
//            modelBuilder.Entity<InventoryOrder>().HasKey(o => o.sl);

//            // Inside OnModelCreating, add:
//            modelBuilder.Entity<ArticleLast>().ToTable("tbl_article_last");
//        }
//    }
//}







using Microsoft.EntityFrameworkCore;
using ApexOMS_Web.Models;

namespace ApexOMS_Web.Data
{
    public class ApexDbContext : DbContext
    {
        public ApexDbContext(DbContextOptions<ApexDbContext> options) : base(options) { }

        public DbSet<InventoryOrder> InventoryOrders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ArticleLast> ArticleLasts { get; set; } // The new bridge
        public DbSet<Bom> Boms { get; set; }
        public DbSet<DashboardData> Dashboards { get; set; }

     

        // Inside OnModelCreating:

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryOrder>().ToTable("tbl_invent_order");
            modelBuilder.Entity<User>().ToTable("tbl_user");
            modelBuilder.Entity<ArticleLast>().ToTable("tbl_article_last");
            modelBuilder.Entity<Bom>().ToTable("tbl_BOM");  
            modelBuilder.Entity<DashboardData>().ToTable("DashboardData");
        }
    }
}