namespace NetsphereExplorer.Views
{
    partial class SimpleProgressView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.InfoLabel = new BlubLib.GUI.Controls.Extended.LabelEx();
            this.ProgressBar = new BlubLib.GUI.Controls.Extended.ProgressBarEx();
            this.SuspendLayout();
            // 
            // InfoLabel
            // 
            this.InfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.InfoLabel.AutoSize = true;
            this.InfoLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.InfoLabel.ForeColor = System.Drawing.Color.Black;
            this.InfoLabel.Image = global::NetsphereExplorer.Properties.Resources.ajax_loader;
            this.InfoLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.InfoLabel.LabelStyle = BlubLib.GUI.Controls.Extended.LabelStyle.BodyText;
            this.InfoLabel.Location = new System.Drawing.Point(6, 6);
            this.InfoLabel.Name = "InfoLabel";
            this.InfoLabel.Size = new System.Drawing.Size(126, 16);
            this.InfoLabel.TabIndex = 1;
            this.InfoLabel.Text = "Checking version...";
            this.InfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(6, 32);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(337, 16);
            this.ProgressBar.State = BlubLib.WinAPI.ProgressBarState.Normal;
            this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressBar.TabIndex = 2;
            // 
            // SimpleProgressView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.InfoLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SimpleProgressView";
            this.Size = new System.Drawing.Size(346, 54);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BlubLib.GUI.Controls.Extended.LabelEx InfoLabel;
        private BlubLib.GUI.Controls.Extended.ProgressBarEx ProgressBar;
    }
}
