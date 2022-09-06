using System.Runtime.InteropServices.JavaScript;
using System;
using Algorithms;
using System.Threading.Tasks;

public partial class MainJS
{
    private const int cw = 640, ch = 480;

    public static void Main()
    {
        Console.WriteLine("Hello, World!");
    }

    [JSImport("renderCanvas", "main.js")]
    internal static partial void RenderCanvas([JSMarshalAs<JSType.MemoryView>] ArraySegment<byte> rgba);

    [JSExport]
    internal static async Task OnRenderClick()
    {
        var now = DateTime.UtcNow;
        Console.WriteLine("Rendering started");
        await RenderView(-1.248f, -.0362f, .001f);
        Console.WriteLine("Rendering finished in " + (DateTime.UtcNow - now).TotalMilliseconds + " ms");
    }

    [JSExport]
    internal static async Task OnFlyClick()
    {
        var renderPoints = new Tuple<float, float, float>[150];

        // Start point/range
        float xs = -0.5f;
        float ys = 0.0f;
        float rs = 3.0f;

        // End point/range
        float xe = -.2649f;
        float ye = -.8506f;
        float re = 0.00048828125f;

        // Interpolate all the points in between
        float l = 1.0f / (renderPoints.Length - 1);
        for (int i = 0; i < renderPoints.Length; i++)
        {
            float scale = (float)Math.Pow(l * i, 0.03125);
            await RenderView(xs + (xe - xs) * scale, ys + (ye - ys) * scale, rs + (re - rs) * scale);
        }
    }

    private static async Task RenderView(float xc, float yc, float scale)
    {
        // Get the min/max/step values and make sure they're all sensible
        float xmin = (xc - scale / 2.0f).Clamp(-3.0f, 1f);
        float xmax = (xc + scale / 2.0f).Clamp(-3.0f, 1f);
        if (xmin > xmax)
        {
            float t = xmin;
            xmin = xmax;
            xmax = t;
        }
        float ymax = (yc + scale / 2.0f).Clamp(-1.5f, 1.5f);
        float ymin = (yc - scale / 2.0f).Clamp(-1.5f, 1.5f);
        if (ymin > ymax)
        {
            float t = ymin;
            ymin = ymax;
            ymax = t;
        }
        float ystep = (scale / ch).Clamp(0, ymax - ymin);
        float xstep = (scale / cw).Clamp(0, xmax - xmin);
        float step = Math.Max(ystep, xstep);
        xmin = xc - (cw * step / 2);
        xmax = xc + (cw * step / 2);
        ymin = yc - (ch * step / 2);
        ymax = yc + (ch * step / 2);

        var canvasRGBA = VectorFloatStrictRenderer.RenderMandelbrot(cw, ch, (float)xmin, (float)xmax, (float)ymin, (float)ymax, (float)step);
        RenderCanvas(canvasRGBA);
        await Task.Delay(10);
    }
}
