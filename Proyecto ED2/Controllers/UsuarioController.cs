using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proyecto_ED2.Models;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Proyecto_ED2.Controllers
{
	//[Authorize]
    public class UsuarioController : Controller
    {
		private const string urlApi = "https://localhost:44389/";
		Servicios Q = new Servicios();

		public ActionResult Index()
        {
            return View();
        }

		[HttpGet]
		public async Task<ActionResult> Mensajes(string emisor)
		{
			if (emisor == null) { }
			else if (emisor == "") { }
			else {
				TempData["usuario"] = emisor;
			}

			ListasVista H = new ListasVista();
			string urlGET = urlApi + "api/Users";
			var clienteGET = new HttpClient();
			var jsonGET = await clienteGET.GetStringAsync(urlGET);
			var usuarios = JsonConvert.DeserializeObject<List<lista>>(jsonGET);
			List<string> Aux = new List<string>();
			foreach (lista x in usuarios)
			{
				Aux.Add(x.user);
			}
			H.Usuarios = Aux;
			return View(H);
		}
		[HttpPost]
		public async Task<ActionResult> Mensajes(string emisor, string receptor, string mensaje) //JsonResult
		{
			try
			{
				TempData["usuario"] = emisor;
				TempData["friend"] = receptor;

				if (emisor != "" && receptor != "" && mensaje != "") //Se va a enviar un mensaje
				{
					var cliente = new HttpClient();
					string llave = emisor + "-" + receptor;
					string urlMensaje = urlApi + "api/Messages/" + llave;
					try
					{
						var json = await cliente.GetStringAsync(urlMensaje); //En esta linea da error
						var conversacionRecibida = JsonConvert.DeserializeObject<Conversacion>(json);

						string urlGET = urlApi + "api/Users";
						var clienteGET = new HttpClient();
						var jsonGET = await clienteGET.GetStringAsync(urlGET);
						var usuarios = JsonConvert.DeserializeObject<List<lista>>(jsonGET);
						List<string> Aux = new List<string>();
						foreach (lista x in usuarios)
						{
							Aux.Add(x.user);
						}

						Messages MensajePUT = new Messages();
						MensajePUT.fecha = DateTime.Now;
						MensajePUT.mensage = mensaje;

						ListasVista Listas = new ListasVista();

						using (var clientePUT = new HttpClient())
						{
							clientePUT.BaseAddress = new Uri(urlApi);
							Conversacion ConversacionPUT = new Conversacion();
							ConversacionPUT.llave = llave;
							ConversacionPUT.id = conversacionRecibida.id;
							ConversacionPUT.recibidos = conversacionRecibida.recibidos;
							ConversacionPUT.paths = conversacionRecibida.paths;
							List<Messages> Enviados = conversacionRecibida.enviados;
							Enviados.Add(MensajePUT);
							Listas.Paths = conversacionRecibida.paths;

							ConversacionPUT.enviados = Enviados;
							string urlPUT = urlApi + "api/Messages/" + llave;
							var jsonPUT = await clientePUT.PutAsync(urlPUT, new StringContent(
								new JavaScriptSerializer().Serialize(ConversacionPUT), Encoding.UTF8, "application/json"));
						}

						using (var clientePUT = new HttpClient())
						{
							string llave2 = receptor + "-" + emisor;
							string urlMensaje2 = urlApi + "api/Messages/" + llave2;
							var json2 = await cliente.GetStringAsync(urlMensaje2);
							var conversacionRecibida2 = JsonConvert.DeserializeObject<Conversacion>(json2);

							clientePUT.BaseAddress = new Uri(urlApi);
                            Conversacion ConversacionPUT = new Conversacion();
                            ConversacionPUT.llave = llave2;
                            ConversacionPUT.id = conversacionRecibida2.id;
                            ConversacionPUT.enviados = conversacionRecibida2.enviados;
                            ConversacionPUT.paths = conversacionRecibida2.paths;
                            List<Messages> Recibidos = conversacionRecibida2.recibidos;
							Recibidos.Add(MensajePUT);

                            
							ConversacionPUT.recibidos = Recibidos;
							string urlPUT = urlApi + "api/Messages/" + llave;
							var jsonPUT = await clientePUT.PutAsync(urlPUT, new StringContent(
								new JavaScriptSerializer().Serialize(conversacionRecibida2), Encoding.UTF8, "application/json"));
						}

						List<Messages> Enviados2 = conversacionRecibida.enviados;
						Enviados2.Add(MensajePUT);
						conversacionRecibida.enviados = Enviados2;
						List<string> ConversacionFiltrada = Q.ConversacionFiltrada(conversacionRecibida);

						
						Listas.Mensajes = ConversacionFiltrada;
						Listas.Usuarios = Aux;

						return View(Listas);
					}
					catch
					{
						//Si falla es por que la conversacion no existe
						using (var clientePOST = new HttpClient())
						{
							clientePOST.BaseAddress = new Uri(urlApi);
							Conversacion ConversacionPOST = new Conversacion();
							Messages MensajePOST = new Messages();
							MensajePOST.fecha = DateTime.Now;
							MensajePOST.mensage = mensaje;
							ConversacionPOST.llave = emisor + "-" + receptor;
							List<Messages> M = new List<Messages>();
							M.Add(MensajePOST);
							ConversacionPOST.enviados = M;

							var jsonPOST = await clientePOST.PostAsync("api/Messages", new StringContent(
							new JavaScriptSerializer().Serialize(ConversacionPOST), Encoding.UTF8, "application/json"));
							if (jsonPOST.IsSuccessStatusCode){}
							else
							{
								TempData["msm"] = "Error emisor-receptor";
							}
						}

						using (var clientePOST = new HttpClient())
						{
							clientePOST.BaseAddress = new Uri(urlApi);
							Conversacion ConversacionPOST = new Conversacion();
							Messages MensajePOST = new Messages();
							MensajePOST.fecha = DateTime.Now;
							MensajePOST.mensage = mensaje;
							ConversacionPOST.llave = receptor + "-" + emisor;
							List<Messages> M = new List<Messages>();
							M.Add(MensajePOST);
							ConversacionPOST.recibidos = M;

							var jsonPOST = await clientePOST.PostAsync("api/Messages", new StringContent(
							new JavaScriptSerializer().Serialize(ConversacionPOST), Encoding.UTF8, "application/json"));

							if (jsonPOST.IsSuccessStatusCode) { }
							else
							{
								TempData["msm2"] = "Error receptor-emisor";
							}
						}

						string urlGET = urlApi + "api/Users";
						var clienteGET = new HttpClient();
						var jsonGET = await clienteGET.GetStringAsync(urlGET);
						var usuarios = JsonConvert.DeserializeObject<List<lista>>(jsonGET);
						List<string> Aux = new List<string>();
						foreach (lista x in usuarios)
						{
							Aux.Add(x.user);
						}

						Conversacion ConversacionSinFiltro = new Conversacion();
						Messages Mensaje1 = new Messages();
						Mensaje1.fecha = DateTime.Now;
						Mensaje1.mensage = mensaje;
						ConversacionSinFiltro.llave = emisor + "-" + receptor;
						List<Messages> M2 = new List<Messages>();
						M2.Add(Mensaje1);
						ConversacionSinFiltro.enviados = M2;

						List<string> ConversacionFiltrada = Q.ConversacionFiltrada(ConversacionSinFiltro);
						ListasVista Listas = new ListasVista();
						Listas.Mensajes = ConversacionFiltrada;
						Listas.Usuarios = Aux;
						Listas.Paths = null;

						return View(Listas);
					}
				}
				else if (receptor != "") //Se esta buscando la conversacion con un Usuario en especial
				{
					try
					{
						var cliente = new HttpClient();
						string llave = emisor + "-" + receptor;
						string urlMensaje = urlApi + "api/Messages/" + llave;
						var json = await cliente.GetStringAsync(urlMensaje); //En esta linea da error
						var conversacionRecibida = JsonConvert.DeserializeObject<Conversacion>(json);
						List<string> ConversacionFiltrada = Q.ConversacionFiltrada(conversacionRecibida);
						ListasVista Listas = new ListasVista();

						string urlGET = urlApi + "api/Users";
						var clienteGET = new HttpClient();
						var jsonGET = await clienteGET.GetStringAsync(urlGET);
						var usuarios = JsonConvert.DeserializeObject<List<lista>>(jsonGET);
						List<string> Aux = new List<string>();
						foreach (lista x in usuarios)
						{
							Aux.Add(x.user);
						}

						Listas.Mensajes = ConversacionFiltrada;
						Listas.Usuarios = Aux;
						Listas.Paths = conversacionRecibida.paths;
						return View(ConversacionFiltrada);
					}
					catch
					{
						TempData["msm"] = "Usuario inexistente en el sistema";
						TempData["friend"] = null;
						return View();
					}
				}
				else //Todos lo parametros son nulos
				{
					TempData["friend"] = null;
					TempData["msm" +
						""] = "Debe llenar al menos un campo";
					return View();
				}
			}
			catch
			{
				@TempData["msm"] = "Ha ocurrido un error";
			}
			return View();
		}

		//ActionLink
		public ActionResult DescargarArchivo(string Path)
		{
			string nombre = Q.ObtenerNombre(Path);
			return File(Path, "txt", (nombre + ".txt"));
		}
	}
}