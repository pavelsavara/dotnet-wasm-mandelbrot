using System.Runtime.InteropServices.JavaScript;
using System;
using Algorithms;

public partial class MainJS
{

    public static void Main()
    {
        Console.WriteLine("Hello, World!");
    }

    [JSImport("renderCanvas", "main.js")]
    internal static partial void RenderCanvas([JSMarshalAs<JSType.MemoryView>] ArraySegment<byte> rgba);

    [JSExport]
    internal static void OnClick()
    {
        var now = DateTime.UtcNow;
        Console.WriteLine("Rendering started");
        int cw = 640, ch = 480;

        double range, xc, yc;

        xc = -1.248;
        yc = -.0362;
        range = .001;

        double xmin = (xc - range / 2.0).Clamp(-3.0, 1);
        double xmax = (xc + range / 2.0).Clamp(-3.0, 1);
        if (xmin > xmax)
        {
            double t = xmin;
            xmin = xmax;
            xmax = t;
        }
        double ymin = (yc - range / 2.0).Clamp(-1.5f, 1.5f);
        double ymax = (yc + range / 2.0).Clamp(-1.5f, 1.5f);
        if (ymin > ymax)
        {
            double t = ymin;
            ymin = ymax;
            ymax = t;
        }
        double ystep = (range / (double)ch).Clamp(0, ymax - ymin);
        double xstep = (range / (double)cw).Clamp(0, xmax - xmin);
        double step = Math.Max(ystep, xstep);
        xmin = xc - (cw * step / 2);
        xmax = xc + (cw * step / 2);
        ymin = yc - (ch * step / 2);
        ymax = yc + (ch * step / 2);

        if (xmin == xmax || ymin == ymax ||
            xmin + xstep <= xmin || ymin + ystep <= ymin ||
            ymax - ystep >= ymax || xmax - xstep >= xmax)
            return;

        var canvasRGBA = VectorFloatStrictRenderer.RenderMandelbrot(cw, ch, (float)xmin, (float)xmax, (float)ymin, (float)ymax, (float)step);

        Console.WriteLine("Rendering finished in " + (DateTime.UtcNow - now).TotalMilliseconds + " ms");
        RenderCanvas(canvasRGBA);
    }
}

public static class Ext
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
