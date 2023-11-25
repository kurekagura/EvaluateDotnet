using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using TorchSharp;
using TorchSharp.Modules;
using static TorchSharp.torch.distributions.transforms;
using static TorchSharp.torchvision.io;

namespace EvalTorchVision;

internal class Program
{
    static async Task Main(string[] args)
    {
        //from torchvision import transforms
        //from PIL import Image
        //image_path = 'path/to/your/image.jpg'
        //image = Image.open(image_path)
        //# 画像をグレースケールに変換する変換を定義
        //transform = transforms.Compose([
        //    transforms.Grayscale(),  # ここでグレースケールに変換
        //    transforms.ToTensor()    # PyTorchのテンソルに変換
        //])
        //# 変換を適用
        //gray_image = transform(image)

        var outdir = Path.Combine(Directory.GetCurrentDirectory(), "output");
        Directory.CreateDirectory(outdir);

        string path = @"..\..\data\img\1_vegetable_trevise.png";
        path = Path.GetFullPath(path);

        //グレースケール
        using (var inStream = File.OpenRead(path))
        using (var ms = await OpenGrayscaleAsync(inStream))
        {
            string path_out = Path.Combine(outdir, Path.GetFileNameWithoutExtension(path) + "_Grayscale" + Path.GetExtension(path));
            using (var outFileSream = File.OpenWrite(path_out))
            {
                await ms.CopyToAsync(outFileSream);
                await ms.FlushAsync();
                await outFileSream.FlushAsync();
            }
        }

        //HorizontalFlip
        using (var inStream = File.OpenRead(path))
        using (var ms = await OpenHorizontalFlipAsync(inStream))
        {
            string path_out = Path.Combine(outdir, Path.GetFileNameWithoutExtension(path) + "_HorizontalFlip" + Path.GetExtension(path));
            using (var outFileSream = File.OpenWrite(path_out))
            {
                await ms.CopyToAsync(outFileSream);
                await ms.FlushAsync();
                await outFileSream.FlushAsync();
            }
        }

        //VerticalFlip
        using (var inStream = File.OpenRead(path))
        using (var ms = await OpenVerticalFlipAsync(inStream))
        {
            string path_out = Path.Combine(outdir, Path.GetFileNameWithoutExtension(path) + "_VerticalFlip" + Path.GetExtension(path));
            using (var outFileSream = File.OpenWrite(path_out))
            {
                await ms.CopyToAsync(outFileSream);
                await ms.FlushAsync();
                await outFileSream.FlushAsync();
            }
        }

        //Rotate
        using (var inStream = File.OpenRead(path))
        using (var ms = await OpenRotateAsync(inStream, 90.0f, torch.InterpolationMode.Linear))
        {
            string path_out = Path.Combine(outdir, Path.GetFileNameWithoutExtension(path) + "_Rotate" + Path.GetExtension(path));
            using (var outFileSream = File.OpenWrite(path_out))
            {
                await ms.CopyToAsync(outFileSream);
                await ms.FlushAsync();
                await outFileSream.FlushAsync();
            }
        }

    }

    //TODO:オプション引数有り
    public static async Task<Stream> OpenRotateAsync(Stream stream, float angle, torch.InterpolationMode interpolationMode)
    {
        torch.Tensor img = await torchvision.io.read_image_async(stream, torchvision.io.ImageReadMode.RGB, new SkiaImager());
        torch.Tensor img_out = torchvision.transforms.Compose(
                    torchvision.transforms.Rotate(angle, interpolationMode),
                    torchvision.transforms.ConvertImageDtype(torch.ScalarType.Byte)
                    ).call(img);

        var ms = new MemoryStream();
        await torchvision.io.write_image_async(img_out, ms, torchvision.ImageFormat.Png, imager: new SkiaImager()); ;
        await ms.FlushAsync();
        ms.Position = 0;
        return ms;
    }

