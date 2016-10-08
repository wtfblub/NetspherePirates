using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using BlubLib.GUI.Controls;
using View = BlubLib.GUI.Controls.View;

namespace NetsphereExplorer.Controls
{
    internal class Overlay : Component
    {
        private readonly SolidBrush _fadeColor;
        private readonly ViewHost _content = new ViewHost();
        private readonly IList<Control> _controlsToRestore = new List<Control>();
        private View _view;

        public View View
        {
            get { return _view; }
            set
            {
                _view = value;
                if (Owner.Controls.Contains(_content))
                {
                    _content.View = value;
                    SetLocation();
                }
            }
        }
        public Form Owner { get; set; }
        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;
                var host = value?.GetService<IDesignerHost>();
                if (host != null)
                {
                    var form = host.RootComponent as Form;
                    Owner = form;
                }
            }
        }
        public bool IsShowing => _content.View != null;

        public Overlay()
        {
            _fadeColor = new SolidBrush(Color.FromArgb(80, Color.Black));
            _content.Dock = DockStyle.Fill;
        }

        public void Show(View view)
        {
            View = view;
            Show();
        }

        public void Show()
        {
            LoadBackground();
            _content.View = View;

            foreach (Control control in Owner.Controls)
            {
                if (control.Enabled)
                {
                    control.Enabled = false;
                    _controlsToRestore.Add(control);
                }
            }

            Owner.Controls.Add(_content);
            SetLocation();
            _content.BringToFront();
        }

        public void Hide()
        {
            foreach (var control in _controlsToRestore)
                control.Enabled = true;
            _controlsToRestore.Clear();

            Owner.Controls.Remove(_content);
            _content.View = null;
            var image = _content.BackgroundImage;
            _content.BackgroundImage = null;
            image?.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fadeColor.Dispose();
                _content.Dispose();
            }
            base.Dispose(disposing);
        }

        private void LoadBackground()
        {
            using (var tmp = new Bitmap(Owner.Width, Owner.Height, PixelFormat.Format32bppArgb))
            {
                Owner.DrawToBitmap(tmp, new Rectangle(0, 0, tmp.Width, tmp.Height));

                var origin = Owner.PointToScreen(new Point(0, 0));
                var diffX = origin.X - Owner.Left;
                var diffY = origin.Y - Owner.Top;

                var image = new Bitmap(Owner.ClientSize.Width, Owner.ClientSize.Height);
                using (var g = Graphics.FromImage(image))
                {
                    g.DrawImage(tmp, 0, 0, new Rectangle(diffX, diffY, image.Width, image.Height), GraphicsUnit.Pixel);
                    g.FillRectangle(_fadeColor, 0, 0, image.Width, image.Height);
                }

                _content.Size = Owner.Size;
                _content.BackgroundImage = image;
            }
        }

        private void SetLocation()
        {
            if (_view == null)
                return;

            var x = _content.Width / 2 - _view.Width / 2;
            var y = _content.Height / 2 - _view.Height / 2;
            _view.Location = new Point(x, y);
        }
    }
}