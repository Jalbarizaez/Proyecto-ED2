using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_PROYECTO.Models
{
    public class UserstoreDatabaseSettings : IstoreDatabaseSettings
    {
        public string CollectionNameUsers { get; set; }
        public string CollectionNameMessages { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public interface IstoreDatabaseSettings
    {
        string CollectionNameMessages { get; set; }
        string CollectionNameUsers { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
