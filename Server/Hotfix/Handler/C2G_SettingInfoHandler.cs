using System;
using System.Net;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_SettingInfoHandler : AMHandler<C2G_SettingInfo>
    {
        protected override void Run(Session session, C2G_SettingInfo message)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;

            SettingInfoComponent settingInfo = player.UserDB.GetComponent<SettingInfoComponent>();

            settingInfo.Language = message.Language;

            settingInfo.RotSpeed = message.RotSpeed;

            settingInfo.Volume = message.Volume;

            settingInfo.BinarySwitch = message.BinarySwitch;
        }
    }
}
