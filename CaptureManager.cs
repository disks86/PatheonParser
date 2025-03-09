using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesseract;

namespace PatheonParser
{
    public partial class CaptureManager
        : IDisposable
    {
        [GeneratedRegex(@"[{\[]\d+[:;]\d+[:;]\d+[\]}]", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex TimeStampRegex();

        public string TessDataPath { get; set; }
        public TesseractEngine TesseractEngine { get; set; }
        public Tesseract.Rect CombatWindowRect { get; set; }
        public string LastHash { get; set; }
        public CaptureManager()
        {
            TessDataPath = Path.Combine(Environment.CurrentDirectory, "tessdata");
            TesseractEngine = new(TessDataPath, "eng", EngineMode.Default);
            LastHash = string.Empty;
        }

        public void CheckForChanges()
        {
            using var bitmap = Utilities.ScreenCapture();
            //bitmap.Save("c:\\temp\\raw.png");

            if (CombatWindowRect.Height == 0)
            {
                var newHash = Utilities.ComputeImageHash(bitmap);
                if (newHash == LastHash)
                {
                    return;
                }

                LastHash = newHash;

                using var originalImage = PixConverter.ToPix(bitmap);
                //originalImage.Save("c:\\temp\\full-originalImage.png");
                using var grayscaleImage = originalImage.ConvertRGBToGray();
                //grayscaleImage.Save("c:\\temp\\full-grayscaleImage.png");
                using var binarizedImage = grayscaleImage.BinarizeOtsuAdaptiveThreshold(128, 128, 10, 10, 0.1f);
                //binarizedImage.Save("c:\\temp\\full-binarizedImage.png");

                CombatWindowRect = FindCombatWindowRect(binarizedImage);

                LastHash = string.Empty;
            }
            else
            {
                var cropArea = new Rectangle(CombatWindowRect.X1, CombatWindowRect.Y1, CombatWindowRect.Width, CombatWindowRect.Height);
                cropArea = Rectangle.Intersect(cropArea, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                //bitmap.Save("c:\\temp\\bitmap.png");
                using var croppedBitmap = bitmap.Clone(cropArea, bitmap.PixelFormat);
                //croppedBitmap.Save("c:\\temp\\croppedBitmap.png");

                var newHash = Utilities.ComputeImageHash(croppedBitmap);
                if (newHash == LastHash)
                {
                    return;
                }

                LastHash = newHash;

                using var originalImage = PixConverter.ToPix(croppedBitmap);
                //originalImage.Save("c:\\temp\\crop-originalImage.png");
                using var grayscaleImage = originalImage.ConvertRGBToGray();
                //grayscaleImage.Save("c:\\temp\\crop-grayscaleImage.png");
                using var binarizedImage = grayscaleImage.BinarizeOtsuAdaptiveThreshold(128, 128, 10, 10, 0.1f);
                //binarizedImage.Save("c:\\temp\\crop-binarizedImage.png");

                CheckImageForChanges(binarizedImage);
            }
        }

        public void CheckImageForChanges(Pix image)
        {
            using var scaledImage = image.Scale(2.0f, 2.0f);
            using var page = TesseractEngine.Process(scaledImage);
            var text = page.GetText();
            text = text.Replace('\r', ' ');
            text = text.Replace('\n', ' ');
            text = text.Replace("[", "\n[");
            text = text.Replace("mitigatad", "mitigated");
            text = text.Replace("Aftack", "Attack");
            text = text.Replace("spirt's", "spirit's");
            text = text.Replace("(Immuna)", "(immune)");
            text = text.Replace("(Immune)", "(immune)");
            text = text.Replace("vangseful", "vengeful");
            text = text.Replace("  ", " ");
            var lines = text.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (lines is not null)
            {
                File.AppendAllLines("c:\\temp\\log.txt", lines);
            }
        }

        public Tesseract.Rect FindCombatWindowRect(Pix image)
        {
            var minX = image.Width / 2;
            var minY = image.Height / 2;

            using var page = TesseractEngine.Process(image, PageSegMode.SparseText);

            var imageRect = new Tesseract.Rect(0, 0, image.Width, image.Height);
            var currentRect = new Tesseract.Rect(image.Width, image.Height, 0, 0);
            var lastText = string.Empty;

            using var it = page.GetIterator();
            while (it.Next(PageIteratorLevel.TextLine))
            {
                if (it.TryGetBoundingBox(PageIteratorLevel.TextLine, out var rect))
                {
                    if (rect.X1 > minX && rect.Y1 > minY && rect.X1 < currentRect.X1 && rect.Y1 < currentRect.Y1)
                    {
                        lastText = it.GetText(PageIteratorLevel.TextLine);
                        if (TimeStampRegex().IsMatch(lastText))
                        {
                            currentRect = new Tesseract.Rect(rect.X1 - 32, rect.Y1 - 32, (imageRect.X2 - rect.X1) + 32, (imageRect.Y2 - rect.Y1) + 32);
                        }
                    }
                }
            }

            return currentRect;
        }

        public void Dispose()
        {
            if (TesseractEngine is not null)
            {
                TesseractEngine.Dispose();
            }
        }
    }
}
