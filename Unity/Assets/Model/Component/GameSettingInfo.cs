namespace ETModel
{
    public static class GameSettingInfo
    {
        public static G2C_SettingInfo Data { get; set; } = null;

        public static bool HpVisible()
        {
            if (Data == null)
                return true;

            return (Data.BinarySwitch & 2) > 0;
        }

        public static bool NameVisible()
        {
            if (Data == null)
                return true;

            return (Data.BinarySwitch & 1) > 0;
        }

        public static float AudioVolume()
        {
            if (Data == null)
                return 0;

            return Data.Volume;
        }
    }
}
