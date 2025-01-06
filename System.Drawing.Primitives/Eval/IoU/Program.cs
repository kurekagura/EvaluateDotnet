using System.Drawing;

namespace IoU;

internal class Program
{
    static void Main(string[] args)
    {
        var rect1 = new RectangleF(1.1f, 2.2f, 3.3f, 4.4f);
        var rect2 = new RectangleF(1.2f, 2.3f, 3.4f, 4.5f);

        float iou = CalculateIoU(rect1, rect2);

        Console.WriteLine($"IoU: {iou}");

        Console.WriteLine("Press any key to exit..");
        Console.ReadKey();
    }

    static float CalculateIoU(RectangleF rect1, RectangleF rect2)
    {
        // Intersectionを計算
        float xOverlap = Math.Max(0, Math.Min(rect1.Right, rect2.Right) - Math.Max(rect1.Left, rect2.Left));
        float yOverlap = Math.Max(0, Math.Min(rect1.Bottom, rect2.Bottom) - Math.Max(rect1.Top, rect2.Top));
        float intersection = xOverlap * yOverlap;

        // 各矩形の面積を計算
        float area1 = rect1.Width * rect1.Height;
        float area2 = rect2.Width * rect2.Height;

        // Unionを計算
        float union = area1 + area2 - intersection;

        // IoUを計算（Unionが0の場合は0を返す）
        return union == 0 ? 0 : intersection / union;
    }
}
