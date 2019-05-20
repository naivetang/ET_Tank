using ETModel;
using PF;
using Vector3 = UnityEngine.Vector3;

namespace ETHotfix
{
    [MessageHandler]
    class B2C_ShootHandler : AMHandler<B2C_Shoot>
    {
        protected override void Run(ETModel.Session session, B2C_Shoot message)
        {
            long tankId = message.TankId;

            if( PlayerComponent.Instance.MyPlayer.TankId == tankId)
                return;

            Tank tank = ETModel.Game.Scene.GetComponent<TankComponent>().Get(tankId);

            TankShootComponent tankShoot = tank.GetComponent<TankShootComponent>();

            tankShoot.NetShoot();

        }
    }
}
