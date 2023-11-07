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

            if (userAccountNumber == null)
            {
                userAccountNumber = AnsiConsole.Prompt(
                    new TextPrompt<string>($"Enter {accountType} Account Number: ")
                        .PromptStyle(Style.Parse("green"))
                );
            }

            var account = accountCollection.Find(x => x.AccountNumber == userAccountNumber).FirstOrDefault();

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
            double commission = 0.01 * amount; // 1% commission
            double transferAmount = amount - commission;

            senderAccount.Amount -= amount;
            receiverAccount.Amount += (int)transferAmount; // Receiver gets the full amount, excluding commission.

            Collections.GetAccountsCollection().UpdateOne(x => x.Id == senderAccount.Id, Builders<Accounts>.Update.Set(a => a.Amount, senderAccount.Amount));
            Collections.GetAccountsCollection().UpdateOne(x => x.Id == receiverAccount.Id, Builders<Accounts>.Update.Set(a => a.Amount, receiverAccount.Amount));

            TransfersController.SaveTransfer(senderAccount.Id, receiverAccount.Id, amount);

            // Call the method to transfer the commission to the bank.
            TransferCommissionToBank(commission);

            var newMovement = new Movement
            {
                DateTime = DateTime.UtcNow.ToLocalTime(),
                Amount = amount,
                Type_Id = receiverAccount.Id,
                SenderId = senderAccount.Id,
                AccountsBalance = senderAccount.Amount
            };

            Collections.GetMovementsCollection().InsertOne(newMovement);
        }

        private void TransferCommissionToBank(double commission)
        {
            // Conecta con la colección de bancos (Banks).
            var bankCollection = Collections.GetBankCollection();

            // Busca el registro de banco especial para las ganancias acumuladas.
            var bankRecord = bankCollection.Find(b => b.UsersID == ObjectId.Empty).FirstOrDefault();

            if (bankRecord != null)
            {
                // Si el registro de banco existe, aumenta 'TotalAmount' by the commission.
                var filter = Builders<Bank>.Filter.Eq(b => b.UsersID, ObjectId.Empty);
                var update = Builders<Bank>.Update.Inc(b => b.TotalAmount, commission);
                bankCollection.UpdateOne(filter, update);
            }
            else
            {
                // If no bank record exists, create a new one with 'TotalAmount' set to the commission.
                var bank = new Bank
                {
                    UsersID = ObjectId.Empty, // Use a special ID for accumulated gains.
                    AmountGains = 0,
                    TotalAmount = commission,
                    UserAccountIDs = new List<ObjectId>() // Initialize the list with no user accounts
                };
                bankCollection.InsertOne(bank);
            }
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

            // Obtén el número de cuenta del usuario logeado
            string userAccountNumber = UsersController.ObtenerNumeroDeCuentaPorUserId(MenuManager.ActiveUser);

            // Obtiene el ID de la cuenta logeada en función del número de cuenta
            ObjectId? userAccountId = AccountController.GetAccountID(userAccountNumber);

            if (userAccountId == null)
            {
                Console.WriteLine("User account not found");
                return;
            }

            // Filtra las transferencias que involucran la cuenta logeada como remitente o receptor
            var filter = Builders<Transfers>.Filter.Or(
                Builders<Transfers>.Filter.Eq("Send_Id", userAccountId),
                Builders<Transfers>.Filter.Eq("Reception_Id", userAccountId)
            );

            var transfers = Collections.GetTransfersCollection().Find(filter).ToList();

            // Crea y muestra la tabla de transferencias
            var table = new Table()
                .Title("My Transfers")
                .BorderColor(Color.Green)
                .AddColumn("Sender ID", column => column.Alignment(Justify.Left))
                .AddColumn("Receiver ID", column => column.Alignment(Justify.Left))
                .AddColumn("Amount", column => column.Alignment(Justify.Left));

            foreach (var transfer in transfers)
            {
                table.AddRow(
                    transfer.Send_Id.ToString(),
                    transfer.Reception_Id.ToString(),
                    transfer.Amount.ToString()
                );
            }

            AnsiConsole.Clear();
            AnsiConsole.Render(table);

            AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
            Console.ReadLine();
        }
    }
}