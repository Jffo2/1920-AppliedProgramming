namespace Project1
{
    partial class Form1
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
            this.PictureBoxLoadedImage = new System.Windows.Forms.PictureBox();
            this.ButtonLoadImage = new System.Windows.Forms.Button();
            this.LabelPath = new System.Windows.Forms.Label();
            this.OpenFileDialogImageLoader = new System.Windows.Forms.OpenFileDialog();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxLoadedImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // PictureBoxLoadedImage
            // 
            this.PictureBoxLoadedImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PictureBoxLoadedImage.Location = new System.Drawing.Point(12, 37);
            this.PictureBoxLoadedImage.Name = "PictureBoxLoadedImage";
            this.PictureBoxLoadedImage.Size = new System.Drawing.Size(497, 347);
            this.PictureBoxLoadedImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBoxLoadedImage.TabIndex = 0;
            this.PictureBoxLoadedImage.TabStop = false;
            // 
            // ButtonLoadImage
            // 
            this.ButtonLoadImage.Location = new System.Drawing.Point(434, 8);
            this.ButtonLoadImage.Name = "ButtonLoadImage";
            this.ButtonLoadImage.Size = new System.Drawing.Size(75, 23);
            this.ButtonLoadImage.TabIndex = 1;
            this.ButtonLoadImage.Text = "Load Image";
            this.ButtonLoadImage.UseVisualStyleBackColor = true;
            this.ButtonLoadImage.Click += new System.EventHandler(this.ButtonLoadImage_Click);
            // 
            // LabelPath
            // 
            this.LabelPath.AutoSize = true;
            this.LabelPath.Location = new System.Drawing.Point(12, 13);
            this.LabelPath.Name = "LabelPath";
            this.LabelPath.Size = new System.Drawing.Size(28, 13);
            this.LabelPath.TabIndex = 2;
            this.LabelPath.Text = "path";
            // 
            // OpenFileDialogImageLoader
            // 
            this.OpenFileDialogImageLoader.Filter = "Bitmap files|*.bmp";
            this.OpenFileDialogImageLoader.InitialDirectory = ".";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(622, 37);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(497, 347);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1142, 773);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.LabelPath);
            this.Controls.Add(this.ButtonLoadImage);
            this.Controls.Add(this.PictureBoxLoadedImage);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxLoadedImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PictureBoxLoadedImage;
        private System.Windows.Forms.Button ButtonLoadImage;
        private System.Windows.Forms.Label LabelPath;
        private System.Windows.Forms.OpenFileDialog OpenFileDialogImageLoader;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

