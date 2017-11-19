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
    public partial class SyncView : Form, ISessionView
    {
        private SyncController syncController;
        private bool repeatingOutput = false;

        public SyncView()
        {
            InitializeComponent();
        }        

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtbxOutput.AppendText("a");
            txtbxOutput.AppendText(Environment.NewLine);
            this.Update();
        }

        public void printInOutputTextField(string v)
        {
            txtbxOutput.AppendText(v);
            txtbxOutput.AppendText(Environment.NewLine);
            this.Update();
        }

        internal void setSyncController(SyncController syncController)
        {
            this.syncController = syncController;
        }



        public void printLine(string v)
        {
            repeatingOutput = false;
            txtbxOutput.AppendText(v);
            txtbxOutput.AppendText(Environment.NewLine);
            this.Update();
        }

        public void printInLineInMainTextLabel(string v)
        {
            repeatingOutput = false;
            txtbxOutput.AppendText(v);
            this.Update();
        }

        public void printStatusLabel(string v)
        {
            if (repeatingOutput)
            {
                txtbxOutput.Text = txtbxOutput.Text.Remove(txtbxOutput.Text.LastIndexOf(Environment.NewLine));
                txtbxOutput.AppendText(v);
                txtbxOutput.AppendText(Environment.NewLine);
            }
            else
            {
                repeatingOutput = true;
                txtbxOutput.AppendText(v);
                txtbxOutput.AppendText(Environment.NewLine);
            }

            this.Update();
        }

        public void refresh()
        {
            throw new NotImplementedException();
        }

        private void SyncView_Load(object sender, EventArgs e)
        {

        }
    }
}
