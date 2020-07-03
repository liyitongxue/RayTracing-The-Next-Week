using System;
using RayTracing.Render.Mathematics;
using Random=RayTracing.Render.Mathematics.Random;
namespace RayTracing.Render.Components
{
    public class Camera
    {
        public Vector3D position, lowLeftCorner, horizontal, vertical;
        public Vector3D u, v, w;
        public double radius;
        public double time0, time1;

        public Camera(Vector3D lookFrom, Vector3D lookat, Vector3D vup, double vfov, double aspect, 
            double r = 0,double focus_dist = 1,double t0=0,double t1=0)
        {
            time0 = t0;
            time1 = t1;
            radius = r * 0.5f;
            double unitAngle = Math.PI / 180f * vfov;
            double halfHeight = Math.Tan(unitAngle * 0.5f);
            double halfWidth = aspect * halfHeight;
            position = lookFrom;
             w = (lookat - lookFrom).GetNormalizeVector();
             u = (vup ^ w).GetNormalizeVector();
             v = (w ^ u).GetNormalizeVector();
            lowLeftCorner = lookFrom + w - halfWidth * u - halfHeight * v;
            horizontal = 2 * halfWidth * u;
            vertical = 2 * halfHeight * v;
            
        }

        public static Vector3D GetRandomPointInUnitDisk()
        {
            Vector3D p = 2.0 * new Vector3D(Random.Get(), Random.Get(), 0) - new Vector3D(1, 1, 0);
            return p.GetNormalizeVector() * Random.Get();
        }

        public Ray CreateRay(double x, double y)
        {
            //在下面声明射线对象的时候赋予参数，一个在时间0到时间1内的随机时间。
            if (radius == 0.0)
            {
                return new Ray(position, lowLeftCorner + x * horizontal + y * vertical - position, time0 + Random.Get() * (time1 - time0));
            }
            Vector3D rd = radius * GetRandomPointInUnitDisk();
            Vector3D offset = rd.X * u + rd.Y * v;

            return new Ray(position + offset, lowLeftCorner + x * horizontal + y * vertical - position - offset, time0 + Random.Get() * (time1 - time0));
        }
    }
}
