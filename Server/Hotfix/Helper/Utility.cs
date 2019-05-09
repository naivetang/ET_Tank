
using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    public static class Utility
    {

        public static void List2RepeatedField<T>(List<T> source, RepeatedField<T> target)
        {
            target.Clear();
            foreach (T t in source)
            {
                target.Add(t);
            }
        }

        public static void RepeatedField2List<T>(RepeatedField<T> source, List<T> target)
        {
            target.Clear();
            foreach (T t in source)
            {
                target.Add(t);
            }
        }
    }
}
