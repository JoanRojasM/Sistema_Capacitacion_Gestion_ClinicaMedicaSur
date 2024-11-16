namespace scg_clinicasur.Models
{
    public class GestionarAlergiasViewModel
    {
        public Usuario Paciente { get; set; }
        public List<Alergia> TodasAlergias { get; set; }
        public List<int> AlergiasPaciente { get; set; } // IDs de las alergias actuales del paciente
        public List<dynamic> HistorialAlergias { get; set; }
    }
}
