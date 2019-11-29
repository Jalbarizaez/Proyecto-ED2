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

        public Conversaciones Get_(string user) =>
          _message.Find<Conversaciones>(x => x.llave == user).FirstOrDefault();



        public void Update(string id, Conversaciones In) =>
            _message.ReplaceOne(x => x.llave == id, In);

        public void Remove(Conversaciones In) =>
            _message.DeleteOne(x => x.llave == In.llave);

        public void Remove(string id) =>
            _message.DeleteOne(x => x.llave == id);
    }
}

