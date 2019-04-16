using ETModel;

namespace ETHotfix
{
    public partial class Map
    {
        public static string Get(int id)
        {
            Map message = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Map), id) as Map;
            return message.ChineseMapName;
        }
        

    }
}
