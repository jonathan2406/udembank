using udembankproject.Models;
using MongoDB.Driver;
using System;

namespace udembankproject
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb+srv://LassoVB:lasso123@udembank.qozsdkg.mongodb.net/");
            var database = client.GetDatabase("UdemBank");
            var peopleDB = database.GetCollection<Accounts>("Accounts");
            //var people = new Usuarios() { Nombre = "Lasso", Edad = 18 };
            //peopleDB.InsertOne(people);


            var gentusa = new List<Gente>()
            {
                new Gente() { Nombre = "Lasso" , Edad = 18},
                new Gente() { Nombre = "Adely" , Edad = 21 },
                new Gente() { Nombre = "John Wick", Edad = 44}

            };

            var nombres = from g in gentusa
                          select g.Nombre;

            foreach (var nombre in nombres)
            {
                Console.WriteLine(nombre);
            }
        }


        public class Gente
        {
            public string? Nombre { get; set; }

            public int? Edad { get; set; }
        }


    }
}