using ETModel;
using PF;
using Vector3 = UnityEngine.Vector3;

namespace ETHotfix
{
    [MessageHandler]
    class B2C_AttackTankHandler : AMHandler<B2C_AttackTank>
    {
        protected override void Run(ETModel.Session session, B2C_AttackTank message)
        {
            
            Tank sourceTank = ETModel.Game.Scene.GetComponent<TankComponent>().Get(message.SourceTankId);

            Tank beAttackTank = ETModel.Game.Scene.GetComponent<TankComponent>().Get(message.TargetTankId);

            beAttackTank.GetComponent<NumericComponent>().Set(NumericType.HpBase, message.CurrentHp);
            //beAttackTank.BeAttacked(sourceTank, message.Damage);
        }
    }
}
