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

            //using (var ms = new MemoryStream())
            //{
            //    MIRV_Preview.Image.Save(ms, MIRV_Preview.Image.RawFormat);
            //    //MessageBox.Show($"Size: {ms.ToArray().Length} b");
            //}

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
            //MessageBox.Show("Called");

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

        public MmImageRulerViewer MIRV_PreviewImage { get { return MIRV_Preview; } }
    }
}
