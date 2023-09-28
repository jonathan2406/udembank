using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using udembankproject.Models;

namespace udembankproject
{
    internal class Collections
    {
        public static IMongoCollection<BsonDocument> GetAccountsCollection()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Accounts");
            return collection;
        }

        public static IMongoCollection<BsonDocument> GetUsersCollection()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Users");
            return collection;
        }

        public static IMongoCollection<BsonDocument> GetSavingsGroupCollection()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("SavingsGroup");
            return collection;
        }

        public static IMongoCollection<BsonDocument> GetLoansCollection()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Loans");
            return collection;
        }

        public static IMongoCollection<BsonDocument> GetMovementsCollection()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Movement");
            return collection;
        }

        public static IMongoCollection<BsonDocument> GetTransfersCollection()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Transfers");
            return collection;
        }

        public static IMongoCollection<BsonDocument> GetBankCollection()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Bank");
            return collection;
        }
    }
}
