using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace API_PROYECTO.Models
{
    public class Messages
    { 
        public string mensage { get; set; }
        public DateTime fecha { get; set; }
        
	}
}
