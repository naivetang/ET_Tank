using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    class G2C_GoldHandler:AMHandler<G2C_Gold>
    {
        protected override void Run(ETModel.Session session, G2C_Gold message)
        {
            PlayerComponent.Instance.MyPlayer.Gold = message.Gold;

            Game.EventSystem.Run(EventIdType.GoldChange);
        }
    }
}
