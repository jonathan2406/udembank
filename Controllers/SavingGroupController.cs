using MongoDB.Bson;
using MongoDB.Driver;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            ObjectId? FirstUser = Invite();
            ObjectId? SecondUser = null;
            if (FirstUser == null)
            {
                Console.WriteLine("The group could not be created because there is not at least one user to add");
                return;
            }
            var menuSecondUser = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Do you want to add a second user?")
                        .AddChoices("Yes", "No")
                );

            switch (menuSecondUser)
            {
                case "Yes":
                    SecondUser = Invite(FirstUser);
                    break;
                case "No":
                    break;
            }
            Savings_Group insertion;
            List<ObjectId?> idslist = new List<ObjectId?> { };
            idslist.Add(MenuManager.ActiveUser);
            idslist.Add(FirstUser);
            if (SecondUser == null)
            {
                insertion = new Savings_Group
                {
                    
                    Name = NameGroup,
                    UsersID = idslist,
                    Amount = 0
                };
            }
            else
            {
                idslist.Add(SecondUser);
                insertion = new Savings_Group
                {
                    Name = NameGroup,
                    UsersID = idslist,
                    Amount = 0
                };
            }

            Collections.GetSavingsGroupCollection().InsertOne(insertion);
            Console.WriteLine("Successful creation");
            Thread.Sleep(2000);
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

        public static List<BsonDocument> ObtenerGruposParaUsuarioActivo()
        {
            var activeUserId = MenuManager.ActiveUser;

            var filter = Builders<BsonDocument>.Filter.AnyEq("UsersID", activeUserId);
            var grupos = Collections.GetSavingsGroupCollectionBson().Find(filter).ToList();
            return grupos;
        }

        public static BsonDocument? SelectSavingGroupToTransfer()
        {
            var list = ObtenerGruposParaUsuarioActivo();
            if (list.Count == 0)
            {
                return null;
            }
            var eo = list.Select(x => x["Name"].AsString).ToArray();
            string selectedName = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("select a user to invite")
                .AddChoices(eo));

            // Ahora, busca el documento correspondiente al nombre seleccionado
            BsonDocument option = list.First(x => x["Name"].AsString == selectedName);

            return option;
        }

        public static void TransferToSavingGroup()
        {
            var BsonTranfer = SelectSavingGroupToTransfer();
            if (BsonTranfer == null)
            {
                Console.WriteLine("savingsGroup in which these were not found");
                return;
            }
            var amountToTransfer = AnsiConsole.Prompt(new TextPrompt<int>("Amount to transfer: ")
                .PromptStyle(Style.Parse("green"))
                );

            RestarMontoAlSavingsGroup(BsonTranfer, amountToTransfer);
            AccountController.RestarMontoAlAccount(MenuManager.ActiveUser, amountToTransfer);
            var savingsGroupID = BsonTranfer["_id"].AsObjectId;

            TransfersController.SaveTransfer(MenuManager.ActiveUser, savingsGroupID, amountToTransfer);

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
            var savingGroups = Collections.GetSavingsGroupCollection().Find(_ => true).ToList();
            // Crea y muestra la tabla de cuentas

            var table = new Table()
                .Title("SavingsGroup")
                .BorderColor(Color.Green)
                .AddColumn("Name", column => column.Alignment(Justify.Left))
                .AddColumn("UsersID", column => column.Alignment(Justify.Left))
                .AddColumn("Amount", column => column.Alignment(Justify.Left))
                .AddColumn("ID", column => column.Alignment(Justify.Left));

            foreach (var savingGroup in savingGroups)
            {
                // Convierte la lista de UsersID a una cadena separada por comas
                string usersIdsString = string.Join(", ", savingGroup.UsersID);

                table.AddRow(
                    savingGroup.Name,
                    usersIdsString, // Usa la cadena creada en lugar de savingGroup.UsersID.ToString()
                    savingGroup.Amount.ToString(),
                    savingGroup.Id.ToString()
                );
            }

            AnsiConsole.Clear();
            AnsiConsole.Render(table);

            AnsiConsole.Markup("[yellow] Press enter to continue...[/]");
            Console.ReadLine();
        }
    }
}
