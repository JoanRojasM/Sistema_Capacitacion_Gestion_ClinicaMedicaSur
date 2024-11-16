namespace scg_clinicasur.Models
{
    public class GestionarMedicamentosViewModel
    {
        public Usuario Paciente { get; set; }
        public List<MedicamentoPrescrito> MedicamentosActivos { get; set; }
        public List<MedicamentoPrescrito> HistorialMedicamentos { get; set; }
    }
}
