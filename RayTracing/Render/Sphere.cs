using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayTracing.Render.Components;
using RayTracing.Render.Materials;
using RayTracing.Render.Mathematics;

namespace RayTracing.Render.Primitives
{
    public class Sphere : Hitable
    {
        public Vector3D center;
        public double radius;
        public Material material;

        public Sphere(Vector3D cen, double rad, Material m)
        {
            center = cen;
            radius = rad;
            material = m;
        }

        public override bool BoundingBox(double t0, double t1, ref AABB box)
        {
            box=new AABB(center-new Vector3D(radius,radius,radius),center+new Vector3D(radius,radius,radius));
            return true;
        }

        public override bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec rec)
        {
            void GetSphereUV(ref  ShadeRec record)
            {
                double phi = Math.Atan2(record.p.Z, record.p.X);
                double theta = Math.Asin(record.p.Y);
                record.u = 1 - (phi + Math.PI) / (2 * Math.PI);
                record.v = (theta + Math.PI / 2) / Math.PI;
            }
            Vector3D oc = ray.original - center;
            double a = ray.direction* ray.direction;
            double b = 2.0 * (oc* ray.direction);
            double c = (oc* oc) - radius * radius;
            double discriminant = b * b - 4 * a * c;
            if (!(discriminant > 0)) return false;
            double temp = (-b - Math.Sqrt(discriminant)) / a * 0.5;
            if (temp < t_max && temp > t_min)
            {
                rec.material = material;
                rec.t = temp;
                rec.p = ray.GetPoint(rec.t);
                rec.normal = (rec.p - center).GetNormalizeVector();
                GetSphereUV(ref rec);
                return true;
            }
            temp = (-b + Math.Sqrt(discriminant)) / a * 0.5f;
            if (!(temp < t_max) || !(temp > t_min)) return false;
            rec.material = material;
            rec.t = temp;
            rec.p = ray.GetPoint(rec.t);
            rec.normal = (rec.p - center).GetNormalizeVector();
            GetSphereUV(ref rec);
            return true;
        }
    }
}
