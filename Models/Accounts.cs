using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace udembankproject.Models
{
    public class Accounts
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; }

        [BsonElement("Owner name")]
        public string? OwnerName { get; set; }

        [BsonElement("Account Number")]

        public string? AccountNumber { get; set; }

       
        [BsonElement("Amount")]

        public int Amount { get; set; }
    }
}
