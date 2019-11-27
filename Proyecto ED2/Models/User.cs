using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_ED2.Models
{
	public class User
	{
		public string id { get; set; }
		public string usuario { get; set; }
		public string contraseña { get; set; }
		public string nombre { get; set; }
		public string apellido { get; set; }
		public string correo { get; set; }
		public int edad { get; set; }

		public User(string usuario, string contraseña, string nombre, string apellido, string correo, int edad)
		{
			this.usuario = usuario;
			this.contraseña = contraseña;
			this.nombre = nombre;
			this.apellido = apellido;
			this.correo = correo;
			this.edad = edad;
		}
	}
}