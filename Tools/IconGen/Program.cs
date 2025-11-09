using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

static class Program
{
    // Generates a multi-size ICO (16,32,48,64,128,256) from a PNG/JPG source
    static int[] Sizes = new[] { 16, 32, 48, 64, 128, 256 };

    static void Main(string[] args)
    {
        var repoRoot = FindRepoRoot();
        var defaultInput = Path.Combine(repoRoot, "MeuAppWinForms", "Resources", "CarvaoChama.png");
        var defaultOutput = Path.Combine(repoRoot, "MeuAppWinForms", "Resources", "AppIcon.ico");

        var input = args.Length > 0 ? args[0] : defaultInput;
        var output = args.Length > 1 ? args[1] : defaultOutput;

        if (!File.Exists(input))
        {
            Console.Error.WriteLine($"Input image not found: {input}");
            Environment.Exit(1);
        }

        Directory.CreateDirectory(Path.GetDirectoryName(output)!);

        using var src = new Bitmap(input);
        using var ico = BuildIcoFromBitmap(src);
        using var fs = new FileStream(output, FileMode.Create, FileAccess.Write);
        ico.CopyTo(fs);
        Console.WriteLine($"ICO generated: {output}");
    }

    static MemoryStream BuildIcoFromBitmap(Bitmap source)
    {
        // Create PNG images for each size
        var images = Sizes.Select(size => new { Size = size, Data = ResizeToPng(source, size, size) }).ToList();

        // ICO header (6 bytes): reserved(2)=0, type(2)=1, count(2)=n
        var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true);
        bw.Write((ushort)0); // reserved
        bw.Write((ushort)1); // type=icon
        bw.Write((ushort)images.Count); // count

        // Prepare directory entries with placeholder offsets
        long dirStart = ms.Position;
        foreach (var img in images)
        {
            WriteIconDirEntry(bw, img.Size, 0, 0); // we will backfill size and offset
        }

        // Write image data and collect sizes/offsets
        long[] offsets = new long[images.Count];
        int[] lengths = new int[images.Count];
        for (int i = 0; i < images.Count; i++)
        {
            offsets[i] = ms.Position;
            ms.Write(images[i].Data, 0, images[i].Data.Length);
            lengths[i] = images[i].Data.Length;
        }

        // Backfill entries
        ms.Position = dirStart;
        for (int i = 0; i < images.Count; i++)
        {
            WriteIconDirEntry(bw, images[i].Size, lengths[i], (int)offsets[i]);
        }

        ms.Position = 0;
        return ms;
    }

    static void WriteIconDirEntry(BinaryWriter bw, int size, int bytesInRes, int imageOffset)
    {
        byte width = (byte)(size == 256 ? 0 : size);
        byte height = (byte)(size == 256 ? 0 : size);
        bw.Write(width); // bWidth
        bw.Write(height); // bHeight
        bw.Write((byte)0); // bColorCount
        bw.Write((byte)0); // bReserved
        bw.Write((ushort)1); // wPlanes
        bw.Write((ushort)32); // wBitCount
        bw.Write(bytesInRes); // dwBytesInRes
        bw.Write(imageOffset); // dwImageOffset
    }

    static byte[] ResizeToPng(Bitmap src, int width, int height)
    {
        using var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(src, new Rectangle(0, 0, width, height), new Rectangle(0, 0, src.Width, src.Height), GraphicsUnit.Pixel);
        }
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }

    static string FindRepoRoot()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir, "CarvaoControl.sln"))) return dir;
            dir = Directory.GetParent(dir)?.FullName!;
        }
        return Directory.GetCurrentDirectory();
    }
}
