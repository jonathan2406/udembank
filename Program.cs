using udembankproject.Models;
using MongoDB.Driver;
using System;
using Spectre.Console;

namespace udembankproject
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb+srv://LassoVB:lasso123@udembank.qozsdkg.mongodb.net/");
            var database = client.GetDatabase("UdemBank");
            var peopleDB = database.GetCollection<Accounts>("Accounts");
            //var account = new Accounts() { OwnerName = "John Wick", AccountNumber = "4357879231", Amount = 40000000 };
            //var account1 = new Accounts() { OwnerName = "Pepe", AccountNumber = "1540678345", Amount = 30000000 };
            //var account2 = new Accounts() { OwnerName = "Ana", AccountNumber = "9854327681", Amount = 20000000 };
            //peopleDB.InsertOne(account);
            //peopleDB.InsertOne(account1);
            //peopleDB.InsertOne(account2);

            var menuManager = new MenuManager();
            menuManager.ShowMainMenu();



        }
    }
}