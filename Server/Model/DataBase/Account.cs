using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
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
    }

    public class UserBaseComponent : Component, ISerializeToEntity
    {
        public int Level = 1;

        public int Experience = 0;
    }

    [BsonIgnoreExtraElements]
    public class SettingInfoComponent: Component , ISerializeToEntity
    {
        public Language Language = Language.Chinese;

        public int Volume = 100;

        public int BinarySwitch = 0x0001;

        public int RotSpeed = 50;
    }
}
