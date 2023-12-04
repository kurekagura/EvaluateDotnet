using SkiaSharp;

namespace EvalSkiaSharp;

internal class Program
{
    static async Task Main(string[] args)
    {
        var outdir = Path.Combine(Directory.GetCurrentDirectory(), "output");
        Directory.CreateDirectory(outdir);

        string path = @"..\..\data\img\12_sweets_tomato_ame.png";
        path = Path.GetFullPath(path);

        //縮小
        using (var inStream = File.OpenRead(path))
        using (var ms = OpenScale(inStream, 0.5f))
        {
            string path_out = Path.Combine(outdir, Path.GetFileNameWithoutExtension(path) + "_Scale" + Path.GetExtension(path));
            using (var outFileSream = File.OpenWrite(path_out))
            {
                await ms.CopyToAsync(outFileSream);
                await ms.FlushAsync();
                await outFileSream.FlushAsync();
            }
        }

        //グレースケール
        using (var inStream = File.OpenRead(path))
        using (var ms = OpenGrayscale(inStream))
        {
            string path_out = Path.Combine(outdir, Path.GetFileNameWithoutExtension(path) + "_Grayscale" + Path.GetExtension(path));
            using (var outFileSream = File.OpenWrite(path_out))
            {
                await ms.CopyToAsync(outFileSream);
                await ms.FlushAsync();
                await outFileSream.FlushAsync();
            }
        }
    }

    public static Stream OpenScale(Stream stream, float scale)
    {
        // SkiaSharpのSKBitmapを使用して画像データを読み込む
        using (var originalBitmap = SKBitmap.Decode(stream))
        {
            // 画像を指定されたスケールで拡大縮小する
            using (var scaledBitmap = ScaleBitmap(originalBitmap, scale))
            {
                // 変換後の画像をメモリストリームに書き込む
                var ms = new MemoryStream();
                scaledBitmap.Encode(ms, SKEncodedImageFormat.Png, 100);
                ms.Position = 0;

                return ms;
            }
        }
    }

    private static SKBitmap ScaleBitmap(SKBitmap originalBitmap, float scale)
    {
        // 新しいSKBitmapを作成して、指定されたスケールで拡大縮小する
        var scaledBitmap = new SKBitmap((int)(originalBitmap.Width * scale), (int)(originalBitmap.Height * scale));

        using (var canvas = new SKCanvas(scaledBitmap))
        using (var paint = new SKPaint())
        {
            // スケールを設定
            canvas.Scale(scale, scale);

            // 拡大縮小した画像をcanvasに描画
            canvas.DrawBitmap(originalBitmap, 0, 0, paint);
        }

        return scaledBitmap;
    }

    public static Stream OpenGrayscale(Stream stream)
    {
        using (var originalBitmap = SKBitmap.Decode(stream))
        {
            // グレースケールに変換する
            using (var grayscaleBitmap = ConvertToGrayscale(originalBitmap))
            {
                // 変換後の画像をメモリストリームに書き込む
                var ms = new MemoryStream();
                grayscaleBitmap.Encode(ms, SKEncodedImageFormat.Png, 100);
                ms.Position = 0;
                return ms;
            }
        }
    }

    private static SKBitmap ConvertToGrayscale(SKBitmap originalBitmap)
    {
        float[] values = new float[]
           {
                0.2125f, 0.7154f, 0.0721f, 0, 0,
                0.2125f, 0.7154f, 0.0721f, 0, 0,
                0.2125f, 0.7154f, 0.0721f, 0, 0,
                0, 0, 0, 1, 0
           };

        // 新しいSKBitmapを作成して、グレースケールに変換する
        var grayscaleBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);

        using (var canvas = new SKCanvas(grayscaleBitmap))
        using (var paint = new SKPaint())
        {
            // グレースケールに変換するためのフィルタを設定
            var colorFilter = SKImageFilter.CreateColorFilter(SKColorFilter.CreateColorMatrix(values));

            // ペイントにフィルタを設定
            paint.ImageFilter = colorFilter;

            // グレースケールに変換してcanvasに描画
            canvas.DrawBitmap(originalBitmap, 0, 0, paint);
        }

        return grayscaleBitmap;
    }
}
