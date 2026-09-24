using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace LabelPrint
{
    public partial class MmImageRulerViewer : ScrollableControl
    {
        private Image originalImage;
        private Image _image;
        private float _mmWidth = .01f;   // Desired width in millimeters
        private float _mmHeight = .01f;  // Desired height in millimeters
        private float dpi = 203;
        private const int RulerSize = 30; // Pixel size reserved for ruler bars
        
        /// <summary>
        /// Display's horizontal dpi
        /// </summary>
        private float dpiY = 203;

        public event EventHandler<EventArgs> DPIUpdated;

        public MmImageRulerViewer() { }

        private void OnDPIUpdated()
        {
            DPIUpdated?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Deletes image from preview
        /// </summary>
        public void ClearImage()
        {
            _image = null;
            UpdateScrollBounds();
        }

        /// <summary>
        /// Loads an image from disk and sets initial target dimensions in mm based on image DPI.
        /// </summary>
        public void LoadImage(string filePath)
        {
            _image?.Dispose();

            using (Bitmap original = new Bitmap(filePath))
            {
                // Converts source image to 1-bit monochrome indexed bitmap
                using (Bitmap monoImg = original.Clone(
                    new Rectangle(0, 0, original.Width, original.Height),
                    PixelFormat.Format1bppIndexed))
                {
                    monoImg.SetResolution(dpi, dpi);
                    originalImage = _image = monoImg.Clone(new Rectangle(0, 0, original.Width, original.Height), 
                        PixelFormat.Format1bppIndexed);

                    //pixelSize = new Size(originalImage.Width, originalImage.Height);
                }
            }
            // Convert natural pixel size to physical mm based on original DPI metadata
            _mmWidth = (_image.Width / _image.HorizontalResolution) * 25.4f;
            _mmHeight = (_image.Height / _image.VerticalResolution) * 25.4f;

            MessageBox.Show($"Size: {_image.Size}");
            MessageBox.Show($"DPI: H:{_image.HorizontalResolution}; V:{_image.VerticalResolution}");

            UpdateScrollBounds();
            Invalidate();
        }

        /// <summary>
        /// Get current image scale (width x height) in millimeters
        /// </summary>
        /// <returns></returns>
        public float[] GetImageScaleMm()
        {
            float[] returns = { _mmWidth, _mmHeight };
            return returns;
        }

        /// <summary>
        /// Explicitly scale the displayed image dimensions in millimeters.
        /// </summary>
        public void SetImageScaleMm(float widthMm, float heightMm)
        {
            _mmWidth = widthMm;
            _mmHeight = heightMm;

            UpdateScrollBounds();
            Invalidate();
        }

        /// <summary>
        /// Updates borders of the scroll zone (to which pixel value can user scroll)
        /// </summary>
        private void UpdateScrollBounds()
        {
            using (Graphics g = CreateGraphics())
            {
                float pxPerMmX = 203 / 25.4f * .05f;
                float pxPerMmY = 203 / 25.4f * .05f;

                // Total scrollable area includes the image size plus ruler margins
                int totalWidth = (int)(_mmWidth * pxPerMmX * 10) + RulerSize;
                int totalHeight = (int)(_mmHeight * pxPerMmY * 10) + RulerSize;

                AutoScrollMinSize = new Size(totalWidth, totalHeight);

                PerformLayout();
                Invalidate();
            }
        }

        /// <summary>
        /// Handles scrolling (vertically & horizontally) by mouse & touchpad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (_image != null && (int)_mmHeight * (dpiY / 25.4f) >= Height - RulerSize)
            {
                // Suppress standard control wheel handling to prevent double-scrolling
                if (e is HandledMouseEventArgs hme)
                {
                    hme.Handled = true;
                }

                // AutoScrollPosition getter returns negative values, so Math.Abs gets the positive pixel offset
                int currentX = Math.Abs(AutoScrollPosition.X);
                int currentY = Math.Abs(AutoScrollPosition.Y);

                // e.Delta is positive (+120) when scrolling up/away, negative (-120) when scrolling down/towards
                int scrollDelta = e.Delta;

                if (ModifierKeys.HasFlag(Keys.Shift))
                {
                    // Shift + Mouse Wheel -> Horizontal Scrolling
                    int newX = currentX - scrollDelta;
                    AutoScrollPosition = new Point(newX, currentY);
                }
                else
                {
                    // Standard Mouse Wheel -> Vertical Scrolling
                    int newY = currentY - scrollDelta;
                    AutoScrollPosition = new Point(currentX, newY);
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Does nothing -> screen doesn't flicker when scrolling
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do nothing - background clearing is handled in OnPaint off-screen
        }

        /// <summary>
        /// Paints the whole control on screen
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            dpiY = g.DpiY;

            g.Clear(BackColor);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.Bilinear;

            // Current scroll offsets (AutoScrollPosition values are negative)
            int scrollX = Math.Abs(AutoScrollPosition.X);
            int scrollY = Math.Abs(AutoScrollPosition.Y);

            // DPI is calculated wrong: bigger DPI -> smaller image
            // Currently:               bigger DPI -> bigger image

            // Calculate screen pixels per millimeter with constant (dpi is used in _mmWidth)
            float pxPerMmX = 203 / 25.4f * .05f;
            float pxPerMmY = 203 / 25.4f * .05f;

            // Convert target mm dimensions to pixel dimensions for drawing
            // Error in calculations might be here
            float imgPxWidth = _mmWidth * pxPerMmX * 10; // 254dpi, 100mm - 500 // 508dpi, 100mm - 
            float imgPxHeight = _mmHeight * pxPerMmY * 10;

            // 1. Draw Image (Anchored at top-left corner past the rulers)
            if (_image != null)
            {
                Rectangle contentArea = new Rectangle(RulerSize, RulerSize, Width - RulerSize, Height - RulerSize);
                Region originalClip = g.Clip;
                g.SetClip(contentArea);

                float imgX = RulerSize - scrollX;
                float imgY = RulerSize - scrollY;
                RectangleF imageRect = new RectangleF(imgX, imgY, imgPxWidth, imgPxHeight);

                g.DrawImage(_image, imageRect);
                g.Clip = originalClip;
            }

            // 2. Draw Rulers
            DrawRulers(g, pxPerMmX, pxPerMmY, imgPxWidth, imgPxHeight, scrollX, scrollY);
        }

        /// <summary>
        /// Draws mm rulers
        /// </summary>
        /// <param name="g">Control's graphics</param>
        /// <param name="pxPerMmX">Pixels per mm horizontally</param>
        /// <param name="pxPerMmY">Pixels per mm vertically</param>
        /// <param name="imgPxWidth">Image width in pixels</param>
        /// <param name="imgPxHeight">Image height in pixels</param>
        /// <param name="scrollX">Horizontal scroll value in pixels</param>
        /// <param name="scrollY">Vertical scroll value in pixels</param>
        private void DrawRulers(Graphics g, float pxPerMmX, float pxPerMmY, float imgPxWidth, float imgPxHeight, int scrollX, int scrollY)
        {
            using (SolidBrush rulerBg = new SolidBrush(Color.White))
            using (Pen linePen = new Pen(Color.DarkSlateGray, 1))
            using (Font textFont = new Font("Segoe UI", 7f))
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                // Fill Ruler Backgrounds
                g.FillRectangle(rulerBg, RulerSize, 0, Width - RulerSize, RulerSize); // Top Ruler
                g.FillRectangle(rulerBg, 0, RulerSize, RulerSize, Height - RulerSize); // Left Ruler

                // --- Horizontal Ruler (Top) ---
                Region originalClip = g.Clip;

                // --- Horizontal Ruler (Top) ---
                g.SetClip(new Rectangle(RulerSize, 0, Width - RulerSize, RulerSize));
                
                // Start position in mm (0 - scroll in mm)
                int startMmX = Math.Max(0, (int)(scrollX / pxPerMmX));
                
                // End position in mm (scroll in mm + clean width in mm)
                int endMmX = (int)((scrollX + Width - RulerSize) / pxPerMmX);

                // Image width - Release version
                int maxMmX = _image == null || (int)_mmWidth < Width - RulerSize ? endMmX : (int)_mmWidth;

                // Starts at start pos, ends at end position or Image width - whatever is less
                for (int mm = startMmX; mm <= Math.Min(endMmX, maxMmX) * 10; mm++)
                {
                    float x = RulerSize + (mm * pxPerMmX) - scrollX;

                    if (mm % 100 == 0) // Major tick (10mm)
                    {
                        g.DrawLine(linePen, x, 10, x, RulerSize);
                        g.DrawString((mm / 10).ToString(), textFont, textBrush, x + 1, 1);
                    }
                    else if (mm % 50 == 0) // Medium tick (5mm)
                    {
                        g.DrawLine(linePen, x, 18, x, RulerSize);
                    }
                    else if (mm % 10 == 0) // Minor tick (1mm)
                    {
                        g.DrawLine(linePen, x, 23, x, RulerSize);
                    }
                }

                // --- Vertical Ruler (Left) ---

                g.SetClip(new Rectangle(0, RulerSize, RulerSize, Height - RulerSize));

                int startMmY = Math.Max(0, (int)(scrollY / pxPerMmY));
                int endMmY = (int)((scrollY + Height - RulerSize) / pxPerMmY);

                // Image Height - Release version
                int maxMmY = _image == null || (int)_mmHeight < Height - RulerSize ? endMmY : (int)_mmHeight;

                for (int mm = startMmY; mm <= Math.Min(endMmY, maxMmY) * 10; mm++)
                {
                    float y = RulerSize + (mm * pxPerMmY) - scrollY;

                    if (mm % 100 == 0) // Major tick (100mm)
                    {
                        g.DrawLine(linePen, 10, y, RulerSize, y);
                        g.DrawString((mm / 10).ToString(), textFont, textBrush, 1, y + 1);
                    }
                    else if (mm % 50 == 0) // Medium tick (50mm)
                    {
                        g.DrawLine(linePen, 18, y, RulerSize, y);
                    }
                    else if (mm % 10 == 0) // Minor tick (10mm)
                    {
                        g.DrawLine(linePen, 23, y, RulerSize, y);
                    }
                }

                g.Clip = originalClip;

                // --- Corner Square (Anchor Intersection at 0,0) ---
                g.FillRectangle(Brushes.LightGray, 0, 0, RulerSize, RulerSize);
                g.DrawRectangle(linePen, 0, 0, RulerSize, RulerSize);
                g.DrawString("mm", textFont, textBrush, 4, 8);

                // Outer Ruler Separation Lines
                g.DrawLine(linePen, RulerSize, 0, RulerSize, Height);
                g.DrawLine(linePen, 0, RulerSize, Width, RulerSize);
            }
        }

        /// <summary>
        /// Enables OS-level composite double buffering
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }

        public Image Image { get { return _image; } }

        public Image OriginalImage { get { return originalImage; } }

        public float MmWidth { get { return _mmWidth; } }
        public float MmHeight { get { return _mmHeight; } }

        /// <summary>
        /// On set changes image dpi & its size
        /// </summary>
        public float DPI 
        { 
            get { return dpi; }
            set
            {
                if (value > 0 && _image != null)
                {
                    // Makes new Size from current dimensions
                    float[] newSizes = { (_mmWidth / 25.4f) * dpi, (_mmHeight / 25.4f) * dpi };

                    dpi = value;

                    // Applies DPI
                    // Changes mm: px = (mm / 25.4) * dpi -> mm = (px / dpi) * 25.4
                    _mmWidth = Math.Clamp((newSizes[0] / dpi) * 25.4f, 0.01f, 1000);
                    _mmHeight = Math.Clamp((newSizes[1] / dpi) * 25.4f, 0.01f, 1000);

                    using (Bitmap imageBM = (Bitmap)_image)
                    {
                        using (Bitmap changedDPI = new Bitmap(imageBM))
                        {
                            changedDPI.SetResolution(dpi, dpi);
                            _image = changedDPI.Clone(new Rectangle(0, 0, changedDPI.Width, changedDPI.Height), PixelFormat.Format1bppIndexed);
                        }
                    }

                    UpdateScrollBounds();
                    Invalidate();

                    OnDPIUpdated();
                }
            }
        }
    }
}
