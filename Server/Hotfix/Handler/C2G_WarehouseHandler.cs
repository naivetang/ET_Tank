using System;
using System.Collections.Generic;
using ETModel;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_WarehouseHandler:AMRpcHandler<C2G_Warehouse,G2C_Warehouse>
    {
        protected override void Run(Session session, C2G_Warehouse message, Action<G2C_Warehouse> reply)
        {
            G2C_Warehouse response = new G2C_Warehouse();

            try
            {
                Player player = session.GetComponent<SessionPlayerComponent>().Player;

                WarehouseComponent warehouseComponent = player.UserDB.GetComponent<WarehouseComponent>();


                response.Tanks = new RepeatedField<int>();

                Utility.List2RepeatedField(warehouseComponent.Tanks, response.Tanks);

                response.TankId = warehouseComponent.UseTankId;

                response.Bullets = new RepeatedField<int>();

                response.BulletId = warehouseComponent.UseBulletId;

                Utility.List2RepeatedField(warehouseComponent.Bullets, response.Bullets);

                response.Props = new RepeatedField<PropInfo>();

                List<PropItem> propItems = warehouseComponent.UnUseProps;

                foreach (PropItem propItem in propItems)
                {
                    PropInfo tmp = new PropInfo();

                    tmp.TableId = propItem.TableId;

                    tmp.PropState = PropState.WaitUse;

                    tmp.TotalTimes = propItem.TotalTimes;

                    tmp.Num = propItem.Num;

                    response.Props.Add(tmp);
                }
                propItems = warehouseComponent.InUseProps;

                foreach (PropItem propItem in propItems)
                {
                    PropInfo tmp = new PropInfo();

                    tmp.TableId = propItem.TableId;

                    tmp.PropState = PropState.InUser;

                    tmp.TotalTimes = propItem.TotalTimes;

                    tmp.UseTimes = propItem.UseTimes;

                    response.Props.Add(tmp);
                }


                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response,e,reply);
            }

            


        }

    }
}
