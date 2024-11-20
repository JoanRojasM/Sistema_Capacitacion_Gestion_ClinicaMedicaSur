using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("contabilidad")]
    public class Contabilidad
    {
        [Key]
        [Column("id_contabilidad")]
        public int IdContabilidad { get; set; }

        [Required]
        [Column("concepto")]
        [StringLength(255)]
        public string Concepto { get; set; }

        [Required]
        [Column("monto")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Monto { get; set; }

        [Required]
        [Column("tipo")]
        [StringLength(50)]
        public string Tipo { get; set; } // Solo puede ser "Gasto" o "Ingreso"

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}