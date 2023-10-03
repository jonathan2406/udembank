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
        public static ObjectId ActiveUser;
        public static void SetActiveUser(ObjectId user)
        {
            ActiveUser = user;
        }

        public enum Register_LoginOptions
        {
            Login,
            Register,
            Quit
        }
        public static Register_LoginOptions Register_LoginMenu()
        {


            var option = AnsiConsole.Prompt(
                new SelectionPrompt<Register_LoginOptions>()
                .Title("Welcome to UdemBank")
                .AddChoices(
                    Register_LoginOptions.Login,
                    Register_LoginOptions.Register,
                    Register_LoginOptions.Quit));

            return option;



        }
        public enum MainMenuOptions
        {
            ViewAccounts,
            CreateAccounts,
            TransferAmounts,
            ViewMovements,
            ViewTransfers,
            SavingsGroups,
            Exit
        }
        public static MainMenuOptions ShowMainMenu()
        {


            var option = AnsiConsole.Prompt(
                new SelectionPrompt<MainMenuOptions>()
                    .Title("Select an option:")
                    .AddChoices(
                    MainMenuOptions.ViewAccounts,
                    MainMenuOptions.CreateAccounts,
                    MainMenuOptions.TransferAmounts,
                    MainMenuOptions.ViewMovements,
                    MainMenuOptions.ViewTransfers,
                    MainMenuOptions.SavingsGroups,
                    MainMenuOptions.Exit));
            Console.WriteLine(option);
            return option;

        }
        public enum SavingsGroupOptions
        {
            ViewMySavingsGroups,
            CreateSavingsGroups,
            TransferToSavingGroup
        }
        public static SavingsGroupOptions SavingsGroupMenu1()
        {
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<SavingsGroupOptions>()
                .Title("SavingsGroupMenu")
                .AddChoices(
                    SavingsGroupOptions.ViewMySavingsGroups,
                    SavingsGroupOptions.CreateSavingsGroups,
                    SavingsGroupOptions.TransferToSavingGroup
                    ));
            return option;
        }
    }
}
