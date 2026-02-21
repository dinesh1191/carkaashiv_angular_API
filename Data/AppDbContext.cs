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
        
        public DbSet<Part> tbl_part { get; set; } //Maps to parts table
            public DbSet<Employee> tbl_emp { get; set; } //Maps to employee table
            public DbSet<User> tbl_user { get; set; }  //Maps to user Table
            public DbSet<Cart> tbl_cart { get; set; }  //Maps to cart table
            public DbSet<Orders> tbl_orders { get; set; }  //Maps to order table
            public DbSet<OrderItems> tbl_order_items { get; set; }  //Maps to cart orderItems
       
        // adding precision for columns store moneytery values
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItems>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.totalPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Part>(entity =>
            {
                entity.Property(e => e.PPrice).HasPrecision(18, 2);
            });
        }
    }


    
}
