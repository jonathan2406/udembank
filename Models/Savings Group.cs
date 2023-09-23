using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace udembankproject.Models
{
    public class Savings_Group
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("UsersID")]
        public ObjectId UsersID { get; set; }

        [BsonElement("Amount")]

        public int Amount { get; set; }
    }
}
