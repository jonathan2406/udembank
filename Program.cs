using udembankproject;
using udembankproject.Models;
using MongoDB.Driver;
using System;
using Spectre.Console;
using udembankproject.Controllers;

namespace udembankproject
{
    class Program
    {
        static void Main(string[] args)
        {
            var database = DBconnection.Connection();
            var peopleDB = database.GetCollection<Accounts>("Accounts");

            while (true)
            {
                bool loggedIn = MenuManager.Register_LoginMenu();

                if (loggedIn)
                {
                    var menuManager = new MenuManager(database);
                    menuManager.ShowMainMenu();
                }
                else
                {
                    Console.WriteLine("Fuera de mi edificio");
                    break;
                }
            }



        }
    }
}

