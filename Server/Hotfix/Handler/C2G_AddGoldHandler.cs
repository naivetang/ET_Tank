using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_AddGoldHandler:AMHandler<C2G_AddGold>
    {
        protected override void Run(Session session, C2G_AddGold message)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;

            player.UserDB.GetComponent<UserBaseComponent>().Gold += message.Add;

            Send_G2C_Gold(player);
        }

        private void Send_G2C_Gold(Player player)
        {
            G2C_Gold msg = new G2C_Gold();

            msg.Gold = player.UserDB.GetComponent<UserBaseComponent>().Gold;

            player.Session.Send(msg);
        }
    }
}
