using ETModel;

namespace ETHotfix
{
    public partial class Prop
    {
        public string Name()
        {
            if (GameSettingsViewComponent.GetLanguage() == Language.Chinese)
            {
                return this.Chinese;
            }
            else
            {
                return this.English;
            }
        }
    }
}
