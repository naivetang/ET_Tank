

using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_SettingInfoHandler : AMHandler<G2C_SettingInfo>
    {
        protected override void Run(ETModel.Session session, G2C_SettingInfo message)
        {
            GameSettingsViewComponent.Data = message;
        }
    }
}
