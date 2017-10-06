using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace NetsphereExplorer.Controls
{
    internal sealed class ImageButton : UserControl
    {
        private readonly VisualStyleRenderer _hoverRenderer;
        private readonly VisualStyleRenderer _pressedRenderer;
        private ButtonState _state;
        private Size _imageSize;
        private Image _image;

        public Image Image
        {
            get => _image;
            set
            {
                _image = value;
                Invalidate();
            }
        }
        public Size ImageSize
        {
            get => _imageSize;
            set
            {
                _imageSize = value;
                Invalidate();
            }
        }

        public ImageButton()
        {
            if (!VisualStyleRenderer.IsElementDefined(VisualStyleElement.Button.PushButton.Hot) ||
                !VisualStyleRenderer.IsElementDefined(VisualStyleElement.Button.PushButton.Pressed))
                throw new NotSupportedException("This control is not supported on this operating system");

            _hoverRenderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Hot);
            _pressedRenderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Pressed);
            Cursor = Cursors.Default;
            Width = 30;
            Height = 30;
            Image = Properties.Resources.document__plus;
            ImageSize = new Size(14, 14);
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            BackgroundImageLayout = ImageLayout.Center;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            switch (_state)
            {
                case ButtonState.Hover:
                    _hoverRenderer.DrawBackground(e.Graphics, ClientRectangle);
                    break;

                case ButtonState.Pressed:
                    _pressedRenderer.DrawBackground(e.Graphics, ClientRectangle);
                    break;
            }

            var imageRect = new Rectangle(Width - Width / 2 - ImageSize.Width / 2, Height / 2 - ImageSize.Height / 2, ImageSize.Width, ImageSize.Height);
            e.Graphics.DrawImage(Image, imageRect);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _state = ButtonState.None;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_state != ButtonState.Pressed)
            {
                _state = ButtonState.Pressed;
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_state == ButtonState.Pressed)
            {
                _state = ButtonState.None;
                Invalidate();
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_state != ButtonState.Pressed && _state != ButtonState.Hover)
            {
                _state = ButtonState.Hover;
                Invalidate();
            }
            base.OnMouseMove(e);
        }

        private enum ButtonState
        {
            None,
            Hover,
            Pressed
        }
    }
}
