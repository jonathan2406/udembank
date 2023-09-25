using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using MongoDB.Driver;
using MongoDB.Bson;
using Spectre.Console;
using udembankproject.Models;
using udembankproject.Controllers;

namespace udembankproject
{
    public class MenuManager
    {
        public static string ActiveUser;
        public static void SetActiveUser(string username)
        {
            ActiveUser = username;
        }
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
                        Console.WriteLine("add userrrrrrr");
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
                        .AddChoices("View Accounts", "Create Accounts","Savings Groups", "Exit")
                );

                switch (option)
                {
                    case "View Accounts":
                        accountController.ViewAccounts();
                        break;

                    case "Create Accounts":
                        accountController.CreateAccount();
                        break;

                    case "Savings Groups":
                        SavingsGroupMenu1();
                        break;


                    case "Exit":
                        return;
                }
            }

        }
        enum SavingsGroupOptions
        {
            ViewMySavingsGroups,
            CreateSavingsGroups,
            TransferToSavingGroup
        }
        public static void SavingsGroupMenu1()
        {
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<SavingsGroupOptions>()
                .Title("SavingsGroupMenu")
                .AddChoices(
                    SavingsGroupOptions.ViewMySavingsGroups,
                    SavingsGroupOptions.CreateSavingsGroups,
                    SavingsGroupOptions.TransferToSavingGroup
                    ));
            switch (option)
            {
                case SavingsGroupOptions.ViewMySavingsGroups:
                    break;

                case SavingsGroupOptions.CreateSavingsGroups:
                    SavingGroupController.AddSavingGroup();
                    break;

                case SavingsGroupOptions.TransferToSavingGroup:
                    break;
            }
        }
    }
}