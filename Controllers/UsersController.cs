﻿using MongoDB.Bson;
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
            var name = AnsiConsole.Prompt(new TextPrompt<string>("User Name:")
                .PromptStyle(Style.Parse("green")));
            var password = AnsiConsole.Prompt(new TextPrompt<string>("Password: ")
                .PromptStyle(Style.Parse("green")));
            var cardNumber = AnsiConsole.Prompt(new TextPrompt<string>("Card Number: ")
                .PromptStyle(Style.Parse("green")));
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
                AccountID = AccoundID,
                Cheats = false
            };

            collection.InsertOne(insertion);
            Console.WriteLine("Successful registration");
            Thread.Sleep(2000);
        }

        public static bool Login()
        {
            var name = AnsiConsole.Prompt(new TextPrompt<string>("Username: ")
                .PromptStyle(Style.Parse("green"))
                );

            var password = AnsiConsole.Prompt(new TextPrompt<string>("Password: ")
                .PromptStyle(Style.Parse("green")));

            if (VerifyLogin(name, password) == true)
            {
                Console.WriteLine($"El número de cuenta del usuario logeado es: {ObtenerNumeroDeCuentaPorUserId(MenuManager.ActiveUser)}");
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
            MenuManager.SetActiveUser(ObtenerIdPorUsername(user));
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

        public static ObjectId ObtenerIdPorUsername(string username)
        {
            IMongoDatabase database = DBconnection.Connection(); // Conecta a tu base de datos
            var collection = database.GetCollection<BsonDocument>("Users"); // Obtiene la colección

            var filter = Builders<BsonDocument>.Filter.Eq("User", username); // Crea un filtro
            var usuario = collection.Find(filter).FirstOrDefault(); // Realiza la consulta


            return usuario["_id"].AsObjectId; // Retorna el _id como ObjectId
            

        }
        public static string ObtenerNumeroDeCuentaPorUserId(ObjectId userId)
        {
            IMongoDatabase database = DBconnection.Connection();
            var usersCollection = database.GetCollection<BsonDocument>("Users");
            var accountsCollection = database.GetCollection<BsonDocument>("Accounts");

            var usuario = usersCollection.Find(new BsonDocument("_id", userId)).FirstOrDefault();

            var accountId = usuario.GetValue("AccountID").AsObjectId;
            var cuenta = accountsCollection.Find(new BsonDocument("_id", accountId)).FirstOrDefault();

            return cuenta.GetValue("Account Number").AsString;
        }

    }
}
