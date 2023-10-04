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
            var movements = Collections.GetMovementsCollection().Find(_ => true).ToList();

            var table = new Table()
                .Title("Movements")
                .BorderColor(Color.Green)
                .AddColumn("Date", column => column.Alignment(Justify.Left))
                .AddColumn("Amount", column => column.Alignment(Justify.Left))
                .AddColumn("Type ID", column => column.Alignment(Justify.Left))
                .AddColumn("Account Balance", column => column.Alignment(Justify.Left));

            foreach (var movement in movements)
            {
                DateTime localTime = movement.DateTime.ToLocalTime();
                table.AddRow(
                    movement.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    movement.Amount.ToString(),
                    movement.Type_Id.ToString(), // Ahora Type_Id es ObjectId
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