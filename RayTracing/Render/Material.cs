using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RayTracing.Render.Components;
using RayTracing.Render.Mathematics;
using Random = RayTracing.Render.Mathematics.Random;

namespace RayTracing.Render.Materials
{
    public abstract class Material
    {
        public abstract bool scatter(Ray rayIn, ShadeRec record, ref SColor attenuation, ref Ray scattered);
        public virtual SColor emitted(double u,double v,Vector3D p)=>new SColor(0,0,0);
        public static Vector3D Reflect(Vector3D vin, Vector3D normal) => vin - 2 * (vin* normal) * normal;

        public Vector3D GetRandomPointInUnitSphere()
        {
            Vector3D p = new Vector3D(Random.Get(), Random.Get(), Random.Get()) * 2.0 - Vector3D.one;
            return p.GetNormalizeVector() * Random.Get();
        }

        public static bool Refract(Vector3D vin, Vector3D normal, double ni_no, ref Vector3D refracted)
        {
            Vector3D uvin = vin.GetNormalizeVector();
            double dt = uvin* normal;
            double discrimination = 1 - ni_no * ni_no * (1 - dt * dt);
            if (discrimination > 0)
            {
                refracted = ni_no * (uvin - normal * dt) - normal * Math.Sqrt(discrimination);
                return true;
            }

            return false;
        }

        public static double Schlick(double cosine, double refIdx)
        {
            double r0 = (1 - refIdx) / (1 + refIdx);
            r0 = r0 * r0;
            return r0 + (1 - r0) * Math.Pow((1 - cosine), 5);
        }
    }

    public class Metal : Material
    {
        public Texture texture;
        public double fuzz;

        public Metal(Texture t, double f)
        {
            fuzz = f;
            texture = t;
        }

        public override bool scatter(Ray rayIn, ShadeRec record, ref SColor attenuation, ref Ray scattered)
        {
            Vector3D reflected = Reflect(rayIn.normalDirection, record.normal);
            scattered = new Ray(record.p, reflected + fuzz * GetRandomPointInUnitSphere(),rayIn.time);
            attenuation = texture.value(record.u, record.v, record.p);
            return (scattered.direction* record.normal) > 0;
        }
        
    }
    public class Dielectrics : Material
    {
        double ref_idx;
        public Dielectrics(double ri) => ref_idx = ri;

        public override bool scatter(Ray rayIn, ShadeRec record, ref SColor attenuation, ref Ray scattered)
        {
            Vector3D outNormal;
            Vector3D reflected = Reflect(rayIn.direction, record.normal);
            attenuation = SColor.white;
            double ni_no = 1.0;
            Vector3D refracted = Vector3D.zero;
            double cos = 0;
            double reflect_prob = 0;
            if ((rayIn.direction* record.normal) > 0)
            {
                outNormal = -record.normal;
                ni_no = ref_idx;
                cos = ni_no * (rayIn.normalDirection* record.normal);
            }
            else
            {
                outNormal = record.normal;
                ni_no = 1.0 / ref_idx;
                cos = -(rayIn.normalDirection* record.normal);
            }

            reflect_prob = Refract(rayIn.direction, outNormal, ni_no, ref refracted) ? Schlick(cos, ref_idx) : 1;
            scattered = Random.Get() <= reflect_prob
                ? new Ray(record.p, reflected,rayIn.time)
                : new Ray(record.p, refracted,rayIn.time);
            return true;
        }
    }
    public class Lambertian : Material
    {
        public Texture texture;
        public Lambertian(Texture t) => texture = t;

        public override bool scatter(Ray rayIn, ShadeRec record, ref SColor attenuation, ref Ray scattered)
        {
            Vector3D target = record.p + record.normal + GetRandomPointInUnitSphere();
            scattered = new Ray(record.p, target - record.p,rayIn.time);

            //float phi = Mathf.Atan2(record.p.z, record.p.x);
            //float theta = Mathf.Asin(record.p.y);
            //var u = 1 - (phi + Mathf.PI) / (2 * Mathf.PI);
            //var v = (theta + Mathf.PI / 2) / Mathf.PI;
            //attenuation = texture.value(u, v, record.p);
            attenuation = texture.value(record.u, record.v, record.p);
            return true;
        }
    }

    public class DiffuseLight : Material
    {
        private readonly Texture texture;
        private double intensity;
        public DiffuseLight(Texture t,double i)
        {
            texture = t;
            intensity = i;
        }
        
        public override bool scatter(Ray rayIn, ShadeRec record, ref SColor attenuation, ref Ray scattered)=>false;
        
        public override SColor emitted(double u, double v, Vector3D p)=>texture.value(u, v, p)*intensity;
        
    }

    public class Isotropic : Material
    {
        public Texture texture;
        public Isotropic(Texture t)
        {
            texture = t;
        }

        public override bool scatter(Ray rayIn, ShadeRec record, ref SColor attenuation, ref Ray scattered)
        {
           scattered=new Ray(record.p,GetRandomPointInUnitSphere(),rayIn.time);
            attenuation = texture.value(record.u, record.v, record.p);
            return true;
        }
    }
}
