﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_ED2.Models
{
	public class Conversacion
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
		public string llave { get; set; }
		public List<Messages> recibidos { get; set; }
		public List<Messages> enviados { get; set; }
		public List<string> paths { get; set; }
	}
}