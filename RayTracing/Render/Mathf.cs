using System;

namespace RayTracing.Render.Mathematics
{
    public class Mathf
    {
        //介于[min,max]中的v值
        public static double Range(double v, double min, double max) => (v <= min) ? min : v >= max ? max : v;

        public static int Range(int v, int min, int max) => (v <= min) ? min : v >= max ? max : v;

        //a、b中的较小值
        public static double Min(double a, double b) { return a < b ? a : b; }

        //a、b中的较大值
        public static double Max(double a, double b) { return a > b ? a : b; }

        //交换a、b的值
        public static void Swap(ref double a, ref double b)
        {
            double c = a;
            a = b;
            b = c;
        }
    }
}
