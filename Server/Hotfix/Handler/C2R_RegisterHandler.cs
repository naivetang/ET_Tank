using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ETModel;

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
                //if (message.Account != "abcdef" || message.Password != "111111")
                //{
                //	response.Error = ErrorCode.ERR_AccountOrPasswordError;
                //	reply(response);
                //	return;
                //}
                bool ret = await this.CreateAccount(message, response);

                reply(response);
                
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }

        private async Task<bool> CreateAccount(C2R_Regist message,R2C_Regist responce)
        {
            DBProxyComponent db = Game.Scene.GetComponent<DBProxyComponent>();

            List<ComponentWithId> accounts = await db.Query<Account>(t => t.UserName == message.Account);

            if (accounts.Count >= 1)
            {
                responce.Error = ErrorCode.ERR_RpcFail;

                responce.Message = "此账号已注册";

                return false;
            }

            Account account = ComponentFactory.Create<Account>();

            account.UserName = message.Account;

            account.Password = message.Password;

            await db.Save(account);

            responce.Error = ErrorCode.ERR_Success;

            return true;
        }
    }
}
