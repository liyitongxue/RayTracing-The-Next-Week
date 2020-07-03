using System;
using System.Windows.Forms;
using Random = RayTracing.Render.Mathematics.Random;

namespace RayTracing
{
    public partial class Form1 : Form
    {
        public static Form1 main;
        private readonly Render.Renderer renderer=new Render.Renderer();
        public Form1()
        {
            main = this;
            InitializeComponent();
        }
        
        private int samples;
        public void ShowTips()
        {
            lblTips.Text = "采样" + (++samples) + "次";
        }

        private void btnRender_Click(object sender, EventArgs e)
        {
            btnRender.Enabled = false;
            renderer.Init();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            renderer.Save();
        }
    }
}
