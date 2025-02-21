using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    public class Expediente
    {
        [Key]
        [Column("id_expediente")]  // Mapea a la columna 'id_expediente' en la base de datos
        public int idExpediente { get; set; }

        [Required(ErrorMessage = "El nombre del paciente es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre del paciente no puede exceder los 255 caracteres.")]
        
        [Column("nombre_paciente")]  // Mapea a la columna 'nombre_paciente' en la base de datos
        public string nombrePaciente { get; set; }

        [Required(ErrorMessage = "El ID del paciente es obligatorio.")]
        [Column("id_paciente")]  // Mapea a la columna 'id_paciente' en la base de datos
        public int idPaciente { get; set; }

        [Required(ErrorMessage = "La fecha de la última consulta es obligatoria.")]
        [DataType(DataType.DateTime, ErrorMessage = "La fecha de la última consulta debe ser válida.")]
        [Column("ultima_consulta")]  // Mapea a la columna 'ultima_consulta' en la base de datos
        public DateTime ultimaConsulta { get; set; }

        [Required(ErrorMessage = "El diagnóstico es obligatorio.")]
        [StringLength(1000, ErrorMessage = "El diagnóstico no puede exceder los 1000 caracteres.")]
        [Column("diagnostico")]  // Mapea a la columna 'diagnostico' en la base de datos
        public string diagnostico { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(2000, ErrorMessage = "La descripción no puede exceder los 2000 caracteres.")]
        [Column("descripcion")]  // Mapea a la columna 'descripcion' en la base de datos
        public string descripcion { get; set; }

        [StringLength(2000, ErrorMessage = "El tratamiento previo no puede exceder los 2000 caracteres.")]
        [Column("tratamientos_previos")]  // Mapea a la columna 'tratamientos_previos' en la base de datos
        public string tratamientosPrevios { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Column("fecha_creacion")]  // Mapea a la columna 'fecha_creacion' en la base de datos
        public DateTime fechaCreacion { get; set; }

        // Relación con ResultadosLaboratorio
        //public ICollection<ResultadosLaboratorio?> ResultadosLaboratorio { get; set; }  // Colección de resultados de laboratorio
    }
}