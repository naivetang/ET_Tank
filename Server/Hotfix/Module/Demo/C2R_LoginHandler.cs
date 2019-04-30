using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ETModel;
using NLog.Targets;

namespace ETHotfix
{
	[MessageHandler(AppType.Realm)]
	public class C2R_LoginHandler : AMRpcHandler<C2R_Login, R2C_Login>
	{
		protected override void Run(Session session, C2R_Login message, Action<R2C_Login> reply)
		{
			RunAsync(session, message, reply).NoAwait();
		}

		private async ETVoid RunAsync(Session session, C2R_Login message, Action<R2C_Login> reply)
		{
			R2C_Login response = new R2C_Login();
			try
			{
			    Account ret = await CheckAccount(message, response);

			    if (ret == null)
			    {
			        reply(response);

                    return;
			    }

				// 随机分配一个Gate
				StartConfig config = Game.Scene.GetComponent<RealmGateAddressComponent>().GetAddress();
				Log.Debug($"gate address: {MongoHelper.ToJson(config)}");
				IPEndPoint innerAddress = config.GetComponent<InnerConfig>().IPEndPoint;
				Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(innerAddress);

                // 向gate请求一个key,客户端可以拿着这个key连接gate
                G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await gateSession.Call(new R2G_GetLoginKey() {PhoneNum = ret.PhoneNum});

				string outerAddress = config.GetComponent<OuterConfig>().Address2;

				response.Address = outerAddress;
				response.Key = g2RGetLoginKey.Key;
				reply(response);
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}

	    private async Task<Account> CheckAccount(C2R_Login message,R2C_Login response)
	    {
	        DBProxyComponent db = Game.Scene.GetComponent<DBProxyComponent>();

            List<ComponentWithId> accounts;

            if (message.Account.IsPhoneNum())
            {
                accounts = await db.Query<Account>(account => account.PhoneNum == message.Account.ToNum());
            }
            else if (message.Account.IsUserName())
            {
                accounts = await db.Query<Account>(account => account.UserName == message.Account);
            }
            else
            {
                response.Error = ErrorCode.ERR_RpcFail;

                response.ErrorMessageId = 1064;

                return null;
            }

	        
	        //var accounts = await db.Query<Account>($"{{\'Name\':\'{message.Account}\'}}");

	        if (accounts.Count == 0)
	        {
	            response.Error = ErrorCode.ERR_RpcFail;

	            response.Message = "不存在此账号";

                response.ErrorMessageId = 1064;

                return null;
	        }
            else
	        {
	            Account a = accounts[0] as Account;
	            if (a.Password == message.Password)
	            {
	                return a;
	            }
	            else
	            {
	                response.Error = ErrorCode.ERR_PasswordError;

	                response.Message = "密码错误";

                    response.ErrorMessageId = 1065;

                    return null;
                }
	        }
	    }

	}
}