﻿namespace Project1
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
            this.PictureBoxHistogram = new System.Windows.Forms.PictureBox();
            this.PictureBoxQuantized = new System.Windows.Forms.PictureBox();
            this.ProgressBarQuantization = new System.Windows.Forms.ProgressBar();
            this.LabelColorDistance = new System.Windows.Forms.Label();
            this.PictureBoxQuantized2 = new System.Windows.Forms.PictureBox();
            this.ProgressBarQuantization2 = new System.Windows.Forms.ProgressBar();
            this.LabelColorDistance2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxLoadedImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxQuantized)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxQuantized2)).BeginInit();
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
            this.OpenFileDialogImageLoader.Filter = "Bitmap files|*.bmp|PNG files|*.png";
            this.OpenFileDialogImageLoader.InitialDirectory = ".";
            // 
            // PictureBoxHistogram
            // 
            this.PictureBoxHistogram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PictureBoxHistogram.Location = new System.Drawing.Point(622, 37);
            this.PictureBoxHistogram.Name = "PictureBoxHistogram";
            this.PictureBoxHistogram.Size = new System.Drawing.Size(497, 347);
            this.PictureBoxHistogram.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBoxHistogram.TabIndex = 3;
            this.PictureBoxHistogram.TabStop = false;
            // 
            // PictureBoxQuantized
            // 
            this.PictureBoxQuantized.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PictureBoxQuantized.Location = new System.Drawing.Point(12, 414);
            this.PictureBoxQuantized.Name = "PictureBoxQuantized";
            this.PictureBoxQuantized.Size = new System.Drawing.Size(497, 347);
            this.PictureBoxQuantized.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBoxQuantized.TabIndex = 4;
            this.PictureBoxQuantized.TabStop = false;
            // 
            // ProgressBarQuantization
            // 
            this.ProgressBarQuantization.Location = new System.Drawing.Point(12, 388);
            this.ProgressBarQuantization.Name = "ProgressBarQuantization";
            this.ProgressBarQuantization.Size = new System.Drawing.Size(497, 23);
            this.ProgressBarQuantization.Step = 2;
            this.ProgressBarQuantization.TabIndex = 5;
            // 
            // LabelColorDistance
            // 
            this.LabelColorDistance.AutoSize = true;
            this.LabelColorDistance.Location = new System.Drawing.Point(12, 777);
            this.LabelColorDistance.Name = "LabelColorDistance";
            this.LabelColorDistance.Size = new System.Drawing.Size(0, 13);
            this.LabelColorDistance.TabIndex = 6;
            // 
            // PictureBoxQuantized2
            // 
            this.PictureBoxQuantized2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PictureBoxQuantized2.Location = new System.Drawing.Point(622, 414);
            this.PictureBoxQuantized2.Name = "PictureBoxQuantized2";
            this.PictureBoxQuantized2.Size = new System.Drawing.Size(497, 347);
            this.PictureBoxQuantized2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBoxQuantized2.TabIndex = 7;
            this.PictureBoxQuantized2.TabStop = false;
            // 
            // ProgressBarQuantization2
            // 
            this.ProgressBarQuantization2.Location = new System.Drawing.Point(622, 388);
            this.ProgressBarQuantization2.Name = "ProgressBarQuantization2";
            this.ProgressBarQuantization2.Size = new System.Drawing.Size(497, 23);
            this.ProgressBarQuantization2.Step = 2;
            this.ProgressBarQuantization2.TabIndex = 8;
            // 
            // LabelColorDistance2
            // 
            this.LabelColorDistance2.AutoSize = true;
            this.LabelColorDistance2.Location = new System.Drawing.Point(619, 777);
            this.LabelColorDistance2.Name = "LabelColorDistance2";
            this.LabelColorDistance2.Size = new System.Drawing.Size(0, 13);
            this.LabelColorDistance2.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1142, 802);
            this.Controls.Add(this.LabelColorDistance2);
            this.Controls.Add(this.ProgressBarQuantization2);
            this.Controls.Add(this.PictureBoxQuantized2);
            this.Controls.Add(this.LabelColorDistance);
            this.Controls.Add(this.ProgressBarQuantization);
            this.Controls.Add(this.PictureBoxQuantized);
            this.Controls.Add(this.PictureBoxHistogram);
            this.Controls.Add(this.LabelPath);
            this.Controls.Add(this.ButtonLoadImage);
            this.Controls.Add(this.PictureBoxLoadedImage);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxLoadedImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxQuantized)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxQuantized2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PictureBoxLoadedImage;
        private System.Windows.Forms.Button ButtonLoadImage;
        private System.Windows.Forms.Label LabelPath;
        private System.Windows.Forms.OpenFileDialog OpenFileDialogImageLoader;
        private System.Windows.Forms.PictureBox PictureBoxHistogram;
        private System.Windows.Forms.PictureBox PictureBoxQuantized;
        private System.Windows.Forms.ProgressBar ProgressBarQuantization;
        private System.Windows.Forms.Label LabelColorDistance;
        private System.Windows.Forms.PictureBox PictureBoxQuantized2;
        private System.Windows.Forms.ProgressBar ProgressBarQuantization2;
        private System.Windows.Forms.Label LabelColorDistance2;
    }
}
