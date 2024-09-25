using Microsoft.EntityFrameworkCore;

namespace scg_clinicasur.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<scg_clinicasur.Models.Usuario> Usuarios { get; set; }
        public DbSet<scg_clinicasur.Models.Rol> Roles { get; set; }
    }
}