using System;
using System.Numerics;
using System.Runtime.Intrinsics;

namespace Algorithms
{
    public static class VectorHelper
    {
        // Helper to construct a vector from a lambda that takes an index. It's not efficient, but I
        // think it's prettier and more succint than the corresponding for loop.
        // Don't use it on a hot code path (i.e. inside a loop)
        public static Vector128<float> Create(Func<int, float> creator)
        {
            float[] data = new float[Vector<float>.Count];
            for (int i = 0; i < data.Length; i++)
                data[i] = creator(i);

            return Vector128.Create(data);
        }
    }

    public static class ClampExt
    {
        public static double Clamp(this double val, double lo, double hi)
        {
            return Math.Min(Math.Max(val, lo), hi);
        }
        public static float Clamp(this float val, float lo, float hi)
        {
            return Math.Min(Math.Max(val, lo), hi);
        }
    }
}