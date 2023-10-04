using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using udembankproject.Models;

namespace udembankproject.Controllers
{
    internal class LoansController
    {
        public static void SaveLoan(ObjectId sendId, ObjectId receptionId, int amount)
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
    }
}
