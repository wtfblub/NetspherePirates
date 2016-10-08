namespace NetsphereExplorer.Views
{
    partial class FileOperationView
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
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new BlubLib.GUI.Controls.Extended.LabelEx();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblDestination = new BlubLib.GUI.Controls.Extended.LinkLabelEx();
            this.lblProgress = new BlubLib.GUI.Controls.Extended.LabelEx();
            this.pbProgress = new BlubLib.GUI.Controls.Extended.ProgressBarEx();
            this.btnPause = new NetsphereExplorer.Controls.ImageButton();
            this.btnCancel = new NetsphereExplorer.Controls.ImageButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.Image = null;
            this.lblTitle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitle.LabelStyle = BlubLib.GUI.Controls.Extended.LabelStyle.BodyText;
            this.lblTitle.Location = new System.Drawing.Point(3, 5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(107, 15);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Copying 0 items to";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.lblTitle);
            this.flowLayoutPanel1.Controls.Add(this.lblDestination);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(349, 27);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // lblDestination
            // 
            this.lblDestination.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblDestination.AutoSize = true;
            this.lblDestination.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblDestination.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDestination.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblDestination.Image = null;
            this.lblDestination.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDestination.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lblDestination.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblDestination.Location = new System.Drawing.Point(116, 5);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(50, 15);
            this.lblDestination.TabIndex = 3;
            this.lblDestination.TabStop = true;
            this.lblDestination.Text = "Desktop";
            this.lblDestination.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProgress.AutoSize = true;
            this.lblProgress.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblProgress.ForeColor = System.Drawing.Color.Black;
            this.lblProgress.Image = null;
            this.lblProgress.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblProgress.LabelStyle = BlubLib.GUI.Controls.Extended.LabelStyle.BodyTitle;
            this.lblProgress.Location = new System.Drawing.Point(6, 52);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(87, 15);
            this.lblProgress.TabIndex = 1;
            this.lblProgress.Text = "10% complete";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.Location = new System.Drawing.Point(9, 70);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(333, 17);
            this.pbProgress.State = BlubLib.Native.ProgressBarState.Normal;
            this.pbProgress.TabIndex = 2;
            // 
            // btnPause
            // 
            this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPause.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnPause.Image = global::NetsphereExplorer.Properties.Resources.control_pause;
            this.btnPause.ImageSize = new System.Drawing.Size(16, 16);
            this.btnPause.Location = new System.Drawing.Point(288, 40);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(24, 24);
            this.btnPause.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnCancel.Image = global::NetsphereExplorer.Properties.Resources.control_stop_square;
            this.btnCancel.ImageSize = new System.Drawing.Size(16, 16);
            this.btnCancel.Location = new System.Drawing.Point(318, 40);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(24, 24);
            this.btnCancel.TabIndex = 2;
            // 
            // ExtractProgressView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ExtractProgressView";
            this.Size = new System.Drawing.Size(355, 103);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BlubLib.GUI.Controls.Extended.LabelEx lblTitle;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private BlubLib.GUI.Controls.Extended.LinkLabelEx lblDestination;
        private BlubLib.GUI.Controls.Extended.LabelEx lblProgress;
        private BlubLib.GUI.Controls.Extended.ProgressBarEx pbProgress;
        private Controls.ImageButton btnCancel;
        private Controls.ImageButton btnPause;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
