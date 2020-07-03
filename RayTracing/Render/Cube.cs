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
    //立方体
    class Cube : Hitable
    {
        public Vector3D pmin, pmax;
        public HitableList list;

        public Cube(Vector3D p0, Vector3D p1, Material mat, Material m2 = null)
        {
            pmax = p1;
            pmin = p0;
            if (m2 == null) m2 = mat;
            list = new HitableList();
            list.list.Add(new PlaneXY(p0.X, p1.X, p0.Y, p1.Y, p1.Z, m2));
            list.list.Add(new FilpNormals(new PlaneXY(p0.X, p1.X, p0.Y, p1.Y, p0.Z, m2)));
            list.list.Add(new PlaneXZ(p0.X, p1.X, p0.Z, p1.Z, p1.Y, mat));
            list.list.Add(new FilpNormals(new PlaneXZ(p0.X, p1.X, p0.Z, p1.Z, p0.Y, mat)));
            list.list.Add(new PlaneYZ(p0.Y, p1.Y, p0.Z, p1.Z, p1.X, m2));
            list.list.Add(new FilpNormals(new PlaneYZ(p0.Y, p1.Y, p0.Z, p1.Z, p0.X, m2)));
        }

        public override bool BoundingBox(double t0, double t1, ref AABB box)
        {
            box = new AABB(pmin, pmax);
            return true;
        }

        public override bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec sr)
        {
            return list.Hit(ray, t_min, t_max, ref sr);
        }
    }
}
