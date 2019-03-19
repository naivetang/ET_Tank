using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class Account:ETModel.Entity
    {
        public string UserName;

        public string Password;
    }
}
