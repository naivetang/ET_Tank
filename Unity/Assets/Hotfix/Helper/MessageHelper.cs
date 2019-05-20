using ETModel;

namespace ETHotfix
{

    public partial class Message
    {
        public static string Get(int id)
        {
            Message message = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Message), id) as Message;

            if (message == null)
                return "null";

            Language language = GameSettingsViewComponent.GetLanguage();

            if (language == Language.Chinese)
                return message.Chinese;
            else
                return message.English;
        }
    }
}
