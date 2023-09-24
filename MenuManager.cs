using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Spectre.Console;
using udembankproject.Models;
using udembankproject.Controllers;

namespace udembankproject
{
    public class MenuManager
    {
        private readonly AccountController accountController;

        public MenuManager(IMongoDatabase database)
        {
            var collection = database.GetCollection<Accounts>("Accounts");
            this.accountController = new AccountController(collection);
        }
        enum Register_LoginOptions
        {
            Login,
            Register,
            Quit
        }
        public static bool Register_LoginMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();

                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<Register_LoginOptions>()
                    .Title("Welcome to UdemBank")
                    .AddChoices(
                        Register_LoginOptions.Login,
                        Register_LoginOptions.Register,
                        Register_LoginOptions.Quit));

                switch (option)
                {
                    case Register_LoginOptions.Login:
                        if (UsersController.Login() == true)
                        {
                            Console.WriteLine("Login exitoso");
                            return true; // Devuelve true en caso de inicio de sesión exitoso
                        }
                        break;

                    case Register_LoginOptions.Register:
                        UsersController.AddUser();
                        break;

                    case Register_LoginOptions.Quit:
                        return false; // Devuelve false si el usuario elige salir;
                }

            }

        }

        public void ShowMainMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();

                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .PageSize(5)
                        .AddChoices("View Accounts", "Create Accounts", "View Banks", "View Loans", "View Movements", "View Savings Groups", "View Transfers", "View Users", "Exit")
                );

                switch (option)
                {
                    case "View Accounts":
                        accountController.ViewAccounts();
                        break;

                    case "Create Accounts":
                        accountController.CreateAccount();
                        break;

                    case "View Banks":

                        break;

                    case "View Loans":

                        break;

                    case "View Movements":

                        break;

                    case "View Savings Groups":

                        break;

                    case "View Transfers":

                        break;

                    case "View Users":

                        break;

                    case "Exit":
                        return;
                }
            }

        }
    }
}