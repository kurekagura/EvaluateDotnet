using OpenCvSharp;

namespace EvalOpenCvSharp;

internal class Program
{
    static async Task Main(string[] args)
    {
        var outdir = Path.Combine(Directory.GetCurrentDirectory(), "output");
        Directory.CreateDirectory(outdir);

        //string path = @"..\..\data\img\12_sweets_tomato_ame.png";
        string path = @"..\..\data\img\1_vegetable_trevise.bmp";

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
    }

    public static Stream OpenScale(Stream stream, float scale)
    {
        using (Mat mat = Mat.FromStream(stream, ImreadModes.Unchanged))
        using (var resizedMat = new Mat())
        {
            // 画像を指定されたスケールで拡大縮小する
            Cv2.Resize(mat, resizedMat, new Size(0, 0), scale, scale);
            // 変換後の画像をメモリストリームmsに書き込む
            byte[] resizedBytes = resizedMat.ImEncode();
            var ms = new MemoryStream(resizedBytes);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }
    }

}
