using System;

namespace RayTracing.Render.Mathematics
{
    public class Vector3D
    {
        public double[] data=new double[3];
        public double X {get => data[0];set => data[0] = value;}
        public double Y {get => data[1];set => data[1] = value;}
        public double Z {get => data[2];set => data[2] = value;}
       
        public double this[int index]
        {
            get => data[index];
            set => data[index] = value;
        }

        //默认构造函数
        public Vector3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        //构造函数
        public Vector3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3D(Vector3D copy)
        {
            X = copy.X;
            Y = copy.Y;
            Z = copy.Z;
        }

        //零向量
        public static Vector3D zero = new Vector3D(0, 0, 0);

        //单位向量
        public static Vector3D one = new Vector3D(1, 1, 1);

        //向量的模，向量的大小
        public double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        ////返回本向量的归一化向量，本向量不变
        public Vector3D GetNormalizeVector()
        {
            double d = Magnitude();
            return new Vector3D(X / d, Y / d, Z / d);
        }

        //向量加法
        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        //向量减法
        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        //向量 * 数
        public static Vector3D operator *(Vector3D v, double d)
        {
            return new Vector3D(d * v.X, d * v.Y, d * v.Z);
        }

        //数 * 向量
        public static Vector3D operator *(double d, Vector3D v)
        {
            return new Vector3D(d * v.X, d * v.Y, d * v.Z);
        }

        //向量 / 数
        public static Vector3D operator /(Vector3D v, double d)
        {
            return new Vector3D(v.X / d, v.Y / d, v.Z / d);
        }

        //反向量
        public static Vector3D operator -(Vector3D v)
        {
            return new Vector3D(-v.X, -v.Y, -v.Z);
        }

        //向量点乘
        public static double operator *(Vector3D v1, Vector3D v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        //向量叉乘
        public static Vector3D operator ^(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
        }
    }
}
