using System.ComponentModel.DataAnnotations;

namespace scg_clinicasur.Models
{
    public class CambiarContraseña
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y un máximo de {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NuevaContraseña { get; set; }

        [DataType(DataType.Password)]
        [Compare("NuevaContraseña", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmarContraseña { get; set; }
    }
}
