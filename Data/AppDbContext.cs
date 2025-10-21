using carkaashiv_angular_API.Models;
using Microsoft.EntityFrameworkCore;

namespace carkaashiv_angular_API.Data
{
  
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
            {
            }
            public DbSet<TablePart> tbl_part { get; set; }
        }

    
}
