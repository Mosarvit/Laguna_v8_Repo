using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashcardMaker.Views
{
    public partial class DecisionAboutContradictionsView : Form
    {
        public string answer = "Cancel";

        public DecisionAboutContradictionsView()
        {
            InitializeComponent();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            this.answer = "DownloadFromServer";
            this.Close();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            this.answer = "UploadToServer";
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.answer = "Cancel";
            this.Close();
        }
    }
}
