namespace NetsphereExplorer.Views
{
    partial class SimpleInfoView
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
            this.InfoLabel.Location = new System.Drawing.Point(13, 11);
            this.InfoLabel.Name = "InfoLabel";
            this.InfoLabel.Size = new System.Drawing.Size(126, 16);
            this.InfoLabel.TabIndex = 0;
            this.InfoLabel.Text = "Checking version...";
            this.InfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // SimpleInfoView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.InfoLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SimpleInfoView";
            this.Size = new System.Drawing.Size(250, 40);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BlubLib.GUI.Controls.Extended.LabelEx InfoLabel;
    }
}
