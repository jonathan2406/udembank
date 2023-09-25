using MongoDB.Bson;
using MongoDB.Driver;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using udembankproject.Models;


namespace udembankproject.Controllers
{
    internal class UsersController
    {
        public static void AddUser()
        {
            var name = AnsiConsole.Ask<string>("User name:");
            var password = AnsiConsole.Ask<string>("password: ");
            var cardNumber = AnsiConsole.Ask<string>("Card Number: ");
            ObjectId? AccoundID = AccountController.GetAccountID(cardNumber);


            if (AccoundID == null)
            {
                return;
            }

            if (VerifyUserAccountExistence(AccoundID) == false)
            {
                return;
            }

            if (VerifyUser(name) == true)
            {
                return;
            }

            var database = DBconnection.Connection();
            var collection = database.GetCollection<Users>("Users");

            var insertion = new Users
            {
                User = name,
                Password = password,
                AccountID = AccoundID
            };

            collection.InsertOne(insertion);
            Console.WriteLine("Successful registration");
            Thread.Sleep(2000);
        }

        public static bool Login()
        {
            var name = AnsiConsole.Ask<string>("User name:");
            var password = AnsiConsole.Ask<string>("password: ");

            if (VerifyLogin(name, password) == true)
            {
                MenuManager.SetActiveUser(name);
                return true;
            }
            return false;
        }

        public static bool VerifyLogin(string user, string password)
        {
            if (VerifyUser(user) == false)
            {
                Console.WriteLine("User wrong");
                return false;
            }
            if (VerifyPassword(password) == false)
            {
                Console.WriteLine("Password wrong");
                return false;
            }
            return true;
        }

        public static bool VerifyUser(string user)
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("User", user);
            bool Verify = collection.Find(filter).Any();

            if (Verify == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool VerifyPassword(string password)
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("Password", password);
            bool Verify = collection.Find(filter).Any();

            if (Verify == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool VerifyUserAccountExistence(ObjectId? AccountID)
        {
            IMongoDatabase database = DBconnection.Connection();
            var collection = database.GetCollection<BsonDocument>("Users");
            var filter2 = Builders<BsonDocument>.Filter.Eq("AccountID", AccountID);
            bool AccountUserexist = collection.Find(filter2).Any();

            if (AccountUserexist == true)
            {
                Console.WriteLine("The card number already has a user assigned");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
