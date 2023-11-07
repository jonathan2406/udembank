using MongoDB.Bson;
using MongoDB.Driver;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using udembankproject.Models;

namespace udembankproject.Controllers
{
    internal class SavingGroupController
    {
        public static void AddSavingGroup()
        {
            var NameGroup = AnsiConsole.Ask<string>("Name group: ");

            // Obtén el número de cuenta del usuario logeado
            string userAccountNumber = UsersController.ObtenerNumeroDeCuentaPorUserId(MenuManager.ActiveUser);
            ObjectId? userAccountId = AccountController.GetAccountID(userAccountNumber);

            if (userAccountId == null)
            {
                Console.WriteLine("User account not found");
                return;
            }

            // Verifica si el usuario logeado ya está en tres grupos de ahorro
            if (!IsUserAccountInLessThanThreeGroups(userAccountId))
            {
                Console.WriteLine("You are already in three savings groups and cannot create another.");
                return;
            }

            var menuSecondUser = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Do you want to add a second user?")
                .AddChoices("Yes", "No")
            );

            ObjectId? SecondUser = null;
            ObjectId? ThirdUser = null;

            if (menuSecondUser == "Yes")
            {
                string SecondAccountNumber = AnsiConsole.Ask<string>("Enter the Account Number of the second user: ");
                SecondUser = AccountController.GetAccountID(SecondAccountNumber);

                // Verifica si se puede agregar un tercer usuario
                if (SecondUser != null)
                {
                    if (!IsUserAccountInLessThanThreeGroups(SecondUser))
                    {
                        Console.WriteLine("The second user is already in three savings groups and cannot be added to another.");
                        return;
                    }

                    var menuThirdUser = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .Title("Do you want to add a third user?")
                        .AddChoices("Yes", "No")
                    );

                    if (menuThirdUser == "Yes")
                    {
                        string ThirdAccountNumber = AnsiConsole.Ask<string>("Enter the Account Number of the third user: ");
                        ThirdUser = AccountController.GetAccountID(ThirdAccountNumber);

                        // Verifica si se puede agregar un tercer usuario
                        if (ThirdUser != null && !IsUserAccountInLessThanThreeGroups(ThirdUser))
                        {
                            Console.WriteLine("The third user is already in three savings groups and cannot be added to another.");
                            return;
                        }
                    }
                }
            }

            Savings_Group insertion;
            List<ObjectId?> accountIdsList = new List<ObjectId?> { };

            accountIdsList.Add(userAccountId);

            if (SecondUser != null)
            {
                accountIdsList.Add(SecondUser);
            }

            if (ThirdUser != null)
            {
                accountIdsList.Add(ThirdUser);
            }

            insertion = new Savings_Group
            {
                Name = NameGroup,
                UsersID = accountIdsList,
                Amount = 0,
                Contributions = new Dictionary<ObjectId, int>()
            };

            Collections.GetSavingsGroupCollection().InsertOne(insertion);

