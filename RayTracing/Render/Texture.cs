using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayTracing.Render.Components;
using RayTracing.Render.Mathematics;

namespace RayTracing.Render.Materials
{
    public abstract class Texture
    {
        public abstract SColor value(double u, double v, Vector3D p);
    }

    public class ConstantTexture : Texture
    {
        private SColor clr;
        public ConstantTexture(SColor c)
        {
            clr = c;
        }
        public override SColor value(double u, double v, Vector3D p)=>clr;
    }

    public class CheckerTexture : Texture
    {
        public Texture odd, even;

        public CheckerTexture(Texture t0, Texture t1)
        {
            even = t0;
            odd = t1;
        }
        public override SColor value(double u, double v, Vector3D p)=>Math.Sin(10 * p.X) * Math.Sin(10 * p.Y) * Math.Sin(10 * p.Z) <= 0?odd.value(u,v,p):even.value(u,v,p);
    }

    public class ImageTexture : Texture
    {
        private byte[] data;
        private int w, h;
        private double scale = 1;

        //构造函数直接读取图片
        public ImageTexture(string file,double s=1)
        {
            scale = s;
            Bitmap bmp = new Bitmap(Image.FromFile(file));
            data=new byte[bmp.Width*bmp.Height*3];
            w = bmp.Width;
            h = bmp.Height;
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color c = bmp.GetPixel(j, i);
                    data[3 * j + 3 * w * i] = c.R;
                    data[3 * j + 3 * w * i+1] = c.G;
                    data[3 * j + 3 * w * i+2] = c.B;
                }
            }
        }

        //构造函数赋予RGB缓冲
        public ImageTexture(byte[] p, int x, int y)
        {
            data = p;
            w = x;
            h = y;
        }

        //取得某UV的颜色值。
        public override SColor value(double u, double v, Vector3D p)
        {
            u = u * scale % 1;
            v = v * scale % 1;

            int i = Mathf.Range((int) (u * w), 0, w - 1);
            int j = Mathf.Range((int) ((1 - v) * h - 0.001), 0, h - 1);

            return new SColor(
                data[3 * i + 3 * w * j] / 255f, 
                data[3 * i + 3 * w * j+1] / 255f, 
                data[3 * i + 3 * w * j+2] / 255f);
        }
    }
}
