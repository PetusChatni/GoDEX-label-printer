using System.Text;

namespace LabelPrint
{
    public partial class Form3 : Form
    {
        private string filename;
        private string path = "";
        private bool updatingSize = false;

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

        #region UI Event functions
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
                path = TB_Path.Text;
                UpdatePreview();
            }
        }

        /// <summary>
        /// Updates image preview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TB_Path_Leave(object sender, EventArgs e)
        {
            if (TB_Path.Text != path)
            {
                MessageBox.Show($"orig: {ReplaceCtrl(TB_Path.Text)}, stored: {ReplaceCtrl(path)}");
                path = TB_Path.Text;

                UpdatePreview();
            }
        }

        /// <summary>
        /// DEBUG ONLY
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReplaceCtrl(string s)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                var isCtrl = char.IsControl(c);
                var n = char.ConvertToUtf32(s, i);

                if (isCtrl)
                {
                    sb.Append($"\\u{n:X4}");
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }


        /// <summary>
        /// Updates MIRV_Preview
        /// </summary>
        private void UpdatePreview()
        {
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
                //MessageBox.Show($"Form3.cs; 80; {ex.Message}");
                MIRV_Preview.ClearImage();
                NUD_Width.Value = (decimal).01f;
                NUD_Height.Value = (decimal).01f;
            }
        }

        /// <summary>
        /// Updates image scale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NUD_Width_ValueChanged(object sender, EventArgs e)
        {
            UpdateScale();
        }

        /// <summary>
        /// Updates image scale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NUD_Height_ValueChanged(object sender, EventArgs e)
        {
            UpdateScale();
        }

        /// <summary>
        /// Updates image DPI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NUD_DPI_ValueChanged(object sender, EventArgs e)
        {
            if (NUD_DPI != null)
                MIRV_Preview.DPI = (float)NUD_DPI.Value;
        }

        /// <summary>
        /// Requests MIRV_Preview to change image scale
        /// </summary>
        private void UpdateScale()
        {
            if (!updatingSize && NUD_Width != null && NUD_Height != null)
                MIRV_Preview.SetImageScaleMm((float)NUD_Width.Value, (float)NUD_Height.Value);
        }

        /// <summary>
        /// Updates scale NUDs after MIRV_Preview finished updating DPI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateScaleFromMIRV(object? sender, EventArgs e)
        {
            if (MIRV_Preview != null)
            {
                updatingSize = true;
                
                NUD_Width.Value = (decimal)MIRV_Preview.MmWidth;
                NUD_Height.Value = (decimal)MIRV_Preview.MmHeight;

                updatingSize = false;
            }
        }

        #endregion

        #region Getters
        public string FileName { get { return filename; } }
        public MmImageRulerViewer MIRV_PreviewImage { get { return MIRV_Preview; } }

        #endregion
    }
}
