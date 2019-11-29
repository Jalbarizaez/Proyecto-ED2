using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Proyecto_ED2.Models;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.IO;

namespace Proyecto_ED2.Controllers
{
	[AllowAnonymous]
    public class HomeController : Controller
    {
		private const string urlApi = "https://localhost:44389/";
		Servicios Q = new Servicios();
		public ActionResult Index()
        {
			string pathCarpeta2 = Path.Combine(Server.MapPath("~/"), "ArchivosTmp");
			Directory.CreateDirectory(pathCarpeta2);

			return RedirectToAction("LogIn");
        }
		
		#region 
		public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
		#endregion

		[HttpGet]
		public ActionResult LogIn()
		{
			return View();
		}
		[HttpPost]
		public async Task<ActionResult> LogIn(string user, string password)
		{
			try
			{
				string path = Server.MapPath("~/Archivos/");
				var cliente = new HttpClient();
				string usuario = urlApi + "api/Users/" + user;
				var json = await cliente.GetStringAsync(usuario);
				var Usuario = JsonConvert.DeserializeObject<User>(json);

                if (Usuario.contraseña == Q.Cifrar(password, user, path))
				{
                    var client = new HttpClient();
					client.BaseAddress = new Uri(urlApi);
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					var jsonWebToken = await client.PostAsync("api/JWT", new StringContent(
							new JavaScriptSerializer().Serialize(Usuario), Encoding.UTF8, "application/json"));

					if (jsonWebToken.IsSuccessStatusCode)
					{
						TempData["msm"] = "Bienvenido " + Usuario.nombre + " " + Usuario.apellido;
						TempData["usuario"] = user;
						return RedirectToAction("Mensajes", "Usuario", user);
					}
					else
					{
						TempData["msm"] = "Ha sucedido un error";
						return View();
					}
				}
				else
				{
					TempData["msm"] = "Usuario y/o Contraseña incorecta";
					return View();
				}
			}
			catch
			{
				TempData["msm"] = "Ha sucedido un error";
				return View();
			}
		}

		[HttpGet]
		public ActionResult SignIn()
		{
			return View();
		}
		[HttpPost]
		public async Task<ActionResult> SignIn(string usuario, string contraseña, string nombre, string apellido, string correo, int edad)
		{
			try
			{
				if (usuario != "" && contraseña != "" && nombre != "" && apellido != "" && correo != "")
				{
					string path = Server.MapPath("~/Archivos/");
					contraseña = Q.Cifrar(contraseña, usuario, path);
					User H = new User(usuario, contraseña, nombre, apellido, correo, edad);

					using (var cliente = new HttpClient())
					{
						cliente.BaseAddress = new Uri(urlApi);
						cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
						var json = await cliente.PostAsync("api/Users", new StringContent(
							new JavaScriptSerializer().Serialize(H), Encoding.UTF8, "application/json"));

						if (json.IsSuccessStatusCode)
						{
							TempData["msm"] = "Usuario " + usuario + " creado exitosametnte";
							return View();
						}
						else
						{
							TempData["msm"] = "Ha sucedido un error";
							return View();
						}
					}
				}
				else
				{
					TempData["msm"] = "Debe llenar todos los campos";
					return View();
				}
			}
			catch
			{
				TempData["msm"] = "Ha sucedido un error";
				return View();
			}
		}
	}
}