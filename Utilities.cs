using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using static System.Windows.Forms.DataFormats;

namespace PatheonParser;

public static partial class Utilities
{
    public static string ComputeImageHash(Bitmap bmp)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            bmp.Save(ms, ImageFormat.Png);
            using (SHA256 sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(ms.ToArray()));
            }
        }
    }

    public static Bitmap Texture2DToBitmap(SharpDX.Direct3D11.Device device, Texture2D texture)
    {
        var dataBox = device.ImmediateContext.MapSubresource(texture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);
        var bitmap = new Bitmap(texture.Description.Width, texture.Description.Height, PixelFormat.Format32bppArgb);
        var boundsRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

        var bmpData = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
        for (int y = 0; y < texture.Description.Height; y++)
        {
            IntPtr destPtr = bmpData.Scan0 + y * bmpData.Stride;
            IntPtr srcPtr = dataBox.DataPointer + y * dataBox.RowPitch;
            //System.Runtime.InteropServices.Marshal.Copy(srcPtr, 0, destPtr, bitmap.Width * 4);
            unsafe
            {
                System.Buffer.MemoryCopy((void*)srcPtr, (void*)destPtr, bitmap.Width * 4, bitmap.Width * 4);
            }
        }

        bitmap.UnlockBits(bmpData);
        device.ImmediateContext.UnmapSubresource(texture, 0);
        return bitmap;
    }

    public static Bitmap ScreenCapture()
    {
        using var factory = new Factory1();
        using var adapter = factory.GetAdapter1(0);
        using var device = new SharpDX.Direct3D11.Device(adapter);
        using var output = adapter.GetOutput(0);
        using var output1 = output.QueryInterface<Output1>();

        var textureDesc = new Texture2DDescription
        {
            CpuAccessFlags = CpuAccessFlags.Read,
            BindFlags = BindFlags.None,
            Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
            Width = output.Description.DesktopBounds.Right - output.Description.DesktopBounds.Left,
            Height = output.Description.DesktopBounds.Bottom - output.Description.DesktopBounds.Top,
            OptionFlags = ResourceOptionFlags.None,
            MipLevels = 1,
            ArraySize = 1,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Staging
        };

        using var texture = new Texture2D(device, textureDesc);
        using var duplicate = output1.DuplicateOutput(device);

        Result result;
        do
        {
            result = duplicate.TryAcquireNextFrame(250, out var info, out var resource);
            device.ImmediateContext.CopyResource(resource.QueryInterface<Texture2D>(), texture);
            resource.Dispose();
        }
        while (result!= Result.Ok);

        return Texture2DToBitmap(device, texture);
    }
}