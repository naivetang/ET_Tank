
namespace ETModel
{
    public partial class Message
    {
        public static string Get(Player player, int id)
        {
            Message message = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Message), id) as Message;

            if (message == null)
                return "null";

            Language language = player.UserDB.GetComponent<SettingInfoComponent>().Language;

            if (language == Language.Chinese)
                return message.Chinese;
            else
                return message.English;
        }

    }
}