    public static async Task<Stream> OpenVerticalFlipAsync(Stream stream)
    {
        torch.Tensor img = await torchvision.io.read_image_async(stream, torchvision.io.ImageReadMode.RGB, new SkiaImager());
        torch.Tensor img_out = torchvision.transforms.Compose(
                    torchvision.transforms.VerticalFlip(),
                    torchvision.transforms.ConvertImageDtype(torch.ScalarType.Byte)
                    ).call(img);

        var ms = new MemoryStream();
        await torchvision.io.write_image_async(img_out, ms, torchvision.ImageFormat.Png, imager: new SkiaImager()); ;
        await ms.FlushAsync();
        ms.Position = 0;
        return ms;
    }

    public static async Task<Stream> OpenHorizontalFlipAsync(Stream stream)
    {
        torch.Tensor img = await torchvision.io.read_image_async(stream, torchvision.io.ImageReadMode.RGB, new SkiaImager());
        torch.Tensor img_out = torchvision.transforms.Compose(
                    torchvision.transforms.HorizontalFlip(),
                    torchvision.transforms.ConvertImageDtype(torch.ScalarType.Byte)
                    ).call(img);

        var ms = new MemoryStream();
        await torchvision.io.write_image_async(img_out, ms, torchvision.ImageFormat.Png, imager: new SkiaImager()); ;
        await ms.FlushAsync();
        ms.Position = 0;
        return ms;
    }

    public static async Task<Stream> OpenBinarizeAsync(Stream stream, byte threshold)
    {
        return await OpenLambdaAsync(stream, t => binarize(t, threshold));
    }

    //def binarize(image):
    //    threshold = 128  # 任意の閾値を設定
    //    return image.point(lambda p: p > threshold and 255)
    public static torch.Tensor binarize(torch.Tensor t, byte threshold)
    {
        //TODO:実装
        return t.clone();
    }

    public static async Task<Stream> OpenLambdaAsync(Stream stream, Func<torch.Tensor, torch.Tensor> lamda)
    {
        torch.Tensor img = await torchvision.io.read_image_async(stream, torchvision.io.ImageReadMode.RGB, new SkiaImager());
        torch.Tensor img_out = torchvision.transforms.Compose(
                    torchvision.transforms.Lambda(lamda),
                    torchvision.transforms.ConvertImageDtype(torch.ScalarType.Byte)
                    ).call(img);

        var ms = new MemoryStream();
        await torchvision.io.write_image_async(img_out, ms, torchvision.ImageFormat.Png, imager: new SkiaImager()); ;
        await ms.FlushAsync();
        ms.Position = 0;
        return ms;
    }

    public static async Task<Stream> OpenGrayscaleAsync(Stream stream)
    {
        torch.Tensor img = await torchvision.io.read_image_async(stream, torchvision.io.ImageReadMode.RGB, new SkiaImager());
        torch.Tensor img_out = torchvision.transforms.Compose(
                    torchvision.transforms.Grayscale(),
                    torchvision.transforms.ConvertImageDtype(torch.ScalarType.Byte)
                    ).call(img);

        var ms = new MemoryStream();
        await torchvision.io.write_image_async(img_out, ms, torchvision.ImageFormat.Png, imager: new SkiaImager()); ;
        await ms.FlushAsync();
        ms.Position = 0;
        return ms;
    }

    //public static async Task<torch.Tensor> GrayscaleAsync(Stream stream)
    //{
    //    torch.Tensor img = await torchvision.io.read_image_async(stream, torchvision.io.ImageReadMode.RGB, new SkiaImager());
    //    torch.Tensor img_out = torchvision.transforms.Compose(
    //                torchvision.transforms.Grayscale(),
    //                torchvision.transforms.ConvertImageDtype(torch.ScalarType.Byte)
    //                ).call(img);

    //    return img_out;
    //}

    //public static async Task<torch.Tensor> GrayscaleAsync(string path)
    //{
    //    torch.Tensor img = await torchvision.io.read_image_async(path, torchvision.io.ImageReadMode.RGB, new SkiaImager());
    //    torch.Tensor img_out = torchvision.transforms.Compose(
    //                torchvision.transforms.Grayscale(),
    //                torchvision.transforms.ConvertImageDtype(torch.ScalarType.Byte)
    //                ).call(img);

    //    return img_out;
    //}
}
