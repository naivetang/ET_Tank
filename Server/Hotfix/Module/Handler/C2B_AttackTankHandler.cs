using System;
using ETModel;
using PF;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Battle)]
    class C2B_AttackTankHandler:AMActorLocationHandler<Tank,C2B_AttackTank>
    {
        protected override void Run(Tank entity, C2B_AttackTank message)
        {
            B2C_AttackTank sc = new B2C_AttackTank();

            Tank tank = Game.Scene.GetComponent<TankComponent>().Get(message.TargetTankId);

            tank.GetComponent<NumericComponent>().Change(NumericType.HpBase, -message.Damage);

            sc.SourceTankId = entity.Id;

            sc.TargetTankId = message.TargetTankId;

            sc.Damage = message.Damage;

            entity.BroadcastExceptSelf(sc);
        }
    }
}
