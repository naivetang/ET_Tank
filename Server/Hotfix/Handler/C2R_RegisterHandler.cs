using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ETModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ETHotfix
{
    [MessageHandler(AppType.AllServer)]
    class C2R_RegisterHandler : AMRpcHandler<C2R_Regist, R2C_Regist>
    {
        protected override void Run(Session session, C2R_Regist message, Action<R2C_Regist> reply)
        {
            RunAsync(session, message, reply).NoAwait();
        }

        private async ETVoid RunAsync(Session session, C2R_Regist message, Action<R2C_Regist> reply)
        {
            R2C_Regist response = new R2C_Regist();
            try
            {
                bool ret = await this.hasAccount(message, response);

                if (!ret)
                {
                    await CreateAccount(message.Account, message.Password);
                }

                reply(response);
                
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }

        private async ETTask<bool> hasAccount(C2R_Regist message,R2C_Regist responce)
        {
            DBProxyComponent db = Game.Scene.GetComponent<DBProxyComponent>();

            List<ComponentWithId> accounts = await db.Query<Account>(t => t.Name == message.Account);

            if (accounts.Count >= 1)
            {
                responce.Error = ErrorCode.ERR_RpcFail;

                responce.Message = "此账号已注册";

                return true;
            }

            return false;
        }

        private async ETTask CreateAccount(string Name, string Password)
        {
            DBProxyComponent db = Game.Scene.GetComponent<DBProxyComponent>();

            Account account = ComponentFactory.Create<Account>();

            account.Name = Name;

            account.Password = Password;

            await db.Save(account);

            UserDB userDb = ComponentFactory.Create<UserDB>();

            userDb.Name = Name;

            await db.Save(userDb);

        }
    }
}
