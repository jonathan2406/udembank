using MongoDB.Bson;
using MongoDB.Driver;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using udembankproject.Models;

namespace udembankproject.Controllers
{
    public class TransfersController
    {

        public static void TransferAmounts()
        {
            AnsiConsole.Clear();

            // Solicita al usuario los detalles de la transferencia
            var sendAccountNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter Sender Account Number: ")
                    .PromptStyle(Style.Parse("green"))
            );

            var receptionAccountNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter Reception Account Number: ")
                    .PromptStyle(Style.Parse("green"))
            );

            var amount = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Amount (Only 9 digits): ")
                    .PromptStyle(Style.Parse("green"))
            );

            // Recupera las cuentas del remitente y del receptor
            var sendAccount = Collections.GetAccountsCollectionOriginal().Find(x => x.AccountNumber == sendAccountNumber).FirstOrDefault();
            var receptionAccount = Collections.GetAccountsCollectionOriginal().Find(x => x.AccountNumber == receptionAccountNumber).FirstOrDefault();

            // Verifica que el remitente tenga suficiente saldo
            if (sendAccount.Amount < amount)
            {
                AnsiConsole.MarkupLine("[red]Error: Insufficient balance![/]");
                return;
            }

            // Realiza la transferencia
            sendAccount.Amount -= amount;
            receptionAccount.Amount += amount;

            // Actualiza las cuentas en la base de datos
            Collections.GetAccountsCollectionOriginal().UpdateOne(x => x.Id == sendAccount.Id, Builders<Accounts>.Update.Set(a => a.Amount, sendAccount.Amount));
            Collections.GetAccountsCollectionOriginal().UpdateOne(x => x.Id == receptionAccount.Id, Builders<Accounts>.Update.Set(a => a.Amount, receptionAccount.Amount));

            var SendAccount = ObjectId.Parse(sendAccount.Id.ToString());
            var ReceptionAccount = ObjectId.Parse(receptionAccount.Id.ToString());


            SaveTransfer(SendAccount, ReceptionAccount, amount);
            var receptionAccountId = ObjectId.Parse(receptionAccount.Id.ToString());

            // Crea un nuevo registro de movimiento para la transferencia
            var newMovement = new Movement
            {
                DateTime = DateTime.UtcNow.ToLocalTime(),
                Amount = amount,
                Type_Id = receptionAccountId, // Usamos el ID de la cuenta receptora
                AccountsBalance = sendAccount.Amount
            };

            // Inserta el nuevo registro de movimiento en la base de datos
            Collections.GetMovementsCollectionOriginal().InsertOne(newMovement);


            AnsiConsole.MarkupLine("[green]Transfer successful![/]");
            AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
            Console.ReadLine();
        }

        public static void SaveTransfer(ObjectId sendId, ObjectId receptionId, int amount)
        {
            // Crea un nuevo objeto Transfers con los datos de la transferencia
            var transfer = new Transfers
            {
                Send_Id = sendId,
                Reception_Id = receptionId,
                Amount = amount
            };

            // Inserta la transferencia en la colección de MongoDB
            Collections.GetTransfersCollectionOriginal().InsertOne(transfer);
        }


        public static void ViewTransfers()
        {
            AnsiConsole.Clear();

            // Recupera la lista de transferencias desde la base de datos
            var transfers = Collections.GetTransfersCollectionOriginal().Find(_ => true).ToList();

            // Crea y muestra la tabla de transferencias
            var table = new Table()
                .Title("Transfers")
                .BorderColor(Color.Green)
                .AddColumn("Sender ID", column => column.Alignment(Justify.Left))
                .AddColumn("Receiver ID", column => column.Alignment(Justify.Left))
                .AddColumn("Amount", column => column.Alignment(Justify.Left));

            foreach (var transfer in transfers)
            {
                table.AddRow(
                    transfer.Send_Id.ToString(), // Mostrar el ID del remitente
                    transfer.Reception_Id.ToString(), // Mostrar el ID del receptor
                    transfer.Amount.ToString()
                );
            }

            AnsiConsole.Render(table);

            AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
            Console.ReadLine();
        }

    }
}