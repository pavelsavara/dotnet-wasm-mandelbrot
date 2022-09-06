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

        // Helper to invoke a function for each element of the vector. This is NOT fast. I just like
        // the way it looks better than a for loop. I expect that if this were provided for the core
        // SIMD API, people might get the wrong idea about performance of this method.
        // i.e. Don't use it somewhere that performance truly matters
        public static void ForEach(this Vector128<float> vec, Action<float, int> op) 
        {
            for (int i = 0; i < Vector<float>.Count; i++)
                op(vec[i], i);
        }
    }
}