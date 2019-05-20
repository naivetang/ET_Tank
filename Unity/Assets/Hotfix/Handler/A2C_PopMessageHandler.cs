using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    class A2C_PopMessageHandler : AMHandler<A2C_PopMessage>
    {
        protected override void Run(ETModel.Session session, A2C_PopMessage message)
        {
            Game.EventSystem.Run(EventIdType.ShowPopMessage, message.Text, message.Type);
        }
    }
}
