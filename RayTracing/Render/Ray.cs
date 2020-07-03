using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayTracing.Render.Mathematics;

namespace RayTracing.Render.Components
{
    public class Ray
    {
        public readonly Vector3D original;
        public readonly Vector3D direction;
        public readonly Vector3D normalDirection;

        public double time;//储存时间的变量

        //构造函数
        public Ray(Vector3D o, Vector3D d,double t=0)
        {
            time = t;
            original = o;
            direction = d;
            normalDirection = d.GetNormalizeVector();
        }

        public Vector3D GetPoint(double t)
        {
            return original + direction * t;
        }
    }
}
