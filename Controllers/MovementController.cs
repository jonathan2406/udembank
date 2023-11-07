using MongoDB.Bson;
using MongoDB.Driver;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using udembankproject.Models;

namespace udembankproject.Controllers
{
    public class MovementController
    {
        public static void CreateMovement(DateTime dateTime, int amount, ObjectId type_id, int accountsBalance)
        {
            var newMovement = new Movement
            {
                DateTime = dateTime,
                Amount = amount,
                Type_Id = type_id,
                AccountsBalance = accountsBalance
            };

            Collections.GetMovementsCollection().InsertOne(newMovement);
        }

        public static void ViewMovements()
        {
            // Obtén el número de cuenta del usuario logeado
            string userAccountNumber = UsersController.ObtenerNumeroDeCuentaPorUserId(MenuManager.ActiveUser);

            // Obtiene el ID de la cuenta logeada en función del número de cuenta
            ObjectId? userAccountId = AccountController.GetAccountID(userAccountNumber);

            if (userAccountId == null)
            {
                Console.WriteLine("User account not found");
                return;
            }

            // Filtra los movimientos que tienen el mismo SenderId que el ID de la cuenta logeada
            var filter = Builders<Movement>.Filter.Eq("SenderId", userAccountId);

            var movements = Collections.GetMovementsCollection().Find(filter).ToList();

            // Crea y muestra la tabla de movimientos
            var table = new Table()
                .Title("My Movements")
                .BorderColor(Color.Green)
                .AddColumn("Date", column => column.Alignment(Justify.Left))
                .AddColumn("Amount", column => column.Alignment(Justify.Left))
                .AddColumn("Sender ID", column => column.Alignment(Justify.Left))
                .AddColumn("Type ID", column => column.Alignment(Justify.Left))
                .AddColumn("Account Balance", column => column.Alignment(Justify.Left));

            foreach (var movement in movements)
            {
                DateTime localTime = movement.DateTime.ToLocalTime();
                table.AddRow(
                    movement.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    movement.Amount.ToString(),
                    movement.SenderId.ToString(),
                    movement.Type_Id.ToString(),
                    movement.AccountsBalance.ToString()
                );
            }

            AnsiConsole.Clear();
            AnsiConsole.Render(table);

            AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
            Console.ReadLine();
        }
    }
}