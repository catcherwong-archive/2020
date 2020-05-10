namespace VCodeTest
{
    using System;
    using System.Linq;
    using SixLabors.Fonts;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.Primitives;

    public class VCodeHelper
    {
        private static readonly Color[] Colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Brown, Color.Purple };
        private static readonly char[] Chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        private static readonly int Width = 90;
        private static readonly int Height = 35;

        public static (string code, byte[] bytes) GenVCode(int num)
        {
            var code = GenCode(num);
            var r = new Random();

            using var image = new Image<Rgba32>(Width, Height);
            var font = SystemFonts.CreateFont(SystemFonts.Families.First().Name, 25, FontStyle.Bold);

            image.Mutate(ctx =>
            {
                ctx.Fill(Color.White);

                for (int i = 0; i < code.Length; i++)
                {
                    ctx.DrawText(code[i].ToString(), font, Colors[r.Next(Colors.Length)], new PointF(20 * i + 10, r.Next(2, 12)));
                }

                for (int i = 0; i < 10; i++)
                {
                    var pen = new Pen(Colors[r.Next(Colors.Length)], 1);
                    var p1 = new PointF(r.Next(Width), r.Next(Height));
                    var p2 = new PointF(r.Next(Width), r.Next(Height));

                    ctx.DrawLines(pen, p1, p2);
                }

                for (int i = 0; i < 80; i++)
                {
                    var pen = new Pen(Colors[r.Next(Colors.Length)], 1);
                    var p1 = new PointF(r.Next(Width), r.Next(Height));
                    var p2 = new PointF(p1.X + 1f, p1.Y + 1f);

                    ctx.DrawLines(pen, p1, p2);
                }
            });

            using var ms = new System.IO.MemoryStream();
            image.SaveAsGif(ms);
            return (code, ms.ToArray());
        }

        private static string GenCode(int num)
        {
            var code = string.Empty;
            var r = new Random();

            for (int i = 0; i < num; i++)
            {
                code += Chars[r.Next(Chars.Length)].ToString();
            }

            return code;
        }
    }
}
