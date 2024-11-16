namespace scg_clinicasur.Models
{
    public class GestionarHabitosVidaViewModel
    {
        public Usuario Paciente { get; set; }
        public List<HabitoVida> HabitosVida { get; set; }
        public HabitoVida UltimoHabito { get; set; }
    }
}
