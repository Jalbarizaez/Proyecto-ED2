using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using WebServices;
namespace API_PROYECTO.Models
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IstoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.CollectionNameUsers);
        }

        public List<User> Get() =>
            _users.Find(x => true).ToList();
        public bool exist(string user)
        {

            var users = _users.Find(x => true).ToList();
            if (users.Any(x => x.usuario ==user))
            {
                return false;
            }
            else
                return true;
        }
        public User Get(string id)
        {
           
           return _users.Find<User>(x => x.id == id).FirstOrDefault();
        }

        public User Get_(string user) =>
          _users.Find<User>(x => x.usuario == user).FirstOrDefault();

        public User Create(User user)
        {
              
            _users.InsertOne(user);
            return _users.Find<User>(x => x.usuario == user.usuario).FirstOrDefault();
        }

        public void Update(string id, User In) =>
            _users.ReplaceOne(x => x.id== id, In);

        public void Remove(User In) =>
            _users.DeleteOne(x => x.nombre == In.nombre);

        public void Remove(string id) =>
            _users.DeleteOne(x => x.id == id);
    }
}
