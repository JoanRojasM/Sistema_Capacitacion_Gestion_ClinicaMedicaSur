using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Models;
using scg_clinicasur.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la cadena de conexión con el nombre "DefaultConnection"
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Soporte para sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Configura el tiempo de inactividad de la sesión (30 minutos)
    options.Cookie.HttpOnly = true; // Asegura que la cookie de la sesión no pueda ser accedida desde JavaScript (más seguro)
    options.Cookie.IsEssential = true; // Hacer que la cookie de sesión sea esencial para cumplir con GDPR si es necesario
});

// Registro IHttpContextAccessor para acceder a la sesión en controladores/vistas
builder.Services.AddHttpContextAccessor();

// Añadir servicios de autenticación, si estás utilizando autenticación basada en cookies
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // Ruta de login si el usuario no está autenticado
        options.LogoutPath = "/Account/Logout"; // Ruta de logout
    });

// Add services to the container (controladores y vistas)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Configuración HSTS (solo en producción)
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Habilita autenticación
app.UseAuthorization();  // Habilita autorización

app.UseSession(); // Habilitar sesiones

// Configurar las rutas por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();