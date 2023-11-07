using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace udembankproject.Models
{
    public class Bank
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; }

        [BsonElement("UsersID")]
        public ObjectId? UsersID { get; set; }

        [BsonElement("UserAccountIDs")]
        public List<ObjectId> UserAccountIDs { get; set; }

        [BsonElement("AmountGains")]
        public double AmountGains { get; set; }

        [BsonElement("TotalAmount")] 
        public double TotalAmount { get; set; }

    }
}