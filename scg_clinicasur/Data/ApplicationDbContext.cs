﻿using Microsoft.EntityFrameworkCore;

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

    }
}