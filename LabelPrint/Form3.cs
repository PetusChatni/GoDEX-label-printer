using System.Drawing.Imaging;

namespace LabelPrint
{
    public partial class Form3 : Form
    {
        private string filename;
        private string path;

        /// <summary>
        ///  Occurs when form is submitted
        /// </summary>
        public event EventHandler<EventArgs> FormSubmitted;

        public Form3()
        {
            InitializeComponent();
        }

        #region Event functions
        private void OnFormSubmitted(EventArgs e)
        {
            FormSubmitted?.Invoke(this, e);
        }
        #endregion

        #region Button Click functions
        /// <summary>
        /// Checks validity of both inputs & if valid, closes the popup (Form3)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTN_AddImg_Click(object sender, EventArgs e)
        {
            if (!FieldChecker.IsNameValid(TB_Name.Text, out filename) |
                !FieldChecker.IsFilePathValid(TB_Path.Text, out path))
            {
                return;
            }

            OnFormSubmitted(EventArgs.Empty);
        }

        /// <summary>
        /// Opens window for file selection & saves chosen file's path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTN_OpenFileWizard_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TB_Path.Text = openFileDialog1.FileName;
                UpdatePreview();
            }
        }

        private void TB_Path_Leave(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            MessageBox.Show("Called");

            try
            {
                MIRV_Preview.LoadImage(TB_Path.Text);
                float[] sizes = MIRV_Preview.GetImageScaleMm();

                if (sizes.Length < 2)
                    return;

                NUD_Width.Value = (decimal)sizes[0];
                NUD_Height.Value = (decimal)sizes[1];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form3.cs; 80; {ex.Message}");
                MIRV_Preview.ClearImage();
                NUD_Width.Value = (decimal).01f;
                NUD_Height.Value = (decimal).01f;
            }
        }

        private void NUD_Width_ValueChanged(object sender, EventArgs e)
        {
            UpdateScale();
        }

        private void NUD_Height_ValueChanged(object sender, EventArgs e)
        {
            UpdateScale();
        }

        private void UpdateScale()
        {
            if (NUD_Width != null && NUD_Height != null)
                MIRV_Preview.SetImageScaleMm((float)NUD_Width.Value, (float)NUD_Height.Value);
        }

        //private void DrawRulers(Graphics g, Rectangle imgRect)
        //{
        //    Pen tickPen = Pens.Black;
        //    Font font = new Font("Arial", 7);
        //    Brush textBrush = Brushes.Black;

        //    int RulerThickness = 25;
        //    int zoomFactor = 1;

        //    // 203 DPI pixels per mm
        //    float pixelsPerMm = (203f / 25.4f) * zoomFactor;

        //    // --- Horizontal Ruler (Top) ---
        //    g.DrawLine(tickPen, imgRect.Left, RulerThickness, imgRect.Right, RulerThickness);
        //    for (float mm = 0; mm <= (PB_Preview.Image.Width / (203f / 25.4f)); mm += 5) // Tick every 5mm
        //    {
        //        float xPos = imgRect.Left + (mm * pixelsPerMm);
        //        if (xPos > imgRect.Right) break;

        //        int tickHeight = (mm % 10 == 0) ? 10 : 5; // Longer tick for every 10mm
        //        g.DrawLine(tickPen, xPos, RulerThickness - tickHeight, xPos, RulerThickness);

        //        if (mm % 10 == 0) // Label every 10mm (1cm)
        //        {
        //            g.DrawString($"{mm}", font, textBrush, xPos - 8, RulerThickness - 22);
        //        }
        //    }

        //    // --- Vertical Ruler (Left) ---
        //    g.DrawLine(tickPen, RulerThickness, imgRect.Top, RulerThickness, imgRect.Bottom);
        //    for (float mm = 0; mm <= (PB_Preview.Image.Height / (203f / 25.4f)); mm += 5)
        //    {
        //        float yPos = imgRect.Top + (mm * pixelsPerMm);
        //        if (yPos > imgRect.Bottom) break;

        //        int tickWidth = (mm % 10 == 0) ? 10 : 5;
        //        g.DrawLine(tickPen, RulerThickness - tickWidth, yPos, RulerThickness, yPos);

        //        if (mm % 10 == 0)
        //        {
        //            // Draw vertical text or rotate for cleaner look, simplified here:
        //            g.DrawString($"{mm}", font, textBrush, 2, yPos - 5);
        //        }
        //    }
        //}

        #endregion

        #region Getters
        public string FileName
        {
            get
            {
                return filename;
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
        }
        #endregion
    }
}
