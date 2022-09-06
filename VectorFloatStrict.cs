using System;
using System.Runtime.Intrinsics;

namespace Algorithms
{
    sealed class VectorFloatStrictRenderer
    {
        const int max_iters = 1000; // Make this higher to see more detail when zoomed in (and slow down rendering a lot)
        private const float limit = 4.0f;

        public static byte[] RenderMandelbrot(int width, int height,
            float xmin, float xmax, float ymin, float ymax, float step)
        {
            var bytes = new byte[width * height * 4];
            Vector128<float> vmax_iters = Vector128.Create<float>(max_iters);
            Vector128<float> vlimit = Vector128.Create<float>(limit);
            Vector128<float> vstep = Vector128.Create<float>(step);
            Vector128<float> vxmax = Vector128.Create<float>(xmax);
            Vector128<float> vinc = Vector128.Create<float>(Vector128<float>.Count * step);
            Vector128<float> vxmin = VectorHelper.Create(i => xmin + step * i);
            Vector128<float> one = Vector128.Create<float>(1.0f);

            float y = ymin;
            int yp = 0;
            for (Vector128<float> vy = Vector128.Create<float>(ymin); y <= ymax; vy += vstep, y += step, yp++)
            {
                int xp = 0;
                for (Vector128<float> vx = vxmin; Vector128.LessThanOrEqualAll(vx, vxmax); vx += vinc, xp += Vector128<int>.Count)
                {
                    Vector128<float> accumx = vx;
                    Vector128<float> accumy = vy;

                    Vector128<float> viters = Vector128<float>.Zero;
                    Vector128<float> increment = one;
                    do
                    {
                        Vector128<float> naccumx = accumx * accumx - accumy * accumy;
                        Vector128<float> naccumy = accumx * accumy + accumx * accumy;
                        accumx = naccumx + vx;
                        accumy = naccumy + vy;
                        viters += increment;
                        Vector128<float> sqabs = accumx * accumx + accumy * accumy;
                        Vector128<float> vCond = Vector128.LessThanOrEqual<float>(sqabs, vlimit) &
                            Vector128.LessThanOrEqual(viters, vmax_iters);
                        increment = increment & vCond;
                    } while (increment != Vector128<float>.Zero);

                    for (int i = 0; i < Vector128<float>.Count; i++)
                    {
                        int x = xp + i;
                        if (yp >= height || x >= width)
                            break;

                        int iters = (int)viters[i];


                        int pos = 4 * (yp * width + x);
                        int val = 1000 - Math.Min(iters, 1000);

                        byte blue = (byte)(val % 43 * 23);
                        byte red = (byte)(val % 97 * 41);
                        byte green = (byte)(val % 71 * 19);
                        bytes[pos++] = red;
                        bytes[pos++] = green;
                        bytes[pos++] = blue;
                        bytes[pos] = 255;
                    }
                }
            }

            return bytes;
        }
    }
}