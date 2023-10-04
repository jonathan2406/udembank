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
        private static IMongoDatabase database = null;

        public static IMongoDatabase Connection()
        {
            if (database == null)
            {
                MongoClient client = new MongoClient("mongodb+srv://LassoVB:lasso123@udembank.qozsdkg.mongodb.net/");
                database = client.GetDatabase("UdemBank");
            }
            return database;
        }
    }
}
