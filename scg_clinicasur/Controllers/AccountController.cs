using Microsoft.AspNetCore.Mvc;

namespace scg_clinicasur.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (username == "Samuel" && password == "123")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Usuario o Contraseña Incorrectos.";
                return View();
            }
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            if (email == "samuel@outlook.com")
            {
                ViewBag.SuccessMessage = "Enviado con éxito.";
            }
            else
            {
                ViewBag.ErrorMessage = "Correo Electrónico Incorrecto.";
            }
            return View();
        }
    }
}
