using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_ED2.Controllers
{
    public class UsuarioController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		[HttpGet]
		public ActionResult Mensajes()
		{
			return View();
		}
		[HttpPost]
		public ActionResult Mensajes(string mensaje)
		{
			return View();
		}
	}
}