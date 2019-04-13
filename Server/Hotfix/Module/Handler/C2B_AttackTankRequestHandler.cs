using System;
using ETModel;
using PF;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Battle)]
    class C2B_AttackTankRequestHandler:AMActorLocationRpcHandler<Tank,C2B_AttackTankRequest,B2C_AttackTankResponse>
    {
        // protected override void Run(Tank entity, C2B_AttackTankRequest message)
        // {
        //     if (entity.Id != message.SourceTankId)
        //     {
        //         Log.Debug("客户端发送炮弹有误");
        //         return;
        //     }
        //
        //     B2C_AttackTankResponse sc = new B2C_AttackTankResponse();
        //
        //     Tank tank = Game.Scene.GetComponent<TankComponent>().Get(message.TargetTankId);
        //
        //     tank.GetComponent<NumericComponent>().Change(NumericType.HpBase, -message.Damage);
        //
        //     sc.SourceTankId = entity.Id;
        //
        //     sc.TargetTankId = message.TargetTankId;
        //
        //     sc.Damage = message.Damage;
        //     
        //     entity.BroadcastExceptSelf(sc);
        // }

        protected override async ETTask Run(Tank entity, C2B_AttackTankRequest message, Action<B2C_AttackTankResponse> reply)
        {
            try
            {
                if (entity.Id != message.SourceTankId)
                {
                    Log.Debug("客户端发送炮弹有误");
                    return;
                }

                Tank targetTank = Game.Scene.GetComponent<TankComponent>().Get(message.TargetTankId);

                if (entity.TankCamp == targetTank.TankCamp)
                    return;

                B2C_AttackTankResponse response = new B2C_AttackTankResponse();

                NumericComponent numericComponent = targetTank.GetComponent<NumericComponent>();

                int damage = numericComponent[NumericType.HpBase] < message.Damage ? numericComponent[NumericType.HpBase] : message.Damage;

                int currHp = targetTank.GetComponent<NumericComponent>().Change(NumericType.HpBase, -damage);

                response.SourceTankId = entity.Id;

                response.TargetTankId = message.TargetTankId;

                response.CurrentHp = currHp;

                reply(response);

                await ETTask.CompletedTask;

                Send_B2CAttackTank(entity, response.SourceTankId, response.TargetTankId, response.CurrentHp);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            //entity.BroadcastExceptSelf(response);
        }

        private void Send_B2CAttackTank(Tank entity, long sourceId, long targetId, int currHp)
        {
            B2C_AttackTank attackTank = new B2C_AttackTank();
            
            attackTank.SourceTankId = sourceId;

            attackTank.TargetTankId = targetId;

            attackTank.CurrentHp = currHp;

            entity.BroadcastExceptSelf(attackTank);
        }
    }
}
