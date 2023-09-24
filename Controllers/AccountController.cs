using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using udembankproject.Models;

namespace udembankproject.Controllers
{
    internal class AccountController
    {
        public static ObjectId? GetAccountID(string AccountNumber)
        {
            ObjectId AccountID;
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Accounts");
            var filter = Builders<BsonDocument>.Filter.Eq("Account Number", AccountNumber);
            var result = collection.Find(filter).ToList();
            if (result.Count() == 1)
            {
                return result[0]["_id"].AsObjectId;
            }
            else
            {
                Console.WriteLine("Card number not found");
                return null;
            }
        }


    }

}
