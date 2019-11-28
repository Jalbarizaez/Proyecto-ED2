using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_ED2.Models
{
	public class Conversacion2
	{
		public string receptor { get; set; }
		public string emisor { get; set; }
		public List<string> mensajes { get; set; }
	}
}