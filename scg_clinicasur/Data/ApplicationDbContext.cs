using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Models;

namespace scg_clinicasur.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<scg_clinicasur.Models.Usuario> Usuarios { get; set; }
        public DbSet<scg_clinicasur.Models.Roles> Roles { get; set; }
        public DbSet<scg_clinicasur.Models.Expediente> Expedientes { get; set; }
        public DbSet<scg_clinicasur.Models.ResultadosLaboratorio> ResultadosLaboratorio { get; set; }
        public virtual DbSet<scg_clinicasur.Models.Evaluacion> Evaluaciones { get; set; }
        public DbSet<scg_clinicasur.Models.Capacitacion> Capacitaciones { get; set; }
        public DbSet<scg_clinicasur.Models.Contabilidad> Contabilidades { get; set; }
        public DbSet<scg_clinicasur.Models.EstadoCita> EstadoCitas { get; set; }
        public DbSet<scg_clinicasur.Models.Cita> Citas { get; set; }
        public DbSet<scg_clinicasur.Models.DisponibilidadDoctor> DisponibilidadDoctor { get; set; }
        public DbSet<scg_clinicasur.Models.Alergia> Alergias { get; set; }
        public DbSet<scg_clinicasur.Models.PacienteAlergia> PacienteAlergias { get; set; }
        public DbSet<scg_clinicasur.Models.ContactoEmergencia> ContactosEmergencia { get; set; }
        public DbSet<scg_clinicasur.Models.AntecedenteFamiliar> AntecedentesFamiliares { get; set; }
        public DbSet<scg_clinicasur.Models.HabitoVida> HabitosVida { get; set; }
        public DbSet<scg_clinicasur.Models.MedicamentoPrescrito> MedicamentosPrescritos { get; set; }
        public DbSet<scg_clinicasur.Models.RecursosAprendizaje> RecursosAprendizaje { get; set; }
        public DbSet<scg_clinicasur.Models.Notificacion> Notificaciones { get; set; }

    }
}