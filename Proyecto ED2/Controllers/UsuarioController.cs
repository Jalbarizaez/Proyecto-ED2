using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proyecto_ED2.Models;

namespace Proyecto_ED2.Controllers
{
	//[Authorize]
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
		public ActionResult Mensajes(string emisor, string receptor, string mensaje) // string emisor, string receptor, string mensaje
		{
			try
			{
				if (emisor != "" && receptor != "")
				{
					//PUT para iniciar las conversaciones y que devuelva un Json tipo Conversacion
				}
				else if (receptor != "")
				{
					//GET para obtener solo las conversaciones con ese sujeto
				}
				else
				{
					//La cago en algo y no funciono
				}
			}
			catch
			{
				@TempData["msm"] = "Ha ocurrido un error";
			}

			return View();
		}
	}
}