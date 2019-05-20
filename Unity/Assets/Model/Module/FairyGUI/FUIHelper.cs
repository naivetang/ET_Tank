using FairyGUI;

namespace ETModel
{
    public static class FUIHelper
    {
        public static GWindow asGWindow(this GObject gObject)
        {
            return gObject as GWindow;
        }

        public static string GetLevelUrl(int level)
        {
            return $"ui://Common/{level}";
        }

        public static string GetPingUrl(int ping)
        {
            return $"ui://CombatSettlement/{ping}";
        }
    }
}