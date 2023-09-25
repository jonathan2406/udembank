using MongoDB.Bson;
using MongoDB.Driver;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace udembankproject.Controllers
{
    internal class SavingGroupController
    {
        public static void AddSavingGroup()
        {
            var NameGroup = AnsiConsole.Ask<string>("Name group: ");
            string option = Invite();
            Console.WriteLine(option);
            
        }

        public static string Invite()
        {
            var list = GetUsersEnabledToInvite();
            var UsersEnabledToInviteArray = list.Select(x => x["User"].AsString).ToArray();
            string option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("select a user to invite")
                .AddChoices(UsersEnabledToInviteArray));
            return option;

        }

        public static List<BsonDocument> GetUsersEnabledToInvite()
        {
            IMongoDatabase database = DBconnection.Connection();
            var usersCollection = database.GetCollection<BsonDocument>("Users");
            var savingsGroupCollection = database.GetCollection<BsonDocument>("SavingsGroup");

            var usuarios = usersCollection.Find(new BsonDocument()).ToList();
            List<BsonDocument> usuariosMenosDe3Veces = new List<BsonDocument>();

            var activeUserName = MenuManager.ActiveUser; // Obtener el nombre de usuario activo

            foreach (var usuario in usuarios)
            {
                var usuarioId = usuario["_id"];

                // Verificar si el nombre de usuario es diferente al nombre de la variable ActiveUser
                if (usuario["User"] != BsonValue.Create(activeUserName))
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("UsersID", usuarioId);
                    var conteo = savingsGroupCollection.Find(filter).ToList().Count;

                    if (conteo < 3)
                    {
                        usuariosMenosDe3Veces.Add(usuario);
                    }
                }
            }
            return usuariosMenosDe3Veces;
        }

    }
}
