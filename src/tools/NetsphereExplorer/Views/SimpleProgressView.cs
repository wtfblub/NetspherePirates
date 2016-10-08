using System;
using System.Drawing;
using System.Windows.Forms;
using View = BlubLib.GUI.Controls.View;

namespace NetsphereExplorer.Views
{
    internal partial class SimpleProgressView : View, IProgress<int>
    {
        public string Message
        {
            get { return InfoLabel.Text; }
            set
            {
                InfoLabel.InvokeIfRequired(() =>
                {
                    InfoLabel.Text = value;
                    SetLocation();
                });
            }
        }
        public int Progress
        {
            get { return ProgressBar.Value; }
            set
            {
                ProgressBar.InvokeIfRequired(() =>
                {
                    ProgressBar.Value = value;
                });
            }
        }

        public SimpleProgressView()
        {
            InitializeComponent();
        }

        protected override void OnActivate()
        {
            SetLocation();
        }

        private void SetLocation()
        {
            InfoLabel.Location = new Point(Width / 2 - InfoLabel.Width / 2, InfoLabel.Location.Y);
        }

        public void Report(int value)
        {
            Progress = value;
        }

        public void Status(string message)
        {
            Message = message;
        }
    }
}
