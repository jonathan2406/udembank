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
    internal class BankController
    {
        public static void ViewBank()
        {
            // Recupera la lista de cuentas desde la base de datos
            var bank = Collections.GetBankCollection().Find(_ => true).ToList();
            // Crea y muestra la tabla de cuentas
            var table = new Table()
                .Title("Bank")
                .BorderColor(Color.Green)
                .AddColumn("User Account IDs", column => column.Alignment(Justify.Left))
                .AddColumn("Amount Gains", column => column.Alignment(Justify.Left))
                .AddColumn("Total Amount", column => column.Alignment(Justify.Left));

            foreach (var banKK in bank)
            {
                // Convierte la lista de UserAccountIDs a una cadena separada por comas
                string accountIDsString = string.Join(", ", banKK.UserAccountIDs.Select(id => id.ToString()));

                table.AddRow(
                    accountIDsString,
                    banKK.AmountGains.ToString(),
                    banKK.TotalAmount.ToString()
                );
            }

            AnsiConsole.Clear();
            AnsiConsole.Render(table);

            AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
            Console.ReadLine();
        }
        public static void RewardTopSavingsGroup()
        {
            // Recupera la lista de grupos de ahorro desde la base de datos
            var savingsGroups = Collections.GetSavingsGroupCollection().Find(_ => true).ToList();

            Savings_Group topSavingsGroup = null;
            int maxGains = 0;

            // Encuentra el equipo de ahorro con las ganancias más altas
            foreach (var group in savingsGroups)
            {
                if (group.Amount > maxGains)
                {
                    maxGains = group.Amount;
                    topSavingsGroup = group;
                }
            }

            if (topSavingsGroup != null)
            {
                // Calcula el 10% de las ganancias del grupo
                int rewardAmount = (int)(0.1 * topSavingsGroup.Amount);

                // Inyecta el 10% de las ganancias al saldo del grupo
                topSavingsGroup.Amount += rewardAmount;

                // Guarda la actualización en la base de datos
                Collections.GetSavingsGroupCollection().ReplaceOne(x => x.Id == topSavingsGroup.Id, topSavingsGroup);

                AnsiConsole.MarkupLine($"[green]Top savings group '{topSavingsGroup.Name}' rewarded with 10% of their current balance.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]No savings group found to reward.[/]");
            }

            AnsiConsole.MarkupLine("[yellow]Press Enter to continue...[/]");
            Console.ReadLine();
        }
    }
}
