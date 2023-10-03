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
        public static IMongoCollection<BsonDocument> GetAccountsCollectionBson()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Accounts");
            return collection;
        }

        public static IMongoCollection<Accounts> GetAccountsCollectionOriginal()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<Accounts>("Accounts");
            return collection;
        }

        public static IMongoCollection<BsonDocument> GetUsersCollectionBson()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Users");
            return collection;
        }

        public static IMongoCollection<Users> GetUsersCollectionOriginal()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<Users>("Users");
            return collection;
        }

        public static IMongoCollection<BsonDocument> GetSavingsGroupCollectionBson()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("SavingsGroup");
            return collection;
        }

        public static IMongoCollection<Savings_Group> GetSavingsGroupCollectionOriginal()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<Savings_Group>("SavingsGroup");
            return collection;
        }

        public static IMongoCollection<BsonDocument> GetLoansCollectionBson()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Loans");
            return collection;
        }

        public static IMongoCollection<Loans> GetLoansCollectionOriginal()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<Loans>("Loans");
            return collection;
        }

        public static IMongoCollection<BsonDocument> GetMovementsCollectionBson()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Movement");
            return collection;
        }

        public static IMongoCollection<Movement> GetMovementsCollectionOriginal()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<Movement>("Movement");
            return collection;
        }

        public static IMongoCollection<BsonDocument> GetTransfersCollectionBson()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Transfers");
            return collection;
        }

        public static IMongoCollection<Transfers> GetTransfersCollectionOriginal()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<Transfers>("Transfers");
            return collection;
        }

        public static IMongoCollection<BsonDocument> GetBankCollectionBson()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Bank");
            return collection;
        }

        public static IMongoCollection<Bank> GetBankCollectionOriginal()
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<Bank>("Bank");
            return collection;
        }
    }
}
