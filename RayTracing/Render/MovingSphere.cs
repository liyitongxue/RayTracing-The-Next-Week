using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayTracing.Render.Components;
using RayTracing.Render.Materials;
using RayTracing.Render.Mathematics;
using Random = RayTracing.Render.Mathematics.Random;

namespace RayTracing.Render.Primitives
{
    public class MovingSphere:Hitable
    {
        public Vector3D pos0, pos1; //起点坐标和终点坐标
        public double time0, time1; //开始时间和结束时间
        public double radius;       //球体半径
        public Material material;   //球体材质

        //构造函数
        public MovingSphere(Vector3D p0, Vector3D p1, double t0, double t1, double r, Material m)
        {
            pos0 = p0;pos1 = p1;
            time0 = t0;time1 = t1;
            radius = r;material = m;
        }

        //获取在某一时刻的中心点坐标
        public Vector3D Center(double time)
        {
            return pos0 + (time - time0) / (time1 - time0) * (pos1 - pos0);
        }
            

        public override bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec rec)
        {
            Vector3D oc = ray.original - Center(ray.time);
            double a = ray.direction* ray.direction;
            double b = oc* ray.direction;
            double c = oc* oc - radius * radius;
            double discriminant = b * b - a * c;

            if (discriminant > 0)
            {
                double temp = (-b - Math.Sqrt(discriminant)) / a;
                if (temp < t_max && temp > t_min)
                {
                    rec.t = temp;
                    rec.p = ray.GetPoint(rec.t);
                    rec.normal = (rec.p - Center(ray.time)) / radius;
                    rec.material = material;
                    return true;
                }
                temp = (-b + Math.Sqrt(discriminant)) / a;
                if (temp < t_max && temp > t_min)
                {
                    rec.t = temp;
                    rec.p = ray.GetPoint(rec.t);
                    rec.normal = (rec.p - Center(ray.time)) / radius;
                    rec.material = material;
                    return true;
                }
            }
            return false;
        }

        public override bool BoundingBox(double t0, double t1, ref AABB box)
        {
            box = GetBox(
                new AABB(Center(t0) - new Vector3D(radius, radius, radius),
                    Center(t0) + new Vector3D(radius, radius, radius)),
                new AABB(Center(t1) - new Vector3D(radius, radius, radius),
                    Center(t1) + new Vector3D(radius, radius, radius)));
            return true;
        }
    }
}
