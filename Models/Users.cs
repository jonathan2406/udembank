﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace udembankproject
{
    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("User")]
        public string User { get; set; }

        [BsonElement("Password")]

        public string Password { get; set; }

        [BsonElement("AccountID")]
        public ObjectId? AccountID { get; set; }

        [BsonElement("Cheats")]
        public bool Cheats { get; set; }
    }
}
