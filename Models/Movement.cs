using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace udembankproject.Models
{
    public class Movement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; }

        [BsonElement("DateTime")]
        public DateTime DateTime { get; set; }

        [BsonElement("Amount")]

        public int Amount { get; set; }

        [BsonElement("Type_Id")]
        public ObjectId Type_Id { get; set; }

        [BsonElement("AccountsBalance")]
        public int AccountsBalance { get; set;}

    }
}
