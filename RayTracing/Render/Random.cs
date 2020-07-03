using System.IO;

namespace RayTracing.Render.Mathematics
{
    public class Sobol
    {
        private int now = 0;

        private int Next()
        {
            if (++now == int.MaxValue)
            {
                now = 0;
            }
            return now;
        }

        private static double IntegerRadicalInverse(int Base, int i)
        {
            int inverse;
            int numPoints = 1;
            for (inverse = 0; i > 0; i /= Base)
            {
                inverse = inverse * Base + (i % Base);
                numPoints = numPoints * Base;
            }
            return inverse / (double) numPoints;
        }

        public double Get() => IntegerRadicalInverse(2, Next());

    }

    public static class Random
    {
        private static readonly System.Random random = new System.Random();
        private static readonly Sobol sobol = new Sobol();

        static long seed = 1;
        public static double Get()
        {
            //采样更快
            seed = (0x5DEECE66DL * seed + 0xB16) & 0xFFFFFFFFFFFFL;
            return (seed >> 16) / (double)0x100000000L;

            //var seed = Guid.NewGuid().GetHashCode();
            //Random r = new Random(seed);
            //int i = r.Next(0, 100000);
            //return (double)i / 100000;
        }
    }
}