using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_PROYECTO.Models
{
    public class MessageService
    {
        private readonly IMongoCollection<Conversaciones> _message;

        public MessageService(IstoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _message = database.GetCollection<Conversaciones>(settings.CollectionNameMessages);
        }
        public Conversaciones Create(Conversaciones mesage)
        {

            _message.InsertOne(mesage);


            return _message.Find<Conversaciones>(x => x.id == mesage.id).FirstOrDefault(); ;
        }
        public List<Conversaciones> Get() =>
           _message.Find(x => true).ToList();
        public bool exist(string key)
        {

            var message = _message.Find(x => true).ToList();
            if (message.Any(x => x.llave == key))
            {
                return false;
            }
            else
                return true;
        }
        public Conversaciones Get(string id)
        {

            return _message.Find<Conversaciones>(x => x.id == id).FirstOrDefault();
        }

        public Conversaciones Get_(string llave) =>
          _message.Find<Conversaciones>(x => x.llave == llave).FirstOrDefault();



        public void Update(string llave, Conversaciones In) =>
            _message.ReplaceOne(x => x.llave == llave, In);

        public void Remove(Conversaciones In) =>
            _message.DeleteOne(x => x.llave == In.llave);

        public void Remove(string llave) =>
            _message.DeleteOne(x => x.llave == llave);
    }
}

