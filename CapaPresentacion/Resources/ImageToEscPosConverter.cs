using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CapaPresentacion.Resources
{
    public static class ImageToEscPosConverter
    {
        public static byte[] ConvertImageToEscPosBytes(Bitmap bitmap, int maxWidth)
        {
            // Convertir imagen a blanco y negro puro
            int width = Math.Min(maxWidth, bitmap.Width);
            int height = bitmap.Height * width / bitmap.Width;
            Bitmap resized = new Bitmap(bitmap, new Size(width, height));

            // Convert to monochrome (1bpp)
            Bitmap monoBitmap = new Bitmap(resized.Width, resized.Height, PixelFormat.Format1bppIndexed);
            using (Graphics g = Graphics.FromImage(monoBitmap))
            {
                g.Clear(Color.White);
                g.DrawImage(resized, new Rectangle(0, 0, monoBitmap.Width, monoBitmap.Height));
            }

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            // ESC * m nL nH d1...dk
            bw.Write((byte)0x1B);
            bw.Write((byte)'*');
            bw.Write((byte)33); // mode: 24-dot double-density
            bw.Write((byte)(width % 256)); // nL
            bw.Write((byte)(width / 256)); // nH

            int offset = 0;
            while (offset < monoBitmap.Height)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int k = 0; k < 3; k++) // 24 dots per column
                    {
                        byte slice = 0;
                        for (int b = 0; b < 8; b++)
                        {
                            int y = offset + (k * 8) + b;
                            if (y >= monoBitmap.Height)
                                continue;

                            Color pixelColor = monoBitmap.GetPixel(x, y);
                            if (pixelColor.R == 0) // Black pixel
                            {
                                slice |= (byte)(1 << (7 - b));
                            }
                        }
                        bw.Write(slice);
                    }
                }
                offset += 24;
                bw.Write((byte)0x0A); // LF
            }

            return ms.ToArray();
        }
    }
}
