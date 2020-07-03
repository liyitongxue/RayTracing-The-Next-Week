using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using RayTracing.Render.Materials;
using RayTracing.Render.Mathematics;

namespace RayTracing.Render.Components
{
    public class ShadeRec
    {
        public double u, v;
        public double t;
        public Vector3D p;
        public Vector3D normal;
        public Material material;
        public ShadeRec() { }

        public ShadeRec(ShadeRec copy)
        {
            u = copy.u;
            v = copy.v;
            p = copy.p;
            normal = copy.normal;
            material = copy.material;
        }
    }

    public abstract class Hitable
    {
        public string name;
        public abstract bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec sr);
        public abstract bool BoundingBox(double t0, double t1, ref AABB box);
        public AABB GetBox(AABB box0, AABB box1)
        {
            Vector3D small = new Vector3D(
                Mathf.Min(box0.min.X, box1.min.X),
                Mathf.Min(box0.min.Y, box1.min.Y),
                Mathf.Min(box0.min.Z, box1.min.Z));
            Vector3D big = new Vector3D(
                Mathf.Max(box0.max.X, box1.max.X),
                Mathf.Max(box0.max.Y, box1.max.Y),
                Mathf.Max(box0.max.Z, box1.max.Z));
            return new AABB(small, big);
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class HitableList : Hitable
    {
        public readonly List<Hitable> list = new List<Hitable>();

        public override bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec sr)
        {
            ShadeRec temp_record = new ShadeRec();
            bool hit_anything = false;
            double closest = t_max;
            foreach (Hitable h in list)
            {
                if (!h.Hit(ray, t_min, closest, ref temp_record)) continue;
                hit_anything = true;
                closest = temp_record.t;
                sr = temp_record;
            }

            return hit_anything;
        }

        public override bool BoundingBox(double t0, double t1, ref AABB box)
        {
            if (list.Count == 0) return false;
            AABB tempBox = new AABB();
            if (!list[0].BoundingBox(t0, t1, ref tempBox)) return false;
            box = tempBox;
            foreach (Hitable t in list)
            {
                if (t.BoundingBox(t0, t1, ref tempBox))
                    box = GetBox(box, tempBox);
                else return false;
            }
            return true;
        }
    }

    public class BVHNode : Hitable
    {
        public Hitable left, right;
        public AABB box;
        public BVHNode() { }
        public override string ToString()
        {
            if (left.ToString() == right.ToString()) return "<" + left + ">";
            return "<" + left + "," + right + ">";
        }

        public BVHNode(Hitable[] p, int n, double time0, double time1)
        {
            //用来排序的嵌套函数
            int Compare(Hitable a, Hitable b, int i)
            {
                AABB l = new AABB(), r = new AABB();
                if (!a.BoundingBox(0, 0, ref l) || !b.BoundingBox(0, 0, ref r)) throw new Exception("NULL");
                return l.min[i] - r.min[i] < 0 ? -1 : 1;
            }
            //用来排序的分割数组的嵌套函数
            Hitable[] SplitArray(Hitable[] Source, int StartIndex, int EndIndex)
            {
                Hitable[] result = new Hitable[EndIndex - StartIndex + 1];
                for (int i = 0; i <= EndIndex - StartIndex; i++) result[i] = Source[i + StartIndex];
                return result;
            }

            //随机一个轴，x轴:0 y轴:1 z轴:3
            int method = (int)(3 * Mathematics.Random.Get());

            //转换为List然后使用排序,最后再转换会Array
            //排序规则使用lambda表达式转向比较函数，并加入轴向参数
            List<Hitable> temp_list = p.ToList();
            temp_list.Sort((a, b) => Compare(a, b, method));
            p = temp_list.ToArray();

            //检测当前子节点数量，如果大于2则继续分割
            switch (n)
            {
                case 1:
                    left = right = p[0];
                    break;
                case 2:
                    left = p[0];
                    right = p[1];
                    break;
                default://拆分
                    left = new BVHNode(SplitArray(p, 0, n / 2 - 1), n / 2, time0, time1);
                    right = new BVHNode(SplitArray(p, n / 2, n - 1), n - n / 2, time0, time1);
                    break;
            }
            //根据子节点生成当前节点的包围盒
            AABB box_left = new AABB(), box_right = new AABB();
            if (!left.BoundingBox(time0, time1, ref box_left) || !right.BoundingBox(time0, time1, ref box_right))
                throw new Exception("no bounding box in bvh_node constructor");
            box = GetBox(box_left, box_right);
            if (n == 6) Console.WriteLine("结果" + this);
        }

        public override bool BoundingBox(double t0, double t1, ref AABB b)
        {
            b = box;
            return true;
        }

