namespace UnitySample
{
    partial class Form0
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.GroupBox groupBox1;
            this.btnResolveAll = new System.Windows.Forms.Button();
            this.numericUpDownAge = new System.Windows.Forms.NumericUpDown();
            this.btnEvilAcme = new System.Windows.Forms.Button();
            this.btnChineseAcme = new System.Windows.Forms.Button();
            this.btnOldAcme = new System.Windows.Forms.Button();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnTransient = new System.Windows.Forms.Button();
            this.btnSingleton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnNonregistered = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnCtorInjection = new System.Windows.Forms.Button();
            this.btnInjectDependencies = new System.Windows.Forms.Button();
            this.btnConfig = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAge)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(this.btnConfig);
            groupBox1.Controls.Add(this.btnResolveAll);
            groupBox1.Controls.Add(this.numericUpDownAge);
            groupBox1.Controls.Add(this.btnEvilAcme);
            groupBox1.Controls.Add(this.btnChineseAcme);
            groupBox1.Controls.Add(this.btnOldAcme);
            groupBox1.Controls.Add(this.shapeContainer1);
            groupBox1.Location = new System.Drawing.Point(13, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(239, 74);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Resolve Type Samples";
            // 
            // btnResolveAll
            // 
            this.btnResolveAll.Location = new System.Drawing.Point(0, 46);
            this.btnResolveAll.Name = "btnResolveAll";
            this.btnResolveAll.Size = new System.Drawing.Size(75, 23);
            this.btnResolveAll.TabIndex = 12;
            this.btnResolveAll.Text = "ResolveAll";
            this.btnResolveAll.UseVisualStyleBackColor = true;
            this.btnResolveAll.Click += new System.EventHandler(this.btnResolveAll_Click);
            // 
            // numericUpDownAge
            // 
            this.numericUpDownAge.Location = new System.Drawing.Point(162, 49);
            this.numericUpDownAge.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownAge.Name = "numericUpDownAge";
            this.numericUpDownAge.Size = new System.Drawing.Size(71, 20);
            this.numericUpDownAge.TabIndex = 4;
            this.numericUpDownAge.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // btnEvilAcme
            // 
            this.btnEvilAcme.Location = new System.Drawing.Point(0, 19);
            this.btnEvilAcme.Name = "btnEvilAcme";
            this.btnEvilAcme.Size = new System.Drawing.Size(75, 23);
            this.btnEvilAcme.TabIndex = 0;
            this.btnEvilAcme.Text = "Evil ACME";
            this.btnEvilAcme.UseVisualStyleBackColor = true;
            this.btnEvilAcme.Click += new System.EventHandler(this.btnEvilAcme_Click);
            // 
            // btnChineseAcme
            // 
            this.btnChineseAcme.Location = new System.Drawing.Point(81, 19);
            this.btnChineseAcme.Name = "btnChineseAcme";
            this.btnChineseAcme.Size = new System.Drawing.Size(75, 23);
            this.btnChineseAcme.TabIndex = 1;
            this.btnChineseAcme.Text = "Chinese ACME";
            this.btnChineseAcme.UseVisualStyleBackColor = true;
            this.btnChineseAcme.Click += new System.EventHandler(this.btnChineseAcme_Click);
            // 
            // btnOldAcme
            // 
            this.btnOldAcme.Location = new System.Drawing.Point(162, 19);
            this.btnOldAcme.Name = "btnOldAcme";
            this.btnOldAcme.Size = new System.Drawing.Size(75, 23);
            this.btnOldAcme.TabIndex = 2;
            this.btnOldAcme.Text = "Old ACME";
            this.btnOldAcme.UseVisualStyleBackColor = true;
            this.btnOldAcme.Click += new System.EventHandler(this.btnOldAcme_Click);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(3, 16);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(233, 55);
            this.shapeContainer1.TabIndex = 3;
            this.shapeContainer1.TabStop = false;
            // 
            // lineShape1
            // 
            this.lineShape1.Name = "lineShape1";
            this.lineShape1.X1 = 156;
            this.lineShape1.X2 = 156;
            this.lineShape1.Y1 = -5;
            this.lineShape1.Y2 = 53;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(13, 92);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(693, 215);
            this.textBox1.TabIndex = 3;
            this.textBox1.WordWrap = false;
            // 
            // btnTransient
            // 
            this.btnTransient.Location = new System.Drawing.Point(0, 18);
            this.btnTransient.Name = "btnTransient";
            this.btnTransient.Size = new System.Drawing.Size(75, 39);
            this.btnTransient.TabIndex = 4;
            this.btnTransient.Text = "Transient";
            this.btnTransient.UseVisualStyleBackColor = true;
            this.btnTransient.Click += new System.EventHandler(this.btnTransient_Click);
            // 
            // btnSingleton
            // 
            this.btnSingleton.Location = new System.Drawing.Point(81, 18);
            this.btnSingleton.Name = "btnSingleton";
            this.btnSingleton.Size = new System.Drawing.Size(75, 39);
            this.btnSingleton.TabIndex = 5;
            this.btnSingleton.Text = "Singleton";
            this.btnSingleton.UseVisualStyleBackColor = true;
            this.btnSingleton.Click += new System.EventHandler(this.btnSingleton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnTransient);
            this.groupBox2.Controls.Add(this.btnSingleton);
            this.groupBox2.Location = new System.Drawing.Point(258, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(157, 74);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Lifetime Manager Samples";
            // 
            // btnNonregistered
            // 
            this.btnNonregistered.Location = new System.Drawing.Point(0, 19);
            this.btnNonregistered.Name = "btnNonregistered";
            this.btnNonregistered.Size = new System.Drawing.Size(85, 38);
            this.btnNonregistered.TabIndex = 11;
            this.btnNonregistered.Text = "Nonregistered";
            this.btnNonregistered.UseVisualStyleBackColor = true;
            this.btnNonregistered.Click += new System.EventHandler(this.btnNonregistered_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnCtorInjection);
            this.groupBox3.Controls.Add(this.btnInjectDependencies);
            this.groupBox3.Controls.Add(this.btnNonregistered);
            this.groupBox3.Location = new System.Drawing.Point(420, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(293, 74);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Dependencies Injection";
            // 
            // btnCtorInjection
            // 
            this.btnCtorInjection.Location = new System.Drawing.Point(201, 20);
            this.btnCtorInjection.Name = "btnCtorInjection";
            this.btnCtorInjection.Size = new System.Drawing.Size(90, 37);
            this.btnCtorInjection.TabIndex = 13;
            this.btnCtorInjection.Text = "Constructor Injection";
            this.btnCtorInjection.UseVisualStyleBackColor = true;
            this.btnCtorInjection.Click += new System.EventHandler(this.btnCtorInjection_Click);
            // 
            // btnInjectDependencies
            // 
            this.btnInjectDependencies.Location = new System.Drawing.Point(92, 19);
            this.btnInjectDependencies.Name = "btnInjectDependencies";
            this.btnInjectDependencies.Size = new System.Drawing.Size(102, 38);
            this.btnInjectDependencies.TabIndex = 12;
            this.btnInjectDependencies.Text = "Inject Dependencies";
            this.btnInjectDependencies.UseVisualStyleBackColor = true;
            this.btnInjectDependencies.Click += new System.EventHandler(this.btnInjectDependencies_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.Location = new System.Drawing.Point(82, 46);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(75, 23);
            this.btnConfig.TabIndex = 13;
            this.btnConfig.Text = "Config";
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // Form0
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 319);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(groupBox1);
            this.Controls.Add(this.textBox1);
            this.Name = "Form0";
            this.Text = "Form0";
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAge)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEvilAcme;
        private System.Windows.Forms.Button btnChineseAcme;
        private System.Windows.Forms.Button btnOldAcme;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnTransient;
        private System.Windows.Forms.Button btnSingleton;
        private System.Windows.Forms.GroupBox groupBox2;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private System.Windows.Forms.NumericUpDown numericUpDownAge;
        private System.Windows.Forms.Button btnNonregistered;
        private System.Windows.Forms.Button btnResolveAll;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnInjectDependencies;
        private System.Windows.Forms.Button btnCtorInjection;
        private System.Windows.Forms.Button btnConfig;
    }
}

