using MongoDB.Bson;
using MongoDB.Driver;
using Spectre.Console;
using udembankproject;
using udembankproject.Models;
using udembankproject.Controllers;

namespace udembankproject.Controllers
{
    public class TransfersController
    {
        private readonly IMongoCollection<Accounts> accountCollection;
        private readonly ObjectId ActiveUser;

        public TransfersController(IMongoCollection<Accounts> accountCollection, ObjectId activeUser)
        {
            this.accountCollection = accountCollection;
            this.ActiveUser = activeUser;
        }

        public void TransferAmounts()
        {
            AnsiConsole.Clear();

            var userAccountNumber = UsersController.ObtenerNumeroDeCuentaPorUserId(ActiveUser);
            var sendAccount = SelectAccount("Sending", userAccountNumber);

            if (sendAccount == null)
            {
                return;
            }

            var receptionAccount = SelectAccount("Receiving");

            if (receptionAccount == null)
            {
                return;
            }

            var amount = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Amount (Only 9 digits): ")
                    .PromptStyle(Style.Parse("green"))
            );

            if (sendAccount.Amount < amount)
            {
                AnsiConsole.MarkupLine("[red]Error: Insufficient balance![/]");
                return;
            }

            PerformTransfer(sendAccount, receptionAccount, amount);

            AnsiConsole.MarkupLine("[green]Transfer successful![/]");
            AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
            Console.ReadLine();
        }

        private Accounts SelectAccount(string accountType, string userAccountNumber = null)
        {
            AnsiConsole.Clear();

            var accountNumber = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter {accountType} Account Number: ")
                    .PromptStyle(Style.Parse("green"))
            );

            var account = accountCollection.Find(x => x.AccountNumber == accountNumber).FirstOrDefault();

            if (account == null)
            {
                AnsiConsole.MarkupLine($"[red]Error: {accountType} account does not exist![/]");
                return null;
            }

            if (userAccountNumber != null && account.AccountNumber != userAccountNumber)
            {
                AnsiConsole.MarkupLine("[red]Error: You can only transfer from your own account![/]");
                return null;
            }

            return account;
        }

        private void PerformTransfer(Accounts senderAccount, Accounts receiverAccount, int amount)
        {
            senderAccount.Amount -= amount;
            receiverAccount.Amount += amount;

            Collections.GetAccountsCollection().UpdateOne(x => x.Id == senderAccount.Id, Builders<Accounts>.Update.Set(a => a.Amount, senderAccount.Amount));
            Collections.GetAccountsCollection().UpdateOne(x => x.Id == receiverAccount.Id, Builders<Accounts>.Update.Set(a => a.Amount, receiverAccount.Amount));

            TransfersController.SaveTransfer(senderAccount.Id, receiverAccount.Id, amount);

            var newMovement = new Movement
            {
                DateTime = DateTime.UtcNow.ToLocalTime(),
                Amount = amount,
                Type_Id = receiverAccount.Id,
                AccountsBalance = senderAccount.Amount
            };

            Collections.GetMovementsCollection().InsertOne(newMovement);
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
            Collections.GetTransfersCollection().InsertOne(transfer);
        }

        public static void ViewTransfers()
        {
            AnsiConsole.Clear();

            // Recupera la lista de transferencias desde la base de datos
            var transfers = Collections.GetTransfersCollection().Find(_ => true).ToList();

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