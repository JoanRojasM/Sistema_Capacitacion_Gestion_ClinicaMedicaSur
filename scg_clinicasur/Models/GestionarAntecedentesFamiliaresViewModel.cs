namespace scg_clinicasur.Models
{
    public class GestionarAntecedentesFamiliaresViewModel
    {
        public Usuario Paciente { get; set; }
        public List<AntecedenteFamiliar> AntecedentesFamiliares { get; set; }
        public AntecedenteFamiliar UltimoAntecedente { get; set; }
    }
}