        public override bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec sr)
        {
            //检测包围和碰撞，返回碰撞的子树的信息
            if (box.Hit(ray, t_min, t_max))
            {
                ShadeRec left_rec = new ShadeRec(), right_rec = new ShadeRec();
                bool hit_left = left.Hit(ray, t_min, t_max, ref left_rec);
                bool hit_right = right.Hit(ray, t_min, t_max, ref right_rec);
                if (hit_left && hit_right)
                {
                    sr = left_rec.t < right_rec.t ? left_rec : right_rec;
                    return true;
                }
                if (hit_left)
                {
                    sr = left_rec;
                    return true;
                }
                if (hit_right)
                {
                    sr = right_rec;
                    return true;
                }
                return false;
            }
            return false;
        }

    }

    //翻转法线
    public class FilpNormals : Hitable
    {
        private readonly Hitable hitable;
        public FilpNormals(Hitable p)
        {
            hitable = p;
        }

        public override bool BoundingBox(double t0, double t1, ref AABB box)
        {
            return hitable.BoundingBox(t0, t1, ref box);
        }

        public override bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec sr)
        {
            if (!hitable.Hit(ray, t_min, t_max, ref sr)) return false;
            sr.normal = -sr.normal;
            return true;
        }
    }

    //位移类
    public class Translate : Hitable
    {
        public Hitable Object;
        private Vector3D offset;

        public Translate(Hitable p, Vector3D displace)
        {
            offset = displace;
            Object = p;
        }

        public override bool BoundingBox(double t0, double t1, ref AABB box)
        {
            if (!Object.BoundingBox(t0, t1, ref box)) return false;
            box = new AABB(box.min + offset, box.max + offset);
            return true;
        }

        public override bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec sr)
        {
            Ray moved = new Ray(ray.original - offset, ray.direction, ray.time);
            if (!Object.Hit(moved, t_min, t_max, ref sr)) return false;
            sr.p += offset;
            return true;
        }
    }

    //Y轴旋转类
    public class RotateY : Hitable
    {
        public AABB bbox = new AABB();
        public bool hasbox;
        public Hitable Object;
        private double sin_theta, cos_theta;
        public override bool BoundingBox(double t0, double t1, ref AABB box)
        {
            box = bbox;
            return hasbox;
        }

        public RotateY(Hitable p, double angle)
        {
            Object = p;
            double radians = (Math.PI / 180f) * angle;
            sin_theta = Math.Sin(radians);
            cos_theta = Math.Cos(radians);
            hasbox = Object.BoundingBox(0, 1, ref bbox);
            Vector3D min = new Vector3D(double.MaxValue, double.MaxValue, double.MaxValue);
            Vector3D max = new Vector3D(-double.MaxValue, -double.MaxValue, -double.MaxValue);
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < 2; k++)
                    {
                        double x = i * bbox.max.X + (1 - i) * bbox.min.X;
                        double y = j * bbox.max.Y + (1 - j) * bbox.min.Y;
                        double z = k * bbox.max.Z + (1 - k) * bbox.min.Z;
                        double newx = cos_theta * x + sin_theta * z;
                        double newz = -sin_theta * x + cos_theta * z;
                        Vector3D tester = new Vector3D(newx, y, newz);
                        for (int c = 0; c < 3; c++)
                        {
                            if (tester[c] > max[c]) max[c] = tester[c];
                            if (tester[c] < min[c]) min[c] = tester[c];
                        }
                    }


            bbox = new AABB(min, max);
        }

        public override bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec sr)
        {
            Vector3D origin = new Vector3D(ray.original);
            Vector3D direction = new Vector3D(ray.direction);
            origin[0] = cos_theta * ray.original[0] - sin_theta * ray.original[2];
            origin[2] = sin_theta * ray.original[0] + cos_theta * ray.original[2];
            direction[0] = cos_theta * ray.direction[0] - sin_theta * ray.direction[2];
            direction[2] = sin_theta * ray.direction[0] + cos_theta * ray.direction[2];
            Ray rotatedR = new Ray(origin, direction, ray.time);
            ShadeRec rec = new ShadeRec(sr);
            if (Object.Hit(rotatedR, t_min, t_max, ref sr))
            {
                Vector3D p = new Vector3D(sr.p);
                Vector3D normal = new Vector3D(sr.normal);
                p[0] = cos_theta * sr.p[0] + sin_theta * sr.p[2];
                p[2] = -sin_theta * sr.p[0] + cos_theta * sr.p[2];
                normal[0] = cos_theta * sr.normal[0] + sin_theta * sr.normal[2];
                normal[2] = -sin_theta * sr.normal[0] + cos_theta * sr.normal[2];
                sr.p = p;
                sr.normal = normal;
                return true;
            }
            sr = rec;
            return false;
        }
    }

    public class ConstantMedium : Hitable
    {
        private Hitable boundary;
        private double density;
        private Material phase_function;

        public ConstantMedium(Hitable b, double d, Texture a)
        {
            boundary = b;
            density = d;
            phase_function = new Isotropic(a);
        }

        public override bool BoundingBox(double t0, double t1, ref AABB box)
        {
            return boundary.BoundingBox(t0, t1, ref box);
        }

        public override bool Hit(Ray ray, double t_min, double t_max, ref ShadeRec sr)
        {
            ShadeRec sr1 = new ShadeRec(), sr2 = new ShadeRec();
            if (boundary.Hit(ray, -double.MaxValue, double.MaxValue, ref sr1))
            {
                if (boundary.Hit(ray, sr1.t + 0.0001, double.MaxValue, ref sr2))
                {
                    sr1.t = Mathf.Range(sr1.t, t_min, t_max);
                    if (sr1.t < t_min) sr1.t = t_min;
                    if (sr2.t > t_max) sr2.t = t_max;
                    if (sr1.t >= sr2.t) return false;
                    if (sr1.t < 0) sr1.t = 0;
                    double distance_inside_boundary = (sr2.t - sr1.t) * ray.direction.Magnitude();

                    double hit_distance = -(1 / density) * Math.Log(Mathematics.Random.Get());
                    if (hit_distance < distance_inside_boundary)
                    {
                        sr.t = sr1.t + hit_distance / ray.direction.Magnitude();
                        sr.p = ray.GetPoint(sr.t);
                        sr.normal = new Vector3D(1, 0, 0);
                        sr.material = phase_function;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
