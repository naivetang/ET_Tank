using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    public static class AssetBundleName
    {
        public static readonly string Unit = "unit.unity3d";
        public static readonly string Config = "config.unity3d";
        public static readonly string Battle = "battle.unity3d";
    }


    public static class PrefabName
    {
        public static readonly string Unit = "Unit";
        public static readonly string Tank = "Tank";
        public static readonly string Bullet = "Bullet";
        public static readonly string Motor = "motor";
        public static readonly string TankBoomEffect = "Fire_01";
        public static readonly string BulletBoomEffect = "Explosion";
        public static readonly string BulletBoomAudio = "BulletBoom";

    }

    public static class Tag
    {
        public static readonly string Tank = "Tank";
        public static readonly string OwnTank = "OwnTank";
    }

    public static class SceneName
    {
        public static readonly string Init = "Init";

        public static readonly string Battlefield = "Battle";
    }
}
