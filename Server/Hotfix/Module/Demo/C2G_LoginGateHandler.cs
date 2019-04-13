using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Gate)]
	public class C2G_LoginGateHandler : AMRpcHandler<C2G_LoginGate, G2C_LoginGate>
	{
		protected override void Run(Session session, C2G_LoginGate message, Action<G2C_LoginGate> reply)
		{
			RunAsync(session,message,reply).NoAwait();
		}

        private async ETVoid RunAsync(Session session, C2G_LoginGate message, Action<G2C_LoginGate> reply)
        {
            G2C_LoginGate response = new G2C_LoginGate();
            try
            {
                string account = Game.Scene.GetComponent<GateSessionKeyComponent>().Get(message.Key);
                if (account == null)
                {
                    response.Error = ErrorCode.ERR_ConnectGateKeyError;
                    response.Message = "Gate key验证失败!";
                    reply(response);
                    return;
                }

                UserDB userDb = await GetUserDB(account);

                //Player player = ComponentFactory.Create<Player, string>(account);

                Player player = ComponentFactory.Create<Player, UserDB>(userDb);

                Game.Scene.GetComponent<PlayerComponent>().Add(player);

                session.AddComponent<SessionPlayerComponent>().Player = player;

                session.AddComponent<MailBoxComponent, string>(MailboxType.GateSession);

                response.PlayerId = player.Id;

                reply(response);

                session.Send(new G2C_TestHotfixMessage() { Info = "recv hotfix message success" });
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }

        private async Task<UserDB> GetUserDB(string name)
        {
            DBProxyComponent db = Game.Scene.GetComponent<DBProxyComponent>();

            List<ComponentWithId> accounts = await db.Query<UserDB>(account => account.Name == name);
            //var accounts = await db.Query<Account>($"{{\'Name\':\'{message.Account}\'}}");

            if (accounts.Count == 0)
            {
                Log.Error("AccountDB登陆成功，但UserDB不存在数据");

                return null;
            }
            else
            {
                UserDB userDb = accounts[0] as UserDB;
                return userDb;
            }
        }
    }
}