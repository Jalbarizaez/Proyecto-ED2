using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using Prueba;
using Proyecto_ED2.Models;

namespace Proyecto_ED2.Models
{
	public class Servicios
	{
		public string Cifrar(string contraseña, string usuario, string pathSDES)
		{
			string Entrada = pathSDES + "Entrada.txt";
			string Salida = pathSDES + "Salida.txt";
			string SDEStxt = pathSDES + "S-DES.txt";

			using (StreamWriter writer = File.CreateText(Entrada)) { writer.Write(contraseña); }
			using (StreamWriter writer = File.CreateText(Salida)) { writer.Write(""); }

			ServiciosDLL H = new ServiciosDLL();
			ServiciosDLL.SDES Q = new ServiciosDLL.SDES();
			ServiciosDLL.ZigZag Z = new ServiciosDLL.ZigZag();

			int hash = H.Hash(usuario);
			var bytes = Convert.ToString(hash, 2);
			char[] bytesC = bytes.ToCharArray();
			if (bytesC.Length < 10)
			{
				List<char> ListaBytes = new List<char>();
				ListaBytes.AddRange(bytesC);
				while (ListaBytes.Count() < 10)
				{
					ListaBytes.Add('0');
				}
				bytesC = ListaBytes.ToArray();
			}
			if (bytesC.Length > 10)
			{
				Array.Resize(ref bytesC, 10);
			}
			usuario = "";
			foreach (var item in bytesC)
			{
				usuario += item.ToString();
			}
			usuario = "1110011100";
			//Q.Cifrado(usuario, SDEStxt, Salida, Entrada);
			Z.Codificar(Entrada, Salida, 3);
			contraseña = File.ReadAllText(Salida, Encoding.UTF8);

			File.Delete(Entrada);
			File.Delete(Salida);

			return contraseña;
		}

		public string Decifrar(string contraseña, string usuario, string pathSDES)
		{
			string Entrada = pathSDES + "Entrada.txt";
			string Salida = pathSDES + "Salida.txt";
			string SDEStxt = pathSDES + "S-DES.txt";

			using (StreamWriter writer = File.CreateText(Entrada)) { writer.Write(contraseña); }
			using (StreamWriter writer = File.CreateText(Salida)) { writer.Write(""); }

			ServiciosDLL H = new ServiciosDLL();
			ServiciosDLL.SDES Q = new ServiciosDLL.SDES();
			ServiciosDLL.ZigZag Z = new ServiciosDLL.ZigZag();

			int hash = H.Hash(usuario);
			var bytes = Convert.ToString(hash, 2);
			char[] bytesC = bytes.ToCharArray();
			if (bytesC.Length < 10)
			{
				List<char> ListaBytes = new List<char>();
				ListaBytes.AddRange(bytesC);
				while (ListaBytes.Count() < 10)
				{
					ListaBytes.Add('0');
				}
				bytesC = ListaBytes.ToArray();
			}
			if (bytesC.Length > 10)
			{
				Array.Resize(ref bytesC, 10);
			}
			usuario = "";
			foreach (var item in bytesC)
			{
				usuario += item.ToString();
			}
			usuario = "1110011100";
			//Q.Descifrado(usuario, SDEStxt, Salida, Entrada);
			Z.Decodificar(Entrada, Salida, 3);
			contraseña = File.ReadAllText(Salida, Encoding.UTF8);

			File.Delete(Entrada);
			File.Delete(Salida);

			return contraseña;
		}

		public List<string> ConversacionFiltrada(Conversacion ConversacionSinFiltro, string usuario, string pathSDES, string receptor)
		{
			List<string> Resultado = new List<string>();
			if (ConversacionSinFiltro.recibidos == null && ConversacionSinFiltro.enviados != null)
			{
				foreach (Messages mensaje in ConversacionSinFiltro.enviados)
				{
					mensaje.mensage = Decifrar(mensaje.mensage, usuario, pathSDES);
					Resultado.Add("emisor█" + mensaje.mensage);
				}
			}
			else if (ConversacionSinFiltro.recibidos != null && ConversacionSinFiltro.enviados == null)
			{
				foreach (Messages mensaje in ConversacionSinFiltro.recibidos)
				{
					mensaje.mensage = Decifrar(mensaje.mensage, receptor, pathSDES);
					Resultado.Add("receptor█" + mensaje.mensage);
				}
			}
			else
			{
				List<Messages> Recibidos = ConversacionSinFiltro.recibidos;
				List<Messages> Enviados = ConversacionSinFiltro.enviados;
				List<Messages> Aux = new List<Messages>();
				foreach (Messages mensaje in Recibidos)
				{
					mensaje.mensage = Decifrar(mensaje.mensage, receptor, pathSDES);
					mensaje.mensage = "receptor█" + mensaje.mensage;
					Aux.Add(mensaje);
				}
				foreach (Messages mensaje in Enviados)
				{
					mensaje.mensage = Decifrar(mensaje.mensage, usuario, pathSDES);
					mensaje.mensage = "emisor█" + mensaje.mensage;
					Aux.Add(mensaje);
				}
				Aux = Aux.OrderBy(o => o.fecha).ToList();
				foreach (Messages mensaje in Aux)
				{
					Resultado.Add(mensaje.mensage);
				}
			}
			return Resultado;
		}

		public string ObtenerNombre(string url)
		{
			string[] urls = url.Split('\\');

			return "";
		}
	}
}