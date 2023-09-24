using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace udembankproject
{
    internal class DBconnection
    {
        public static IMongoDatabase Connection()
        {
            MongoClient client = new MongoClient("mongodb+srv://siyahkugu:2536182Aleja.@udembank.qozsdkg.mongodb.net/?retryWrites=true&w=majority");
            var database = client.GetDatabase("UdemBank");
            return database;
        }
    }
}
