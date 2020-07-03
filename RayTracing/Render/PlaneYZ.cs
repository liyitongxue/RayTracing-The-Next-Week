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
    class PlaneXY:Hitable
    {
        private double x0, x1, y0, y1, k;
        private Material material;
        public override bool BoundingBox(double t0, double t1, ref AABB box)
        {
            box=new AABB(new Vector3D(x0,y0,k-0.0001),new Vector3D(x1,y1,k+0.0001) );
            return true;
        }

        public PlaneXY(double _x0, double _x1, double _y0, double _y1, double _k, Material mat)
        {
            material = mat;
            x0 = _x0;
            x1 = _x1;
            y0 = _y0;
            y1 = _y1;
            k = _k;
        }
        public override bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec rec)
        {
            double t = (k - ray.original.Z) / ray.direction.Z;
            if (t < t_min || t > t_max) return false;
            double x = ray.original.X + t * ray.direction.X;
            if (x < x0 || x > x1) return false;
            double y = ray.original.Y + t * ray.direction.Y;
            if (y < y0 || y > y1) return false;

            rec.u = (x - x0) / (x1 - x0);
            rec.v = (y - y0) / (y1 - y0);
            rec.t = t;
            rec.material = material;
            rec.normal=new Vector3D(0,0,1);
            rec.p = ray.GetPoint(t);
            return true;
        }
    }

    class PlaneXZ : Hitable
    {
        private double x0, x1, z0, z1, k;
        private Material material;

        public PlaneXZ(double _x0, double _x1, double _z0, double _z1, double _k, Material mat)
        {
            material = mat;
            x0 = _x0;
            x1 = _x1;
            z0 = _z0;
            z1 = _z1;
            k = _k;
        }
        public override bool BoundingBox(double t0, double t1, ref AABB box)
        {
            box = new AABB(new Vector3D(x0, k - 0.0001,z0), new Vector3D(x1, k + 0.0001,z1 ));
            return true;
        }
        public override bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec rec)
        {
            double t = (k - ray.original.Y) / ray.direction.Y;
            if (t < t_min || t > t_max) return false;
            double x = ray.original.X + t * ray.direction.X;
            if (x < x0 || x > x1) return false;
            double z = ray.original.Z + t * ray.direction.Z;
            if (z < z0 || z > z1) return false;

            rec.u = (x - x0) / (x1 - x0);
            rec.v = (z - z0) / (z1 - z0);
            rec.t = t;
            rec.material = material;
            rec.normal = new Vector3D(0, 1, 0);
            rec.p = ray.GetPoint(t);
            return true;
        }
    }

    class PlaneYZ : Hitable
    {
        private double z0, z1, y0, y1, k;
        private Material material;

        public PlaneYZ(double _y0, double _y1, double _z0, double _z1, double _k, Material mat)
        {
            material = mat;
            z0 = _z0;
            z1 = _z1;
            y0 = _y0;
            y1 = _y1;
            k = _k;
        }
        public override bool BoundingBox(double t0, double t1, ref AABB box)
        {
            box = new AABB(new Vector3D(k - 0.0001, y0, z0), new Vector3D(k + 0.0001, y1, z1));
            return true;
        }
        public override bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec rec)
        {
            double t = (k - ray.original.X) / ray.direction.X;
            if (t < t_min || t > t_max) return false;
            double z = ray.original.Z + t * ray.direction.Z;
            if (z < z0 || z > z1) return false;
            double y = ray.original.Y + t * ray.direction.Y;
            if (y < y0 || y > y1) return false;

            rec.v = (z - z0) / (z1 - z0);
            rec.u = (y - y0) / (y1 - y0);
            rec.t = t;
            rec.material = material;
            rec.normal = new Vector3D(1, 0, 0);
            rec.p = ray.GetPoint(t);
            return true;
        }
    }
}
