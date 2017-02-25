using System.Drawing;
using BlubLib.GUI;
using View = BlubLib.GUI.Controls.View;

namespace NetsphereExplorer.Views
{
    internal partial class SimpleInfoView : View
    {
        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                InfoLabel.InvokeIfRequired(() =>
                {
                    InfoLabel.Text = _message;
                    SetLocation();
                });
            }
        }

        public SimpleInfoView(string message)
        {
            InitializeComponent();
            Message = message;
        }

        protected override void OnActivate()
        {
            SetLocation();
        }

        private void SetLocation()
        {
            InfoLabel.Location = new Point(Width / 2 - InfoLabel.Width / 2, Height / 2 - InfoLabel.Height / 2);
        }
    }
}
