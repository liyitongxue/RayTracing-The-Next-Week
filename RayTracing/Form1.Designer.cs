namespace RayTracing
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRender = new System.Windows.Forms.Button();
            this.lblTips = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRender
            // 
            this.btnRender.Location = new System.Drawing.Point(53, 14);
            this.btnRender.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRender.Name = "btnRender";
            this.btnRender.Size = new System.Drawing.Size(267, 58);
            this.btnRender.TabIndex = 0;
            this.btnRender.Text = "开始渲染";
            this.btnRender.UseVisualStyleBackColor = true;
            this.btnRender.Click += new System.EventHandler(this.btnRender_Click);
            // 
            // SPP
            // 
            this.lblTips.AutoSize = true;
            this.lblTips.Font = new System.Drawing.Font("微软雅黑", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTips.Location = new System.Drawing.Point(65, 145);
            this.lblTips.Name = "Tips";
            this.lblTips.Size = new System.Drawing.Size(0, 65);
            this.lblTips.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(53, 76);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(267, 58);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "保存图片";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 284);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblTips);
            this.Controls.Add(this.btnRender);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "RayTracing";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRender;
        private System.Windows.Forms.Label lblTips;
        private System.Windows.Forms.Button btnSave;
    }
}

