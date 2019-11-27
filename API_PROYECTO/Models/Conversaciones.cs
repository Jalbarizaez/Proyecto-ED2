using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_PROYECTO.Models;
namespace API_PROYECTO.Models
{
    public class Conversaciones
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string llave { get; set; }
        public List<Messages> recibidos { get; set; }
        public List<Messages> enviados { get; set; }
    }
}
