using ETModel;

namespace ETHotfix
{

    public partial class Message
    {
        public static string Get(int id)
        {
            Message message = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Message), id) as Message;
            return message.Chinese;
        }
    }
}
