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
		public async Task<ActionResult> Mensajes(string emisor, string receptor, string mensaje, HttpPostedFileBase ArchivoEntrada)
		{
			try
			{
				TempData["usuario"] = emisor;
				TempData["friend"] = receptor;

				if (ArchivoEntrada == null)
				{
					if (emisor != "" && receptor != "" && mensaje != "") //Se va a enviar un mensaje
					{
						var cliente = new HttpClient();
						string llave = emisor + "-" + receptor;
						string urlMensaje = urlApi + "api/Messages/" + llave;
						try
						{
							var clienteGET = new HttpClient();
							string urlGET = urlApi + "api/Users";
							var jsonGET = await clienteGET.GetStringAsync(urlGET);
							var usuarios = JsonConvert.DeserializeObject<List<lista>>(jsonGET);
							List<string> Aux = new List<string>();
							foreach (lista x in usuarios)
							{
								Aux.Add(x.user);
							}

							var clienteV = new HttpClient();
							string usuarioV = urlApi + "api/Users/" + TempData["friend"].ToString();

							var jsonV = await clienteV.GetStringAsync(usuarioV);
							var Usuario = JsonConvert.DeserializeObject<User>(jsonV);

							if (TempData["usuario"].ToString() == TempData["friend"].ToString())
							{
								TempData["friend"] = null;
								TempData["msm"] = "No puede enviarse mensajes a si mismo";
								ListasVista Listas = new ListasVista();
								Listas.Usuarios = Aux;
								Listas.Mensajes = new List<string>();
								Listas.Paths = new List<string>();

								return View(Listas);
							}
							else if (Usuario == null)
							{
								TempData["friend"] = null;
								TempData["msm"] = "Usuario destinatario inexistente";
								ListasVista Listas = new ListasVista();
								Listas.Usuarios = Aux;
								Listas.Mensajes = new List<string>();
								Listas.Paths = new List<string>();
								return View(Listas);
							}
							else
							{
								var json = await cliente.GetStringAsync(urlMensaje); //En esta linea da error
								var conversacionRecibida = JsonConvert.DeserializeObject<Conversacion>(json);

								string path = Server.MapPath("~/Archivos/");
								Messages MensajePUT = new Messages();
								MensajePUT.fecha = DateTime.Now;
								MensajePUT.mensage = Q.Cifrar(mensaje, emisor, path);
								//MensajePUT.mensage = mensaje;

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
									if (Enviados == null)
									{
										Enviados = new List<Messages>();
									}
									Enviados.Add(MensajePUT);
									Listas.Paths = conversacionRecibida.paths;

									ConversacionPUT.enviados = Enviados;
									string urlPUT = urlApi + "api/Messages/" + conversacionRecibida.id;
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
									if (Recibidos == null)
									{
										Recibidos = new List<Messages>();
									}

									string path2 = Server.MapPath("~/Archivos/");
									MensajePUT.mensage = Q.Cifrar(mensaje, receptor, path);
									//MensajePUT.mensage = mensaje;
									Recibidos.Add(MensajePUT);

									ConversacionPUT.recibidos = Recibidos;
									string urlPUT = urlApi + "api/Messages/" + conversacionRecibida2.id;
									var jsonPUT = await clientePUT.PutAsync(urlPUT, new StringContent(
										new JavaScriptSerializer().Serialize(ConversacionPUT), Encoding.UTF8, "application/json"));
								}
								string path3 = Server.MapPath("~/Archivos/");
								List<string> ConversacionFiltrada = Q.ConversacionFiltrada(conversacionRecibida, emisor, path3, receptor);
								Listas.Mensajes = ConversacionFiltrada;
								Listas.Usuarios = Aux;

								return View(Listas);
							}
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
								string path = Server.MapPath("~/Archivos/");
								MensajePOST.mensage = Q.Cifrar(mensaje, emisor, path);
								//MensajePOST.mensage = mensaje;
								ConversacionPOST.llave = emisor + "-" + receptor;
								List<Messages> M = new List<Messages>();
								M.Add(MensajePOST);
								ConversacionPOST.enviados = M;

								var jsonPOST = await clientePOST.PostAsync("api/Messages", new StringContent(
								new JavaScriptSerializer().Serialize(ConversacionPOST), Encoding.UTF8, "application/json"));
								if (jsonPOST.IsSuccessStatusCode) { }
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
								string path = Server.MapPath("~/Archivos/");
								MensajePOST.mensage = Q.Cifrar(mensaje, receptor, path);
								//MensajePOST.mensage = mensaje;
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
							string pathS = Server.MapPath("~/Archivos/");
							Mensaje1.mensage = Q.Cifrar(mensaje, emisor, pathS);
							//Mensaje1.mensage = mensaje;
							ConversacionSinFiltro.llave = emisor + "-" + receptor;
							List<Messages> M2 = new List<Messages>();
							M2.Add(Mensaje1);
							ConversacionSinFiltro.enviados = M2;

							string path2 = Server.MapPath("~/Archivos/");
							List<string> ConversacionFiltrada = Q.ConversacionFiltrada(ConversacionSinFiltro, emisor, path2, receptor);
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
							string path2 = Server.MapPath("~/Archivos/");
							List<string> ConversacionFiltrada = Q.ConversacionFiltrada(conversacionRecibida, emisor, path2, receptor);
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
							return View(Listas);
						}
						catch
						{
							TempData["msm"] = "No exite conversacion con el usuario o este no esta registrado en el sistema";
							TempData["friend"] = null;
							ListasVista X = new ListasVista();
							X.Mensajes = new List<string>();
							X.Paths = new List<string>();
							X.Usuarios = new List<string>();
							return View(X);
						}
					}
					else //Todos lo parametros son nulos
					{
						TempData["friend"] = null;
						TempData["msm"] = "Debe llenar al menos un campo";
						ListasVista X = new ListasVista();
						X.Mensajes = new List<string>();
						X.Paths = new List<string>();
						X.Usuarios = new List<string>();
						return View(X);
					}
				}
				else
				{
					ListasVista Listas = new ListasVista();
					Listas.Mensajes = new List<string>();
					Listas.Usuarios = new List<string>();
					var clienteGET = new HttpClient();
					string urlGET = urlApi + "api/Users";
					List<string> Aux = new List<string>();
					try
					{
						var jsonGET = await clienteGET.GetStringAsync(urlGET);
						var usuarios = JsonConvert.DeserializeObject<List<lista>>(jsonGET);
						foreach (lista x in usuarios)
						{
							Aux.Add(x.user);
						}
						Listas.Usuarios = Aux;

						var clienteV = new HttpClient();
						string usuarioV = urlApi + "api/Users/" + TempData["friend"].ToString();

						var jsonV = await clienteV.GetStringAsync(usuarioV);
						var Usuario = JsonConvert.DeserializeObject<User>(jsonV);

						if (Usuario == null)
						{
							TempData["msm"] = "Usuario destinatario inexistente";
							Listas.Paths = new List<string>();
							return View(Listas);
						}
						else
						{
							if (TempData["usuario"].ToString() == TempData["friend"].ToString())
							{
								TempData["msm"] = "No se puede enviar un mensaje a si mismo";
								Listas.Paths = new List<string>();
								return View(Listas);
							}
							else
							{
								string[] nombreArchivo = ArchivoEntrada.FileName.Split('.');
								string path = Server.MapPath("~/ArchivosTmp/");
								path = path + ArchivoEntrada.FileName;
								ArchivoEntrada.SaveAs(path);
								Q.Comprimir(path, (path + nombreArchivo[0] + ".huff"));

								var cliente = new HttpClient();
								string llave = receptor + "-" + emisor;
								string urlMensaje = urlApi + "api/Messages/" + llave;
								var json = await cliente.GetStringAsync(urlMensaje); //En esta linea da error
								var conversacionRecibida = JsonConvert.DeserializeObject<Conversacion>(json);

								using (var clientePUT = new HttpClient())
								{
									clientePUT.BaseAddress = new Uri(urlApi);
									Conversacion ConversacionPUT = new Conversacion();
									ConversacionPUT = conversacionRecibida;
									List<string> PPP = ConversacionPUT.paths;
									if (PPP == null)
									{
										PPP = new List<string>();
									}
									PPP.Add((path + nombreArchivo[0] + ".huff"));
									ConversacionPUT.paths = PPP;

									string urlPUT = urlApi + "api/Messages/" + conversacionRecibida.id;
									var jsonPUT = await clientePUT.PutAsync(urlPUT, new StringContent(
										new JavaScriptSerializer().Serialize(ConversacionPUT), Encoding.UTF8, "application/json"));
								}

								Listas.Paths = new List<string>();
								return View(Listas);
							}
						}

					}
					catch
					{
						Listas.Paths = new List<string>();
						return View(Listas);
					}
				}
			}
			catch
			{
				@TempData["msm"] = "Ha ocurrido un error";
				ListasVista X = new ListasVista();
				X.Mensajes = new List<string>();
				X.Paths = new List<string>();
				X.Usuarios = new List<string>();
				return View(X);
			}
		}
		//ActionLink
		public ActionResult DescargarArchivo(string Path)
		{
			return File(Path, "txt", ("ArchivoChat.txt"));
		}
	}
}