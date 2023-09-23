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
    public class Transfers
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; }

        [BsonElement("Send_Id")]
        public ObjectId Send_Id { get; set; }

        [BsonElement("Reception_Id")]
        public ObjectId Reception_Id { get; set; }

        [BsonElement("Amount")]
        public int Amount { get; set; }

    }
}
