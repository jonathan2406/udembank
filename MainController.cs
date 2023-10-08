using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static udembankproject.MenuManager;
using udembankproject.Controllers;

namespace udembankproject
{
    internal class MainController
    {
        public static void Run()
        {
            while (true)
            {
                var option = LoginRegister();
                if (option == true)
                {
                    MainMenu();
                    break;
                }
            }
        }

        public static bool LoginRegister()
        {
            while (true)
            {
                switch (MenuManager.Register_LoginMenu())
                {
                    case Register_LoginOptions.Login:
                        return UsersController.Login();

                    case Register_LoginOptions.Register:
                        Console.WriteLine("Register user");
                        UsersController.AddUser();
                        break;

                    case Register_LoginOptions.Quit:
                        Environment.Exit(0);
                        break;
                }
            }
        }
        public static void MainMenu()
        {
            while (true)
            {


                switch (MenuManager.ShowMainMenu())
                {
                    case MainMenuOptions.ViewAccounts:
                        AccountController.ViewAccounts();
                        break;
                    case MainMenuOptions.CreateAccounts:
                        AccountController.CreateAccount();
                        break;   
                    case MainMenuOptions.TransferAmounts:
                        var transfersController = new TransfersController(Collections.GetAccountsCollection(), ActiveUser);
                        transfersController.TransferAmounts();
                        break;
                    case MainMenuOptions.ViewMovements:
                        MovementController.ViewMovements();
                        break;
                    case MainMenuOptions.ViewTransfers:
                        TransfersController.ViewTransfers();
                        break;
                    case MainMenuOptions.SavingsGroups:
                        SavingsGroup();
                        break;
                    case MainMenuOptions.Exit:
                        return;

                }
            }
        }
        public static void SavingsGroup()
        {
            switch (MenuManager.SavingsGroupMenu1())
            {
                case SavingsGroupOptions.ViewMySavingsGroups:
                    SavingGroupController.ViewMySavingsGroups();
                    break;

                case SavingsGroupOptions.CreateSavingsGroups:
                    SavingGroupController.AddSavingGroup();
                    break;

                case SavingsGroupOptions.TransferToSavingGroup:
                    SavingGroupController.TransferToSavingGroup();
                    break;
            }
        }
    }
}
