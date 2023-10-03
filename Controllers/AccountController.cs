﻿using MongoDB.Bson;
using MongoDB.Driver;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using udembankproject.Models;


namespace udembankproject.Controllers
{
    internal class AccountController
    {

        private readonly IMongoCollection<Accounts> accountCollection;
        private readonly ObjectId ActiveUser;

        public AccountController(IMongoCollection<Accounts> accountCollection, ObjectId activeUser)
        {
            this.accountCollection = accountCollection;
            this.ActiveUser = activeUser;
        }
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

        public void ViewAccounts()
        {
            // Recupera la lista de cuentas desde la base de datos
            var accounts = accountCollection.Find(_ => true).ToList();

            // Crea y muestra la tabla de cuentas
            var table = new Table()
                .Title("Accounts")
                .BorderColor(Color.Green)
                .AddColumn("Account Number", column => column.Alignment(Justify.Left))
                .AddColumn("Owner Name", column => column.Alignment(Justify.Left))
                .AddColumn("Amount", column => column.Alignment(Justify.Left))
                .AddColumn("ID", column => column.Alignment(Justify.Left));

            foreach (var account in accounts)
            {
                table.AddRow(
                    account.AccountNumber,
                    account.OwnerName,
                    account.Amount.ToString(),
                    account.Id.ToString()
                );
            }

            AnsiConsole.Clear();
            AnsiConsole.Render(table);

            AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
            Console.ReadLine();
        }

        public void CreateAccount()
        {
            AnsiConsole.Clear();

            // Solicita al usuario los detalles de la cuenta
            var ownerName = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter Owner Name: ")
                    .PromptStyle(Style.Parse("green"))
            );

            var accountNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter Account Number: ")
                    .PromptStyle(Style.Parse("green"))
            );

            var amount = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Amount (Only 9 digits): ")
                    .PromptStyle(Style.Parse("green"))
            );

            // Crea una nueva cuenta
            var newAccount = new Accounts
            {
                OwnerName = ownerName,
                AccountNumber = accountNumber,
                Amount = amount
            };

            // Inserta la nueva cuenta en la base de datos
            accountCollection.InsertOne(newAccount);

            AnsiConsole.MarkupLine("[green]Account created successfully![/]");
            AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
            Console.ReadLine();
        }

    }
}



