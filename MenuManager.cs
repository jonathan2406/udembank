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
                        Console.WriteLine("login exitoso");
                        //menu principal
                    }
                    break;

                case Register_LoginOptions.Register:
                    UsersController.AddUser();
                    break;

                case Register_LoginOptions.Quit:
                    break;
                    
                    case "Exit":
                        return;
                }
            }
        }
    }
}
