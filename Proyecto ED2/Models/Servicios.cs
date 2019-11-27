using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using Prueba;

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

			Q.Cifrado(usuario, SDEStxt, Salida, Entrada);
			contraseña = File.ReadAllText(Salida, Encoding.UTF8);

			File.Delete(Entrada);
			File.Delete(Salida);

			return contraseña;
		}
	}
}