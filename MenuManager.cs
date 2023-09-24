using System;
using System.Collections.Generic;
using System.Linq;
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

        public MenuManager() 
        {
            var client = new MongoClient("mongodb+srv://LassoVB:lasso123@udembank.qozsdkg.mongodb.net/");
            var database = client.GetDatabase("UdemBank");
            var collection = database.GetCollection<Accounts>("Accounts");
            this.accountController = new AccountController(collection);
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
