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
        public static ObjectId ActiveUser { get; set; }
        public static void SetActiveUser(string username)
        {

            ActiveUser = UsersController.ObtenerIdPorUsername(username);
        }
        private readonly AccountController accountController;
        private readonly TransfersController transfersController;
        private readonly MovementController movementController;

        public MenuManager(IMongoDatabase database)
        {
            var collection = database.GetCollection<Accounts>("Accounts");
            this.accountController = new AccountController(collection, ActiveUser);
            this.transfersController = new TransfersController(
                collection,
                database.GetCollection<Movement>("Movement"),
                database.GetCollection<Transfers>("Transfers"),
                ActiveUser // Utiliza la cuenta asignada al usuario activo
            );
            this.movementController = new MovementController(database.GetCollection<Movement>("Movement"));
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
                            Console.WriteLine("Successful login");
                            return true; // Devuelve true en caso de inicio de sesión exitoso
                        }
                        break;

                    case Register_LoginOptions.Register:
                        Console.WriteLine("Register user");
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

                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an option:")
                        .PageSize(5)
                        .AddChoices("View Accounts", "Create Accounts", "Transfer Amounts", "View Movements", "View Transfers", "Savings Groups","Exit")
                );

                switch (option)
                {
                    case "View Accounts":
                        accountController.ViewAccounts();
                        break;
                    case "Create Accounts":
                        accountController.CreateAccount();
                        break;
                    case "Transfer Amounts":
                        transfersController.TransferAmounts();
                        break;
                    case "View Movements":
                        movementController.ViewMovements();
                        break;
                    case "View Transfers":
                        transfersController.ViewTransfers();
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
                    if (SavingGroupController.VerificarAparicionesMenosDeTresVeces(ActiveUser) == true)
                    {
                        SavingGroupController.AddSavingGroup();
                        break;
                    }
                    Console.WriteLine("The user is already in the maximum number of groups allowed");
                    break;

                case SavingsGroupOptions.TransferToSavingGroup:
                    break;
            }
        }
    }
}