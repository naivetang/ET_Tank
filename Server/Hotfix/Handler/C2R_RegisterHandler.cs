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
                bool canCreate = await this.CheckAccount(message, response);

                if (canCreate)
                {
                    await CreateAccount(message.PhoneNum, message.UserName, message.Password);
                }

                reply(response);
                
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
        /// <summary>
        /// 返回true说明可添加，返回false不可添加
        /// </summary>
        /// <param name="message"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private async ETTask<bool> CheckAccount(C2R_Regist message,R2C_Regist response)
        {
            if (!message.PhoneNum.IsPhoneNum())
            {
                response.ErrorMessagId = 1061;
                return false;
            }

            if (!message.UserName.IsUserName())
            {
                response.ErrorMessagId = 1062;
                return false;
            }
            if (!message.Password.IsPassword())
            {
                response.ErrorMessagId = 1063;
                return false;
            }


            DBProxyComponent db = Game.Scene.GetComponent<DBProxyComponent>();

            List<ComponentWithId> accounts = await db.Query<Account>(t => t.PhoneNum == message.PhoneNum.ToNum());

            if (accounts.Count != 0)
            {
                response.Error = ErrorCode.ERR_RpcFail;

                response.Message = "此账号已注册";

                response.ErrorMessagId = 1066;

                return false;
            }

            accounts = await db.Query<Account>(t => t.UserName == message.UserName);

            if (accounts.Count != 0)
            {
                response.Error = ErrorCode.ERR_RpcFail;

                response.Message = "此账号已注册";

                response.ErrorMessagId = 1066;

                return false;
            }

            return true;
        }

        private async ETTask CreateAccount(string PhoneNum, string UserName, string Password)
        {
            DBProxyComponent db = Game.Scene.GetComponent<DBProxyComponent>();

            Account account = ComponentFactory.Create<Account>();

            account.PhoneNum = PhoneNum.ToNum();

            account.UserName = UserName;

            account.Password = Password;

            await db.Save(account);

            UserDB userDb = ComponentFactory.Create<UserDB>();

            userDb.PhoneNum = PhoneNum.ToNum();

            userDb.AddComponent<UserBaseComponent>().UserName = UserName;

            userDb.AddComponent<SettingInfoComponent>();

            await db.Save(userDb);

        }
    }
}
