using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;
using Google.Protobuf.Collections;

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
                UInt64 account = Game.Scene.GetComponent<GateSessionKeyComponent>().Get(message.Key);
                if (account == 0)
                {
                    response.Error = ErrorCode.ERR_ConnectGateKeyError;
                    response.Message = "Gate key验证失败!";
                    reply(response);
                    return;
                }

                UserDB userDb = await GetUserDB(account);

                //Player player = ComponentFactory.Create<Player, string>(account);

                Player player = ComponentFactory.Create<Player, Session, UserDB>(session, userDb);

                player.Session = session;

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

        

        private async Task<UserDB> GetUserDB(UInt64 phoneNum)
        {
            DBProxyComponent db = Game.Scene.GetComponent<DBProxyComponent>();

            List<ComponentWithId> accounts = await db.Query<UserDB>(account => account.PhoneNum == phoneNum);
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