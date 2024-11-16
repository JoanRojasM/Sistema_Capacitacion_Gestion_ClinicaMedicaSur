namespace scg_clinicasur.Services
{
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;

    public class EmailService
    {
        private readonly string _smtpServer = "smtp.tu-servidor.com"; // ponga servidor SMTP
        private readonly int _smtpPort = 587; 
        private readonly string _emailFrom = "tu-correo@example.com"; // correo remitente
        private readonly string _emailPassword = "tu-contraseña"; // contraseña del correo remitente

        public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
        {
            var mensaje = new MailMessage(_emailFrom, destinatario, asunto, cuerpo)
            {
                IsBodyHtml = true
            };

            using var cliente = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_emailFrom, _emailPassword),
                EnableSsl = true
            };

            await cliente.SendMailAsync(mensaje);
        }
    }
}
