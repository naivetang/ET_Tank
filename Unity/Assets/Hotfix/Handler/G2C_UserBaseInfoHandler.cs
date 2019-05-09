using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_UserBaseInfoHandler : AMHandler<G2C_UserBaseInfo>
    {

        protected override void Run(ETModel.Session session, G2C_UserBaseInfo message)
        {
            Player player = PlayerComponent.Instance.MyPlayer;

            player.Name = message.Name;

            player.Level = message.Level;

            player.Experience = message.Experience;

            player.DbID = message.UserDBID;

            player.Gold = message.Gold;
        }
    }
}
