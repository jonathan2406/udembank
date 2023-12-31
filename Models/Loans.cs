﻿using System;
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
    public class Loans
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; }

        [BsonElement("Interest_Percent")]
        public double Interest_Percent { get; set; }

        [BsonElement("Id_Account")]
        public ObjectId Id_Account{ get; set; }

        [BsonElement("Id_group")]
        public ObjectId Id_group { get; set; }

        [BsonElement("Amount")]
        public int Amount { get; set; }

        [BsonElement("IsPaid")]
        public bool IsPaid { get; set; }

        [BsonElement("LoanTermMonths")]
        public int LoanTermMonths { get; set; }
    }
}
