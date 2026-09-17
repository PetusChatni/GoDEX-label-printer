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
                TB_Path.Text = openFileDialog1.FileName;
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
    }
}