            AnsiConsole.Markup("[yellow]Successful creation...[/]");
            Thread.Sleep(2000);
        }
        private static bool IsUserAccountInLessThanThreeGroups(ObjectId? userAccountId)
        {
            var filter = Builders<BsonDocument>.Filter.AnyEq("UsersID", userAccountId);
            var count = Collections.GetSavingsGroupCollectionBson().Find(filter).CountDocuments();

            return count < 3;
        }

        public static ObjectId? Invite()
        {
            var list = GetUsersEnabledToInvite();
            if (list.Count == 0)
            {
                return null;
            }
            var UsersEnabledToInviteArray = list.Select(x => x["User"].AsString).ToArray();
            string option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Select a user to invite")
                .AddChoices(UsersEnabledToInviteArray));
            return UsersController.ObtenerIdPorUsername(option);
        }
        public static ObjectId? Invite(ObjectId? repetido)
        {
            var list = GetUsersEnabledToInvite(repetido);
            if (list.Count == 0)
            {
                Console.WriteLine("There are no users to invite");
                return null;
            }
            var UsersEnabledToInviteArray = list.Select(x => x["User"].AsString).ToArray();
            string option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Select a user to invite")
                .AddChoices(UsersEnabledToInviteArray));
            return UsersController.ObtenerIdPorUsername(option);
        }

        public static List<BsonDocument> GetUsersEnabledToInvite()
        {
            var usuarios = Collections.GetUsersCollectionBson().Find(new BsonDocument()).ToList();
            List<BsonDocument> usuariosMenosDe3Veces = new List<BsonDocument>();

            ObjectId activeUserName = MenuManager.ActiveUser; // Obtener el nombre de usuario activo

            foreach (var usuario in usuarios)
            {
                var usuarioId = usuario["_id"];

                // Verificar si el nombre de usuario es diferente al nombre de la variable ActiveUser
                if (usuario["_id"] != BsonValue.Create(activeUserName))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("UsersID", usuarioId);
                    var conteo = Collections.GetSavingsGroupCollectionBson().Find(filter).ToList().Count;

                    if (conteo < 3)
                    {
                        usuariosMenosDe3Veces.Add(usuario);
                    }
                }
            }
            return usuariosMenosDe3Veces;
        }
        public static List<BsonDocument> GetUsersEnabledToInvite(ObjectId? repetido)
        {
            var usuarios = Collections.GetUsersCollectionBson().Find(new BsonDocument()).ToList();
            List<BsonDocument> usuariosMenosDe3Veces = new List<BsonDocument>();

            ObjectId activeUserName = MenuManager.ActiveUser; // Obtener el nombre de usuario activo

            foreach (var usuario in usuarios)
            {
                var usuarioId = usuario["_id"];

                // Verificar si el nombre de usuario es diferente al nombre de la variable ActiveUser
                if (usuario["_id"] != BsonValue.Create(activeUserName) && usuario["_id"] != BsonValue.Create(repetido))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("UsersID", usuarioId);
                    var conteo = Collections.GetSavingsGroupCollectionBson().Find(filter).ToList().Count;

                    if (conteo < 3)
                    {
                        usuariosMenosDe3Veces.Add(usuario);
                    }
                }
            }
            return usuariosMenosDe3Veces;
        }
        public static bool VerificarAparicionesMenosDeTresVeces(ObjectId userId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("UsersID", userId);
            var conteo = Collections.GetSavingsGroupCollectionBson().Find(filter).CountDocuments();

            return conteo < 3;
        }

        //--------------------Transfer savings group

        public static List<BsonDocument> ObtenerGruposParaUsuarioActivo(ObjectId? userAccountId)
        {
            var filter = Builders<BsonDocument>.Filter.AnyEq("UsersID", userAccountId);
            var grupos = Collections.GetSavingsGroupCollectionBson().Find(filter).ToList();
            return grupos;
        }

        public static BsonDocument? SelectSavingGroupToTransfer(ObjectId? userAccountId)
        {
            var list = ObtenerGruposParaUsuarioActivo(userAccountId);
            if (list.Count == 0)
            {
                Console.WriteLine("No savings groups found.");
                return null;
            }

            var eo = list.Select(x => x["Name"].AsString).ToArray();
            string selectedName = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Select a savings group to dissolve")
                .AddChoices(eo));

            // Ahora, busca el documento correspondiente al nombre seleccionado
            BsonDocument option = list.First(x => x["Name"].AsString == selectedName);

            return option;
        }
        public static void TransferToSavingGroup()
        {
            string userAccountNumber = UsersController.ObtenerNumeroDeCuentaPorUserId(MenuManager.ActiveUser);

            // Obtén el ID de la cuenta del usuario logeado en función del número de cuenta
            ObjectId? userAccountId = AccountController.GetAccountID(userAccountNumber);

            if (userAccountId == null)
            {
                Console.WriteLine("User account not found");
                return;
            }

            // Selecciona el grupo de ahorro en función del ID de la cuenta del usuario logeado
            var BsonTranfer = SelectSavingGroupToTransfer(userAccountId);

            if (BsonTranfer == null)
            {
                Console.WriteLine("SavingsGroup in which these were not found");
                return;
            }

            var amountToTransfer = AnsiConsole.Prompt(new TextPrompt<int>("Amount to transfer: ")
                .PromptStyle(Style.Parse("green"))
            );

            // Resta el monto del grupo de ahorro
            RestarMontoAlSavingsGroup(BsonTranfer, amountToTransfer);

            // Obtener el saldo actual de la cuenta del usuario
            var userAccount = Collections.GetAccountsCollection().Find(x => x.Id == userAccountId).FirstOrDefault();

            if (userAccount == null)
            {
                Console.WriteLine("User account not found");
                return;
            }

            // Calcula el nuevo saldo restando el monto transferido
            var newBalance = userAccount.Amount - amountToTransfer;

            // Acceder al diccionario de contribuciones en BsonTranfer
            var contributions = BsonTranfer.GetValue("Contributions").AsBsonDocument;

            // Actualiza el diccionario de contribuciones con la contribución del usuario
            contributions[userAccountId.ToString()] = amountToTransfer;

            // Resta el monto de la cuenta del usuario
            AccountController.RestarMontoAlAccount(userAccountId.Value, amountToTransfer);

            var savingsGroupID = BsonTranfer["_id"].AsObjectId;
            var filter = Builders<BsonDocument>.Filter.Eq("_id", savingsGroupID);
            var update = Builders<BsonDocument>.Update.Set("Contributions", contributions);
            Collections.GetSavingsGroupCollectionBson().UpdateOne(filter, update);

            // Crear un objeto de movimiento para la transferencia a un grupo de ahorro
            var movementToSavingsGroup = new Movement
            {
                DateTime = DateTime.UtcNow, // Obtener la fecha y hora actual
                Amount = amountToTransfer,
                Type_Id = savingsGroupID, // Usar el ID del grupo de ahorro como Type_Id
                SenderId = userAccountId.Value, // Marcar al usuario logeado como remitente
                AccountsBalance = newBalance // Usar el nuevo saldo como AccountsBalance
            };

            // Guardar el movimiento en la base de datos
            Collections.GetMovementsCollection().InsertOne(movementToSavingsGroup);

            Console.WriteLine("Transfer to savings group successful");

            TransfersController.SaveTransfer(userAccountId.Value, savingsGroupID, amountToTransfer);
        }

        public static void RestarMontoAlSavingsGroup(BsonDocument savingsGroup, int monto)
        {
            var savingsGroupID = savingsGroup["_id"].AsObjectId;
            var filterSavingsGroup = Builders<BsonDocument>.Filter.Eq("_id", savingsGroupID);
            var update = Builders<BsonDocument>.Update.Inc("Amount", +monto); // Restar el monto al amount
            Collections.GetSavingsGroupCollectionBson().UpdateOne(filterSavingsGroup, update);
        }

        public static void ViewMySavingsGroups()
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

            // Filtra los grupos de ahorro que contienen la cuenta logeada en la lista UsersID
            var filter = Builders<Savings_Group>.Filter.AnyEq("UsersID", userAccountId);
            var savingGroups = Collections.GetSavingsGroupCollection().Find(filter).ToList();

            // Crea y muestra la tabla de grupos de ahorro
            var table = new Table()
                .Title("My Savings Groups")
                .BorderColor(Color.Green)
                .AddColumn("Name", column => column.Alignment(Justify.Left))
                .AddColumn("UsersID", column => column.Alignment(Justify.Left))
                .AddColumn("Amount", column => column.Alignment(Justify.Left))
                .AddColumn("Contributions", column => column.Alignment(Justify.Left))
                .AddColumn("ID", column => column.Alignment(Justify.Left));

            foreach (var savingGroup in savingGroups)
            {
                // Convierte la lista de UsersID a una cadena separada por comas
                string usersIdsString = string.Join(", ", savingGroup.UsersID);

                table.AddRow(
                    savingGroup.Name,
                    usersIdsString,
                    savingGroup.Amount.ToString(),
                    GetContributionsString(savingGroup.Contributions),
                    savingGroup.Id.ToString()
                );
            }

            AnsiConsole.Clear();
            AnsiConsole.Render(table);

            AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
            Console.ReadLine();
        }
        private static string GetContributionsString(Dictionary<ObjectId, int> contributions)
        {
            StringBuilder contributionString = new StringBuilder();

            foreach (var kvp in contributions)
            {
                contributionString.Append($"UserID: {kvp.Key}, Amount: {kvp.Value}\n");
            }

            return contributionString.ToString();
        }
        public static void DissolveSavingsGroup()
        {
            var userAccountNumber = UsersController.ObtenerNumeroDeCuentaPorUserId(MenuManager.ActiveUser);
            ObjectId? userAccountId = AccountController.GetAccountID(userAccountNumber);

            if (userAccountId == null)
            {
                Console.WriteLine("User account not found");
                return;
            }

            // Selecciona el grupo de ahorro a disolver
            var savingsGroupToDissolve = SelectSavingGroupToTransfer(userAccountId);

            if (savingsGroupToDissolve == null)
            {
                Console.WriteLine("SavingsGroup to dissolve not found");
                return;
            }

            var userAccountIDs = savingsGroupToDissolve["UsersID"].AsBsonArray
            .Select(id => id.AsObjectId)
            .ToList();
            // Obtén el ID del grupo de ahorro
            ObjectId groupId = savingsGroupToDissolve["_id"].AsObjectId;
            // Calcula el saldo total del grupo
            var groupBalance = savingsGroupToDissolve["Amount"].AsInt32;
            // Calcula la comisión del banco (5% del saldo del grupo)
            var bankCommission = (int)(groupBalance * 0.05);

            // Calcula la cantidad restante después de la comisión
            var remainingBalance = groupBalance - bankCommission;

            // Divide el saldo restante entre los usuarios del grupo
            var usersInGroup = savingsGroupToDissolve["UsersID"].AsBsonArray.Select(id => id.AsObjectId).ToList();
            // Crea un diccionario para almacenar los montos transferidos por cada usuario
            var userTransfers = new Dictionary<ObjectId, int>();

            foreach (var userId in usersInGroup)
            {
                // Llama a la función para obtener el monto transferido por el usuario en ese grupo
                var transferAmount = ObtenerMontoTransferidoPorUsuario(userId, groupId);
                userTransfers[userId] = transferAmount;
            }

            // Calcula la contribución total de todos los usuarios
            var totalContributions = userTransfers.Values.Sum();

            // Distribuye el saldo restante en función de los montos transferidos
            foreach (var userId in usersInGroup)
            {
                var userShare = (int)(remainingBalance * ((double)userTransfers[userId] / totalContributions));
                AccountController.AgregarMontoAlAccount(userId, userShare);
            }

            // Transfiere el 5% de la comisión al banco
            TransferCommissionToBank(bankCommission, userAccountIDs);

            // Borra el grupo de ahorro
            var groupToDelete = Builders<BsonDocument>.Filter.Eq("_id", groupId);
            Collections.GetSavingsGroupCollectionBson().DeleteOne(groupToDelete);

            Console.WriteLine("Savings group dissolved, and funds divided based on user transfers. Group deleted.");
        }
        public static int ObtenerMontoTransferidoPorUsuario(ObjectId userId, ObjectId groupId)
        {
            // Conecta con la colección de movimientos (Movements).
            var movementsCollection = Collections.GetMovementsCollection();

            // Define un filtro para buscar los movimientos del usuario en el grupo de ahorro específico.
            var filter = Builders<Movement>.Filter.And(
                Builders<Movement>.Filter.Eq("SenderId", userId),  // Filtra por el ID del usuario como remitente.
                Builders<Movement>.Filter.Eq("Type_Id", groupId)  // Filtra por el ID del grupo de ahorro específico.
            );

            // Realiza una consulta para obtener los movimientos del usuario en el grupo de ahorro.
            var userMovements = movementsCollection.Find(filter).ToList();

            // Suma los montos de los movimientos del usuario en el grupo de ahorro.
            int totalTransferAmount = userMovements.Sum(movement => movement.Amount);

            return totalTransferAmount;
        }
        public static void TransferCommissionToBank(int commissionAmount, List<ObjectId> userAccountIDs)
        {
            // Conecta con la colección de bancos (Banks).
            var bankCollection = Collections.GetBankCollection();

            // Busca el registro de banco especial para las ganancias acumuladas.
            var bankRecord = bankCollection.Find(x => x.UsersID == ObjectId.Empty).FirstOrDefault();

            if (bankRecord != null)
            {
                // Si el registro de banco existe, establece 'AmountGains' en la comisión actual y aumenta 'TotalAmount'.
                var filter = Builders<Bank>.Filter.Eq(x => x.UsersID, ObjectId.Empty);
                var update = Builders<Bank>.Update
                    .Set(x => x.AmountGains, commissionAmount)
                    .Inc(x => x.TotalAmount, commissionAmount)
                    .PushEach(x => x.UserAccountIDs, userAccountIDs); // Agregar los IDs de las cuentas de usuario al arreglo
                bankCollection.UpdateOne(filter, update);
            }
            else
            {
                // Si no existe un registro de banco para las ganancias acumuladas, crea uno nuevo con la comisión actual y los IDs de las cuentas de usuario.
                var bank = new Bank
                {
                    UsersID = ObjectId.Empty, // Usar un ID especial para las ganancias acumuladas.
                    AmountGains = commissionAmount,
                    TotalAmount = commissionAmount,
                    UserAccountIDs = userAccountIDs // Establecer los IDs de las cuentas de usuario
                };
                bankCollection.InsertOne(bank);
            }
        }
    }
}