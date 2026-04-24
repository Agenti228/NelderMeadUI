using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.InteropServices;

namespace SimplexUI
{
    internal class TopographicRenderer(Function function, TopographicCamera camera, (int width, int height) pictureBoxDimentions)
    {
        public Function Function { get; set; } = function;
        public TopographicCamera Camera { get; set; } = camera;
        public (int width, int height) PictureBoxDimentions { get; set; } = pictureBoxDimentions;
        public Bitmap Render()
        {
            if (PictureBoxDimentions.width <= 0 || PictureBoxDimentions.height <= 0)
            {
                return new Bitmap(0,0);
            }

            double[] zBuffer = new double[PictureBoxDimentions.width * PictureBoxDimentions.height];
            double minZ = double.MaxValue;
            double maxZ = double.MinValue;

            int index = 0;

            for(int y = 0; y < PictureBoxDimentions.height; y++)
            {
                for (int x = 0; x < PictureBoxDimentions.width; x++)
                {
                    Camera.ScreenToWorld(x, y, out double worldX, out double worldY);
                    double z = Function.Calculate([worldX, worldY]);
                    zBuffer[index] = z;
                    if(z < minZ)
                    {
                        minZ = z;
                    }
                    if(z > maxZ)
                    {
                        maxZ = z;
                    }
                    index++;
                }
            }

            if (maxZ - minZ < 1e-9)
            {
                maxZ = minZ + 1.0;
            }
            double range = maxZ - minZ;

            Bitmap bmp = new Bitmap(PictureBoxDimentions.width, PictureBoxDimentions.height, PixelFormat.Format24bppRgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, PictureBoxDimentions.width, PictureBoxDimentions.height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = bmpData.Stride;
            byte[] pixels = new byte[stride * PictureBoxDimentions.height];

            index = 0;
            for(int y = 0; y < PictureBoxDimentions.height; y++)
            {
                int rowOffset = y * stride;
                for(int x = 0; x < PictureBoxDimentions.width; x++)
                {
                    double t = (zBuffer[index] - minZ) / range;

                    byte red = (byte)((1.0 - t) * 255);
                    byte green = (byte)(t * 255);

                    pixels[rowOffset + x * 3 + 0] = 0;
                    pixels[rowOffset + x * 3 + 1] = green;
                    pixels[rowOffset + x * 3 + 2] = red;
                    index++;
                }
            }

            Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }
    }
}
