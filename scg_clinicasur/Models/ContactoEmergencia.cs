using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("contactosemergencia")]
    public class ContactoEmergencia
    {
        [Key]
        [Column("id_contacto_emergencia")]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Paciente")]
        [Column("id_paciente")]
        public int IdPaciente { get; set; }
        public Usuario Paciente { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre_contacto")]
        public string NombreContacto { get; set; }

        [Required]
        [StringLength(50)]
        [Column("relacion")]
        public string Relacion { get; set; }

        [Required]
        [StringLength(15)]
        [Column("telefono_contacto")]
        public string TelefonoContacto { get; set; }

        [Required]
        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }
    }
}