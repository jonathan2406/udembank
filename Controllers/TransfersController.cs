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
        private readonly IMongoCollection<Movement> movementCollection;
        private readonly IMongoCollection<Transfers> transfersCollection;
        private readonly ObjectId ActiveUser;

        public TransfersController(IMongoCollection<Accounts> accountCollection, IMongoCollection<Movement> movementCollection,
            IMongoCollection<Transfers> transfersCollection, ObjectId activeUser)
        {
            this.accountCollection = accountCollection;
            this.movementCollection = movementCollection;
            this.transfersCollection = transfersCollection;
            this.ActiveUser = activeUser;
        }

        public Accounts SelectSendingAccount()
        {
            AnsiConsole.Clear();

            // Solicita al usuario el número de cuenta remitente
            var sendAccountNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter Sending Account Number: ")
                    .PromptStyle(Style.Parse("green"))
            );

            // Recupera la cuenta remitente desde la base de datos
            var sendAccount = accountCollection.Find(x => x.AccountNumber == sendAccountNumber).FirstOrDefault();

            if (sendAccount == null)
            {
                AnsiConsole.MarkupLine("[red]Error: Sending account does not exist![/]");
                return null;
            }

            return sendAccount;
        }
        public Accounts SelectReceivingAccount()
        {
            AnsiConsole.Clear();

            // Solicita al usuario el número de cuenta de recepción
            var receptionAccountNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter Receiving Account Number: ")
                    .PromptStyle(Style.Parse("green"))
            );

            // Recupera la cuenta de recepción desde la base de datos
            var receptionAccount = accountCollection.Find(x => x.AccountNumber == receptionAccountNumber).FirstOrDefault();

            if (receptionAccount == null)
            {
                AnsiConsole.MarkupLine("[red]Error: Reception account does not exist![/]");
                return null;
            }

            return receptionAccount;
        }
        public void TransferAmounts()
        {
            AnsiConsole.Clear();

            // Obtén el número de cuenta del usuario activo
            var userAccountNumber = UsersController.ObtenerNumeroDeCuentaPorUserId(ActiveUser);

            // Solicita al usuario los detalles de la transferencia
            var sendAccount = SelectSendingAccount();

            if (sendAccount == null)
            {
                return;
            }

            // Verifica que la cuenta remitente sea la cuenta del usuario activo
            if (sendAccount.AccountNumber != userAccountNumber)
            {
                AnsiConsole.MarkupLine("[red]Error: You can only transfer from your own account![/]");
                return;
            }

            var receptionAccount = SelectReceivingAccount();

            if (receptionAccount == null)
            {
                return;
            }

            var amount = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Amount (Only 9 digits): ")
                    .PromptStyle(Style.Parse("green"))
            );

            // Verifica que la cuenta remitente tenga suficiente saldo
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

            SaveTransfer(sendAccount.Id, receptionAccount.Id, amount);

            // Crea un nuevo registro de movimiento para la transferencia
            var newMovement = new Movement
            {
                DateTime = DateTime.UtcNow.ToLocalTime(),
                Amount = amount,
                Type_Id = receptionAccount.Id, // Usamos el ID de la cuenta receptora
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