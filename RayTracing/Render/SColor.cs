using System;
using System.Drawing;

namespace RayTracing.Render.Mathematics
{
    public class SColor
    {
        public readonly double R, G, B, A;

        //默认构造函数
        public SColor()
        {
            this.R = 0;
            this.G = 0;
            this.B = 0;
        }

        //构造函数
        public SColor(double r, double g, double b, double a = 1)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public static SColor operator +(SColor color1, SColor color2)
        {
            return new SColor(color1.R + color2.R, color1.G + color2.G, color1.B + color2.B, color1.A + color2.A);
        }


        public static SColor operator -(SColor color1, SColor color2)
        {
            return new SColor(color1.R - color2.R, color1.G - color2.G, color1.B - color2.B, color1.A - color2.A);
        }


        public static SColor operator *(SColor color1, SColor color2)
        {
            return new SColor(color1.R * color2.R, color1.G * color2.G, color1.B * color2.B, color1.A * color2.A);
        }


        public static SColor operator *(SColor color, double d)
        {
            return new SColor(color.R * d, color.G * d, color.B * d, color.A * d);
        }

        public static SColor operator *(double d, SColor color)
        {
            return new SColor(color.R * d, color.G * d, color.B * d, color.A * d);
        }

        public static SColor operator /(SColor color, double d)
        {
            return new SColor(color.R / d, color.G / d, color.B / d, color.A / d);
        }

        //几种常见的颜色
        public static SColor
            black = new SColor(0, 0, 0),//黑色
            red = new SColor(1, 0, 0),//红色
            green = new SColor(0, 1, 0),//绿色
            blue = new SColor(0, 0, 1),//蓝色
            white = new SColor(1, 1, 1);//白色
    }
}
