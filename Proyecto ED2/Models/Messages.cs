using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_ED2.Models
{
	public class Messages
	{
		public string id { get; set; }
		public string emisor { get; set; }
		public string receptor { get; set; }
		public string mensage { get; set; }
		public DateTime fecha { get; set; }
		public string conversacion { get; set; }
	}
}