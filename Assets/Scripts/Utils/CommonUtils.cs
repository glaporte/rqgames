using UnityEngine;

namespace rqgames.Utils
{
    public class CommonUtils
    {
        public static int GetRandomWeightedIndex(params float[] values)
        {
            if (values == null || values.Length == 0) return -1;

            float total = 0;
            for (int i = 0; i < values.Length; i++)
                total += values[i];

            float r = Random.value;
            float s = 0f;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == 0)
                    continue;
                s += values[i] / total;
                if (s >= r) return i;
            }

            return -1;
        }
    }
}