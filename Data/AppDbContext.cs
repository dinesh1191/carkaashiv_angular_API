using carkaashiv_angular_API.Models;
using Microsoft.EntityFrameworkCore;

namespace carkaashiv_angular_API.Data
{
  
        public class AppDbContext : DbContext
        {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //medille ware configuration 
        }
        
        public DbSet<TablePart> tbl_part { get; set; } //Maps to parts table
            public DbSet<TableEmployee> tbl_emp { get; set; } //Maps to employee table
            public DbSet<TableUser> tbl_user { get; set; }  //Maps to user Table
            public DbSet<TableCart> tbl_cart { get; set; }  //Maps to cart table
            public DbSet<TableOrders> tbl_orders { get; set; }  //Maps to order table
            public DbSet<TableOrderItems> tbl_order_items { get; set; }  //Maps to cart orderItems
       
        // adding precision for columns store moneytery values
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TableOrderItems>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.totalPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<TableOrders>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<TablePart>(entity =>
            {
                entity.Property(e => e.PPrice).HasPrecision(18, 2);
            });
        }
    }


    
}
