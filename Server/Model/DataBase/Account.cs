using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    [ObjectSystem]
    public class UserDBAwakeSystem: AwakeSystem<UserDB>
    {
        public override void Awake(UserDB self)
        {
            self.Awake();
        }
    }
    /// <summary>
    /// 登陆时候检查用的
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Account:ETModel.Entity
    {
        public string Name;

        public string Password;
    }

    /// <summary>
    /// 存用户数据的
    /// </summary>
    [BsonIgnoreExtraElements]
    public class UserDB : ETModel.Entity
    {
        public string Name;

        public int Level;

        public int Experience;

        public void Awake()
        {
            this.Level = 1;

            this.Experience = 0;
        }
    }
}
