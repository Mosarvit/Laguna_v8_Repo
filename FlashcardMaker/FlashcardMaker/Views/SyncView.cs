using FlashcardMaker.Controllers;
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
    public partial class SyncView : Form
    {
        private SyncController syncController;

        public SyncView()
        {
            InitializeComponent();
        }        

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtOutput.AppendText("a");
            txtOutput.AppendText(Environment.NewLine);
            this.Update();
        }

        public void printInOutputTextField(string v)
        {
            txtOutput.AppendText(v);
            txtOutput.AppendText(Environment.NewLine);
            this.Update();
        }

        internal void setSyncController(SyncController syncController)
        {
            this.syncController = syncController;
        }

        internal string decideAboutConradictions()
        {
            DecisionAboutContradictionsView dacv = new DecisionAboutContradictionsView();
            dacv.StartPosition = FormStartPosition.Manual;
            dacv.Location = new Point(10, 10);
            dacv.ShowDialog();

            return dacv.answer;
        }
    }
}
