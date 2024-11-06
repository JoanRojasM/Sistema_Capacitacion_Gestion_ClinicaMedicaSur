using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;

public class CalendarioController : Controller
{
    private readonly ApplicationDbContext _context;

    public CalendarioController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Obtener citas por mes
    [HttpGet]
    public async Task<IActionResult> ObtenerCitasPorMes(int year, int month)
    {
        var citas = await _context.Citas
            .Where(c => c.FechaInicio.Year == year && c.FechaInicio.Month == month)
            .Select(c => new {
                c.IdCita,
                c.FechaInicio,
                c.FechaFin,
                c.MotivoCita,
                Paciente = c.Paciente.nombre + " " + c.Paciente.apellido,
                Doctor = c.Doctor.nombre + " " + c.Doctor.apellido
            })
            .ToListAsync();

        Console.WriteLine("Citas obtenidas: " + citas.Count);  // Verificar el número de citas obtenidas

        return Json(citas);
    }
}