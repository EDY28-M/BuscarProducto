using Microsoft.EntityFrameworkCore;
using BuscarProducto.Model;

namespace BuscarProducto.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Producto> Productos { get; set; }
    }
}
