using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using udembankproject.Controllers;

namespace udembankproject
{
    internal class MenuManager
    {
        enum Register_LoginOptions
        {
            Login,
            Register,
            Quit
        }
        public static void Register_LoginMenu()
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
                        Console.WriteLine("login exitoso");
                        //menu principal
                    }
                    break;

                case Register_LoginOptions.Register:
                    UsersController.AddUser();
                    break;

                case Register_LoginOptions.Quit:
                    break;
            }
        }
        enum SavingsGroupOptions
        {
            ViewMySavingsGroups,
            CreateSavingsGroups
        }
        public static void SavingsGroupMenu1()
        {
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<SavingsGroupOptions>()
                .Title("SavingsGroupMenu")
                .AddChoices(
                    SavingsGroupOptions.ViewMySavingsGroups,
                    SavingsGroupOptions.CreateSavingsGroups));
            switch (option)
            {
                case SavingsGroupOptions.ViewMySavingsGroups:
                    break;

                case SavingsGroupOptions.CreateSavingsGroups:
                    break;
            }
        }
    }
}
