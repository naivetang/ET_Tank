
using System;
using PF;

namespace ETModel
{
    public static partial class  ConfigHelper
    {
        public static Vector3 String2Vector3(string source)
        {
            string str = source;

            str = str.Replace('(', ' ');
            str = str.Replace(')', ' ');

            str = str.Trim(' ');

            string[] strs = str.Split(',');

            if (strs.Length != 3)
            {
                Log.Error($"字符串\"{source}\"不能转换为Vector3");
            }

            try
            {
                Vector3 vector3 = new Vector3(Convert.ToSingle(strs[0]), Convert.ToSingle(strs[1]), Convert.ToSingle(strs[2]));

                return vector3;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return Vector3.zero;
            }
        }
    }
}
