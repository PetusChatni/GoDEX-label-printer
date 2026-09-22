using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace LabelPrint
{
    public partial class MmImageRulerViewer : ScrollableControl
    {
        private Image _image;
        private float _mmWidth = 100f;   // Desired width in millimeters
        private float _mmHeight = 100f;  // Desired height in millimeters
        private const int RulerSize = 30; // Pixel size reserved for ruler bars

        public MmImageRulerViewer() { }

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
                    monoImg.SetResolution(203, 203);
                    _image = monoImg.Clone(new Rectangle(0, 0, original.Width, original.Height), 
                        PixelFormat.Format1bppIndexed);
                }
            }

            //_image = Image.FromFile(filePath);

            // Convert natural pixel size to physical mm based on original DPI metadata
            _mmWidth = (_image.Width / _image.HorizontalResolution) * 25.4f;
            _mmHeight = (_image.Height / _image.VerticalResolution) * 25.4f;

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

        private void UpdateScrollBounds()
        {
            using (Graphics g = CreateGraphics())
            {
                float pxPerMmX = 203 / 25.4f;
                float pxPerMmY = 203 / 25.4f;

                // Total scrollable area includes the image size plus ruler margins
                int totalWidth = (int)(_mmWidth * pxPerMmX) + RulerSize;
                int totalHeight = (int)(_mmHeight * pxPerMmY) + RulerSize;

                AutoScrollMinSize = new Size(totalWidth, totalHeight);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Current scroll offsets (AutoScrollPosition values are negative)
            int scrollX = Math.Abs(AutoScrollPosition.X);
            int scrollY = Math.Abs(AutoScrollPosition.Y);

            // Calculate screen pixels per centimeter for current monitor
            float pxPerMmX = 203 / 25.4f * .05f;
            float pxPerMmY = 203 / 25.4f * .05f;

            // Convert target mm dimensions to pixel dimensions for drawing
            float imgPxWidth = _mmWidth * pxPerMmX * 10;
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

                //RectangleF imageRect = new RectangleF(RulerSize, RulerSize, imgPxWidth, imgPxHeight);
                //g.DrawImage(_image, imageRect);

                // Optional: Draw border around image bounds
                using (Pen borderPen = new Pen(Color.Gray, 1))
                {
                    g.DrawRectangle(borderPen, imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height);
                }
                g.Clip = originalClip;
            }

            // 2. Draw Rulers
            DrawRulers(g, pxPerMmX, pxPerMmY, imgPxWidth, imgPxHeight, scrollX, scrollY);
        }

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
                //int maxMmX = (int)((Width - RulerSize) / pxPerMmX);
                int startMmX = Math.Max(0, (int)(scrollX / pxPerMmX));
                int endMmX = (int)((scrollX + Width - RulerSize) / pxPerMmX) + 1;
                int maxMmX = (int)_mmWidth;

                for (int mm = startMmX; mm <= Math.Min(endMmX, maxMmX); mm++)
                {
                    float x = RulerSize + (mm * pxPerMmX) - scrollX;

                    if (mm % 100 == 0) // Major tick (100mm)
                    {
                        g.DrawLine(linePen, x, 10, x, RulerSize);
                        g.DrawString((mm / 10).ToString(), textFont, textBrush, x + 1, 1);
                    }
                    else if (mm % 50 == 0) // Medium tick (50mm)
                    {
                        g.DrawLine(linePen, x, 18, x, RulerSize);
                    }
                    else if (mm % 10 == 0) // Minor tick (10mm)
                    {
                        g.DrawLine(linePen, x, 23, x, RulerSize);
                    }
                }

                //for (int mm = 0; mm <= maxMmX; mm++)
                //{
                //    float x = RulerSize + (mm * pxPerMmX);

                //    if (mm % 100 == 0) // Major tick (100mm)
                //    {
                //        g.DrawLine(linePen, x, 10, x, RulerSize);
                //        g.DrawString((mm / 10).ToString(), textFont, textBrush, x + 1, 1);
                //    }
                //    else if (mm % 50 == 0) // Medium tick (50mm)
                //    {
                //        g.DrawLine(linePen, x, 18, x, RulerSize);
                //    }
                //    else if(mm % 10 == 0) // Minor tick (10mm)
                //    {
                //        g.DrawLine(linePen, x, 23, x, RulerSize);
                //    }
                //}

                // --- Vertical Ruler (Left) ---

                g.SetClip(new Rectangle(0, RulerSize, RulerSize, Height - RulerSize));

                int startMmY = Math.Max(0, (int)(scrollY / pxPerMmY));
                int endMmY = (int)((scrollY + Height - RulerSize) / pxPerMmY) + 1;
                int maxMmY = (int)_mmHeight;

                for (int mm = startMmY; mm <= Math.Min(endMmY, maxMmY); mm++)
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

                //int maxMmY = (int)((Height - RulerSize) / pxPerMmY);
                //for (int mm = 0; mm <= maxMmY; mm++)
                //{
                //    float y = RulerSize + (mm * pxPerMmY);

                //    if (mm % 100 == 0) // Major tick (100mm)
                //    {
                //        g.DrawLine(linePen, 10, y, RulerSize, y);
                //        g.DrawString((mm / 10).ToString(), textFont, textBrush, 1, y + 1);
                //    }
                //    else if (mm % 50 == 0) // Medium tick (50mm)
                //    {
                //        g.DrawLine(linePen, 18, y, RulerSize, y);
                //    }
                //    else if (mm % 10 == 0) // Minor tick (10mm)
                //    {
                //        g.DrawLine(linePen, 23, y, RulerSize, y);
                //    }
                //}

                int totalWidth = (int)(_mmWidth * pxPerMmX) + RulerSize;
                int totalHeight = (int)(_mmHeight * pxPerMmY) + RulerSize;

                AutoScrollMinSize = new Size(totalWidth, totalHeight);

                // --- Corner Square (Anchor Intersection at 0,0) ---
                g.FillRectangle(Brushes.LightGray, 0, 0, RulerSize, RulerSize);
                g.DrawRectangle(linePen, 0, 0, RulerSize, RulerSize);
                g.DrawString("mm", textFont, textBrush, 4, 8);

                // Outer Ruler Separation Lines
                g.DrawLine(linePen, RulerSize, 0, RulerSize, Height);
                g.DrawLine(linePen, 0, RulerSize, Width, RulerSize);
            }
        }
    }
}
