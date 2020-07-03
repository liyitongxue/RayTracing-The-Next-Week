using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using AcDx;
using RayTracing.Render.Components;
using RayTracing.Render.Materials;
using RayTracing.Render.Mathematics;
using RayTracing.Render.Primitives;
using Random = RayTracing.Render.Mathematics.Random;

namespace RayTracing.Render
{
    public class Preview : DxWindow
    {
        public override void Update()
        {
            for (int i = 0; i < Buff.Length; i++)
                Buff[i] = (byte)Mathf.Range(Renderer.main.buff[i] * 255 / Renderer.main.Changes[i / 4] + 0.5f, 0, 255f);
        }
    }
    public class Renderer
    {
        public static Renderer main;
        private readonly Mode mode = Mode.Diffusing;
        public bool SkyColor = true;
        private readonly HitableList world = new HitableList();
        public int Samples = 10240, MAX_SCATTER_TIME = 4;
        private int width = 512, height = 512;
        private readonly Preview preview = new Preview();
        public double[] buff;
        public int[] Changes;

        public Camera camera;
        private double recip_width, recip_height;

        public void Init()
        {
            main = this;
            buff = new double[width * height * 4];
            Changes = new int[width * height];
            InitScene();

            Start();
            preview.Run(new DxConfiguration("预览", width, height));
        }

        private void InitScene()
        {
            recip_width = 1.0 / width;
            recip_height = 1.0 / height;

            //康奈尔盒子
            CornellBox();
        }

        void CornellBox()
        {
            SkyColor = false;
            camera = new Camera(new Vector3D(278, 278, -800), new Vector3D(278, 278, 0), new Vector3D(0, 1, 0), 40, (double)width / (double)height);
            List<Hitable> list = new List<Hitable>();
            list.Add(new FilpNormals(new PlaneYZ(0, 555, 0, 555, 555, new Lambertian(new ConstantTexture(SColor.green)))));
            list.Add(new PlaneYZ(0, 555, 0, 555, 0, new Lambertian(new ConstantTexture(SColor.red))));
            list.Add(new PlaneXZ(213, 343, 227, 332, 554, new DiffuseLight(new ConstantTexture(new SColor(1, 1, 1, 1)), 15)));
            list.Add(new FilpNormals(new PlaneXZ(0, 555, 0, 555, 555, new Lambertian(new ConstantTexture(SColor.white)))));
            list.Add(new PlaneXZ(0, 555, 0, 555, 0, new Lambertian(new ConstantTexture(SColor.white))));
            list.Add(new FilpNormals(new PlaneXY(0, 555, 0, 555, 555, new Lambertian(new ConstantTexture(SColor.white)))));


            world.list.Add(new BVHNode(list.ToArray(), list.Count, 0, 1));
            world.list.Add(new Translate(new RotateY(
                    new Cube(
                        new Vector3D(0, 0, 0),
                        new Vector3D(165, 330, 165),
                        new Lambertian(new ConstantTexture(SColor.white))), 15)
                , new Vector3D(265, 0, 295)));
            world.list.Add(new Translate(new RotateY(
                    new Cube(
                        new Vector3D(0, 0, 0),
                        new Vector3D(165, 165, 165),
                        new Lambertian(new ConstantTexture(SColor.white))), -18),
                new Vector3D(130, 0, 65)));
            world.list.Add(new Sphere(new Vector3D(190, 165 + 60, 95), 60, new Dielectrics(1.5)));
        }

        private class ScannerConfig
        {
            public readonly int w, h;

            public ScannerConfig(int h, int w)
            {
                this.h = h;
                this.w = w;
            }
        }

        private async void Start()
        {
            ThreadPool.SetMaxThreads(16, 16);
            await Task.Factory.StartNew(delegate { LinearScanner(new ScannerConfig(height, width)); });
            for (int i = 1; i < Samples; i++)
                ThreadPool.QueueUserWorkItem(LinearScanner, new ScannerConfig(height, width));
        }

        private void LinearScanner(object o)
        {
            ScannerConfig config = (ScannerConfig)o;
            for (int j = config.h - 1; j >= 0; j--)
                for (int i = 0; i < config.w; i++)
                {
                    SColor color = mode == Mode.Diffusing
                        ? Diffusing(camera.CreateRay(
                            (i + Random.Get()) * recip_width,
                            (j + Random.Get()) * recip_height), world, 0)
                        : NormalMap(camera.CreateRay(
                            (i + Random.Get()) * recip_width,
                            (j + Random.Get()) * recip_height), world);
                    SetPixel(config.w - i - 1, config.h - j - 1, color);
                }
            Form1.main.BeginInvoke(new Action(() => { Form1.main.ShowTips(); }));
        }

        private void SetPixel(int x, int y, SColor c32)
        {
            int i = width * 4 * y + x * 4;
            Changes[width * y + x]++;
            buff[i] += c32.R;
            buff[i + 1] += c32.G;
            buff[i + 2] += c32.B;
            buff[i + 3] += c32.A;
        }

        private SColor NormalMap(Ray ray, HitableList hitableList)
        {
            ShadeRec record = new ShadeRec();
            if (hitableList.Hit(ray, 0.0, double.MaxValue, ref record))
                return 0.5 * new SColor(record.normal.X + 1, record.normal.Y + 1, record.normal.Z + 1, 2.0);
            double t = 0.5 * ray.normalDirection.Y + 1.0;
            return (1 - t) * new SColor(1, 1, 1) + t * new SColor(0.5, 0.7, 1);
        }

        private SColor Diffusing(Ray ray, HitableList hitableList, int depth)
        {
            ShadeRec record = new ShadeRec();
            if (hitableList.Hit(ray, 0.0001, double.MaxValue, ref record))
            {
                Ray r = new Ray(Vector3D.zero, Vector3D.zero);
                SColor attenuation = SColor.black;
                SColor emitted = record.material.emitted(record.u, record.v, record.p);
                if (depth >= MAX_SCATTER_TIME || !record.material.scatter(ray, record, ref attenuation, ref r))
                    return emitted;
                SColor clr = Diffusing(r, hitableList, depth + 1);
                return new SColor(clr.R * attenuation.R, clr.G * attenuation.G, clr.B * attenuation.B) + emitted;
            }
            double t = 0.5 * ray.normalDirection.Y + 1.0;
            return SkyColor ? (1 - t) * new SColor(2, 2, 2) + t * new SColor(0.5, 0.7, 1) : SColor.black;
        }

        private enum Mode
        {
            NormalMap,
            Diffusing
        };

        public void Save()
        {
            byte[] pic_buff = preview.Buff;
            Bitmap pic = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int i = 0; i < pic_buff.Length; i += 4)
            {
                Color c = Color.FromArgb(pic_buff[i + 3], pic_buff[i], pic_buff[i + 1], pic_buff[i + 2]);
                pic.SetPixel(i % (width * 4) / 4, i / (width * 4), c);
            }
            pic.Save("rt.png");
        }
    }
}
