namespace scg_clinicasur.Models
{
    public class GestionarContactoEmergenciaViewModel
    {
        public Usuario Paciente { get; set; }
        public ContactoEmergencia ContactoEmergenciaActual { get; set; }
        public List<ContactoEmergencia> HistorialContactoEmergencia { get; set; }
    }
}
