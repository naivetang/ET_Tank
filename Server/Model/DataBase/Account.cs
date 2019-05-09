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
        public UInt64 PhoneNum;

        public string UserName;

        public string Password;
    }

    /// <summary>
    /// 存用户数据的
    /// </summary>
    [BsonIgnoreExtraElements]
    public class UserDB : ETModel.Entity
    {
        public UInt64 PhoneNum;
    }


    public class UserBaseComponent : Component, ISerializeToEntity
    {
        public string UserName;

        public int Level = 1;

        public int Experience = 0;

        public int Gold = 0;
    }

    [BsonIgnoreExtraElements]
    public class SettingInfoComponent: Component , ISerializeToEntity
    {
        public Language Language = Language.Chinese;

        public int Volume = 100;

        public int BinarySwitch = 0x0003;

        public int RotSpeed = 50;
    }

    [ObjectSystem]
    public class WarehouseAwake:AwakeSystem<WarehouseComponent>
    {
        public override void Awake(WarehouseComponent self)
        {
            self.Awake();
        }
    }

    public class PropItem
    {
        public PropItem(){}
        
        public PropItem(PropItem other)
        {
            this.TableId = other.TableId;
            this.BuyTime = other.BuyTime;
            this.TotalTimes = other.TotalTimes;
            this.UseTimes = other.UseTimes;
        }

        public int TableId;

        /// <summary>
        /// 当前是否使用状态
        /// </summary>
        public PropState PropState;

        /// <summary>
        /// 购买时间
        /// </summary>
        public long BuyTime;

        /// <summary>
        /// 总共可使用次数
        /// </summary>
        public int TotalTimes;


        /// <summary>
        /// 已经使用次数
        /// </summary>
        public int UseTimes;

        public int Num;
    }

    /// <summary>
    /// 仓库
    /// </summary>
    public class WarehouseComponent: Component, ISerializeToEntity
    {
        public void Awake()
        {
            
            this.Tanks.Add(10001);

            this.UseTankId = 10001;

            this.Bullets.Add(20001);

            this.UseBulletId = 20001;

            // PropItem item = new PropItem();
            //
            // item.TableId = 30001;
            //
            // item.BuyTime = TimeHelper.NowSecond();
            //
            // item.StopTime = TimeHelper.NowSecond() + 100000;
            //
            // this.UnUseProps.Add(item);

        }

        public List<int> Tanks = new List<int>();

        public int UseTankId;

        public List<int> Bullets = new List<int>();

        public int UseBulletId;

        public List<PropItem> UnUseProps = new List<PropItem>();
        public List<PropItem> InUseProps = new List<PropItem>();

        public bool isExitTank(int tankId)
        {
            if (this.Tanks.Contains(tankId))
                return true;
            return false;
        }

        public bool isExitBullet(int bulletId)
        {
            if (this.Bullets.Contains(bulletId))
                return true;
            return false;
        }

        public PropItem GetUnUseProp(int propId)
        {
            foreach (PropItem propItem in this.UnUseProps)
            {
                if (propItem.TableId == propId)
                {
                    return propItem;
                }
            }

            return null;
        }

        public PropItem GetInUseProp(int propId)
        {
            foreach (PropItem propItem in this.InUseProps)
            {
                if (propItem.TableId == propId)
                {
                    return propItem;
                }
            }

            return null;
        }
    }


}
