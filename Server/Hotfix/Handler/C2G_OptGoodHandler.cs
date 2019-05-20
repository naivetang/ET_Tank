
using System;
using System.Collections.Generic;
using ETModel;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    class C2G_OptGoodHandler:AMHandler<C2G_OptGood>
    {
        protected override void Run(Session session, C2G_OptGood message)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;

            WarehouseComponent warehouse = player.UserDB.GetComponent<WarehouseComponent>();

            UserBaseComponent userBase = player.UserDB.GetComponent<UserBaseComponent>();

            switch (message.GoodType)
            {
                case GoodType.Tank:
                    TankCfg tankInfo = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(TankCfg), message.TableId) as TankCfg;

                    if (message.GoodOpt == GoodOpt.Use && warehouse.UseTankId != message.TableId
                        && warehouse.isExitTank(message.TableId))
                    {
                        warehouse.UseTankId = message.TableId;
                        Send_G2C_Warehouse(player);
                        player.Send_PopMessage(1083);
                    }

                    if (message.GoodOpt == GoodOpt.Buy)
                    {
                        if (warehouse.isExitTank(message.TableId))
                        {

                            player.Send_PopMessage(1084);
                        }
                        else if (userBase.Gold < tankInfo.Price)
                        {
                            player.Send_PopMessage(1086);
                        }
                        else
                        {
                            userBase.Gold -= tankInfo.Price;
                            player.Send_PopMessage(1087);
                            warehouse.Tanks.Add(message.TableId);
                        }
                    }
                    break;
                case GoodType.Bullet:
                    BulletCfg bulletInfo = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(BulletCfg), message.TableId) as BulletCfg;
                    if (message.GoodOpt == GoodOpt.Use && warehouse.UseBulletId != message.TableId
                                                       && warehouse.isExitBullet(message.TableId))
                    {
                        warehouse.UseBulletId = message.TableId;
                        Send_G2C_Warehouse(player);
                        player.Send_PopMessage(1085);
                    }

                    if (message.GoodOpt == GoodOpt.Buy)
                    {
                        if (warehouse.isExitBullet(message.TableId))
                        {
                            player.Send_PopMessage(1084);
                        }
                        else if (userBase.Gold < bulletInfo.Price)
                        {
                            player.Send_PopMessage(1086);
                        }
                        else
                        {
                            userBase.Gold -= bulletInfo.Price;
                            player.Send_PopMessage(1087);
                            warehouse.Bullets.Add(message.TableId);
                        }
                    }

                    break;
                case GoodType.Prop:
                    Prop propInfo = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Prop), message.TableId) as Prop;

                    if (message.GoodOpt == GoodOpt.Use && warehouse.GetUnUseProp(message.TableId)!=null)
                    {
                        PropItem unUseItem = warehouse.GetUnUseProp(message.TableId);

                        PropItem inUseItem = warehouse.GetInUseProp(message.TableId);


                        if (inUseItem != null)
                        {
                            inUseItem.TotalTimes += unUseItem.TotalTimes;

                            unUseItem.Num -= 1;

                            if(unUseItem.Num <= 0)
                                warehouse.UnUseProps.Remove(unUseItem);
                        }
                        else
                        {
                            inUseItem = new PropItem(unUseItem);

                            inUseItem.PropState = PropState.InUser;

                            inUseItem.Num = 1;

                            warehouse.InUseProps.Add(inUseItem);

                            unUseItem.Num -= 1;

                            if (unUseItem.Num <= 0)
                                warehouse.UnUseProps.Remove(unUseItem);
                        }

                        Send_G2C_Warehouse(player);
                        player.Send_PopMessage(1085);
                    }

                    if (message.GoodOpt == GoodOpt.Buy)
                    {

                        if (userBase.Gold < propInfo.Price)
                        {
                            player.Send_PopMessage(1086);
                            break;
                        }

                        PropItem propItem = warehouse.GetUnUseProp(message.TableId);

                        if (propItem != null)
                        {
                            propItem.Num += 1;
                        }
                        else
                        {
                            propItem = new PropItem();

                            propItem.PropState = PropState.WaitUse;

                            propItem.TableId = message.TableId;

                            propItem.BuyTime = TimeHelper.NowSecond();

                            propItem.Num = 1;

                            propItem.TotalTimes = (Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Prop), message.TableId) as Prop).TotleTimes;

                            warehouse.UnUseProps.Add(propItem);
                        }

                        userBase.Gold -= propInfo.Price;

                        player.Send_PopMessage(1087);


                        Send_G2C_Warehouse(player);
                    }

                    break;
                default:
                    Log.Error($"不存在的GoodType:{message.GoodType}");
                    break;
            }

            
        }

        private void Send_G2C_Warehouse(Player player)
        {
            G2C_Warehouse msg = new G2C_Warehouse();

            WarehouseComponent warehouseComponent = player.UserDB.GetComponent<WarehouseComponent>();

            msg.Tanks = new RepeatedField<int>();

            Utility.List2RepeatedField(warehouseComponent.Tanks, msg.Tanks);

            msg.TankId = warehouseComponent.UseTankId;

            msg.Bullets = new RepeatedField<int>();

            msg.BulletId = warehouseComponent.UseBulletId;

            Utility.List2RepeatedField(warehouseComponent.Bullets, msg.Bullets);

            msg.Props = new RepeatedField<PropInfo>();

            List<PropItem> propItems = warehouseComponent.UnUseProps;

            foreach (PropItem propItem in propItems)
            {
                PropInfo tmp = new PropInfo();

                tmp.TableId = propItem.TableId;

                tmp.PropState = PropState.WaitUse;

                tmp.TotalTimes = propItem.TotalTimes;

                tmp.Num = propItem.Num;

                msg.Props.Add(tmp);
            }
            propItems = warehouseComponent.InUseProps;

            foreach (PropItem propItem in propItems)
            {
                PropInfo tmp = new PropInfo();

                tmp.TableId = propItem.TableId;

                tmp.PropState = PropState.InUser;

                tmp.TotalTimes = propItem.TotalTimes;

                tmp.UseTimes = propItem.UseTimes;

                msg.Props.Add(tmp);
            }

            player.Session.Send(msg);
        }
    }
}
