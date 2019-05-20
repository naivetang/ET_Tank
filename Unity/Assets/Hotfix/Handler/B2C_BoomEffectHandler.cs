using ETModel;
using PF;
using Vector3 = UnityEngine.Vector3;

namespace ETHotfix
{
    [MessageHandler]
    class B2C_BoomEffectHandler:AMHandler<B2C_BoomEffect>
    {
        protected override void Run(ETModel.Session session, B2C_BoomEffect message)
        {
            Vector3 vector3 = new Vector3(message.PX * 1.0f / Tank.m_coefficient, message.PY * 1.0f / Tank.m_coefficient,
                    message.PZ * 1.0f / Tank.m_coefficient);

            TankShootComponent.CreateBoomEffect(vector3);
        }
    }
}
