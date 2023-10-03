﻿using MongoDB.Bson;
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

            Collections.GetSavingsGroupCollectionOriginal().InsertOne(insertion);
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
                .Title("select a user to invite")
                .AddChoices(UsersEnabledToInviteArray));
            return UsersController.ObtenerIdPorUsername(option);
        }
        public static ObjectId? Invite(ObjectId? repetido)
        {
            var list = GetUsersEnabledToInvite(repetido);
            if (list.Count == 0)
            {
                Console.WriteLine("there are no users to invite");
                return null;
            }
            var UsersEnabledToInviteArray = list.Select(x => x["User"].AsString).ToArray();
            string option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("select a user to invite")
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



    }
}
