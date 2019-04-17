using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_StartGameHandler : AMHandler<G2C_StartGame>
    {
        protected override void Run(ETModel.Session session, G2C_StartGame message)
        {
            RunAsync(session,message).NoAwait();
        }

        protected async ETVoid RunAsync(ETModel.Session session, G2C_StartGame message)
        {
            C2G_EnterBattle msg = new C2G_EnterBattle();

            msg.BattleId = message.RoomId;

            G2C_EnterBattle response = (G2C_EnterBattle) await ETModel.SessionComponent.Instance.Session.Call(msg);

            PlayerComponent.Instance.MyPlayer.TankId = response.TankId;

            Log.Warning($"坦克id = {response.TankId}");

            Game.EventSystem.Run(EventIdType.EnterBattle);

            //ETModel.Game.Scene.AddComponent<BattleComponent>();
        }
    }
}
