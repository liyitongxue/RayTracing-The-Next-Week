using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayTracing.Render.Mathematics;

namespace RayTracing.Render.Components
{
    public class AABB
    {
        public Vector3D min, max;
        public AABB()
        {
            min = new Vector3D();
            max = new Vector3D();
        }
        public AABB(Vector3D a, Vector3D b)
        {
            min = a;
            max = b;
        }

        public bool Hit(Ray r, double tmin, double tmax)
        {
            for (int a = 0; a < 3; a++)
            {
                double t0 = Mathf.Min((min[a] - r.original[a]) / r.direction[a],
                    (max[a] - r.original[a]) / r.direction[a]);
                double t1 = Mathf.Max((min[a] - r.original[a]) / r.direction[a],
                    (max[a] - r.original[a]) / r.direction[a]);
                tmin = Mathf.Max(t0, tmin);
                tmax = Mathf.Min(t1, tmax);
                if (tmax <= tmin)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
