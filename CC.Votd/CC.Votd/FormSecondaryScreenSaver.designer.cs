namespace CC.Votd
{
    partial class FormSecondaryScreenSaver
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
            this.SuspendLayout();
            // 
            // FormSecondaryScreenSaver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::CC.Votd.Properties.Resources.Cross;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSecondaryScreenSaver";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormSecondaryScreenSaver";
            this.TopMost = true;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormSecondaryScreenSaver_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormSecondaryScreenSaver_MouseMove);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormSecondaryScreenSaver_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}