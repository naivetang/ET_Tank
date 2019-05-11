using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    class G2C_ChatMessageHandler : AMHandler<G2C_ChatMessage>
    {
        protected override void Run(ETModel.Session session, G2C_ChatMessage message)
        {
            if (message.ChatType == ChatType.Hall)
            {
                FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Hall);

                if (fui != null)
                {
                    fui.GetComponent<HallViewComponent>().UpdateChatInfo(message);
                }
            }
            else if (message.ChatType == ChatType.Room)
            {
                FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Room);

                if (fui != null)
                {
                    fui.GetComponent<RoomViewComponent>().UpdateChatInfo(message);
                }
            }
        }
    }
}
