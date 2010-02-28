namespace CC.Votd.TestTool
{
    partial class FormMain
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
            this._PanelMiniPreview = new System.Windows.Forms.Panel();
            this._ButtonPreview = new System.Windows.Forms.Button();
            this._ButtonSettings = new System.Windows.Forms.Button();
            this._CheckBoxDebug = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _PanelMiniPreview
            // 
            this._PanelMiniPreview.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._PanelMiniPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._PanelMiniPreview.Location = new System.Drawing.Point(29, 41);
            this._PanelMiniPreview.Name = "_PanelMiniPreview";
            this._PanelMiniPreview.Size = new System.Drawing.Size(155, 115);
            this._PanelMiniPreview.TabIndex = 0;
            // 
            // _ButtonPreview
            // 
            this._ButtonPreview.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._ButtonPreview.Location = new System.Drawing.Point(112, 173);
            this._ButtonPreview.Name = "_ButtonPreview";
            this._ButtonPreview.Size = new System.Drawing.Size(86, 23);
            this._ButtonPreview.TabIndex = 1;
            this._ButtonPreview.Text = "&Preview";
            this._ButtonPreview.UseVisualStyleBackColor = true;
            this._ButtonPreview.Click += new System.EventHandler(this._ButtonPreview_Click);
            // 
            // _ButtonSettings
            // 
            this._ButtonSettings.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._ButtonSettings.Location = new System.Drawing.Point(15, 173);
            this._ButtonSettings.Name = "_ButtonSettings";
            this._ButtonSettings.Size = new System.Drawing.Size(86, 23);
            this._ButtonSettings.TabIndex = 2;
            this._ButtonSettings.Text = "&Settings";
            this._ButtonSettings.UseVisualStyleBackColor = true;
            this._ButtonSettings.Click += new System.EventHandler(this._ButtonSettings_Click);
            // 
            // _CheckBoxDebug
            // 
            this._CheckBoxDebug.AutoSize = true;
            this._CheckBoxDebug.Location = new System.Drawing.Point(56, 12);
            this._CheckBoxDebug.Name = "_CheckBoxDebug";
            this._CheckBoxDebug.Size = new System.Drawing.Size(94, 17);
            this._CheckBoxDebug.TabIndex = 3;
            this._CheckBoxDebug.Text = "Enable &Debug";
            this._CheckBoxDebug.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(213, 208);
            this.Controls.Add(this._CheckBoxDebug);
            this.Controls.Add(this._ButtonSettings);
            this.Controls.Add(this._ButtonPreview);
            this.Controls.Add(this._PanelMiniPreview);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "CC.Votd.TestTool";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel _PanelMiniPreview;
        private System.Windows.Forms.Button _ButtonPreview;
        private System.Windows.Forms.Button _ButtonSettings;
        private System.Windows.Forms.CheckBox _CheckBoxDebug;
    }
}

