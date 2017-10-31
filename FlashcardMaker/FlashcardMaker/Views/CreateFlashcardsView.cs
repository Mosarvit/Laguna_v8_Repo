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
    public partial class CreateFlashcardsView : Form, ISessionView
    {
        private CreateFlashcardsController createFlashcardsController;

        private bool repeatingOutput = false;

        public CreateFlashcardsView()
        {
            InitializeComponent();
            cbSortinAlgorythm.Items.Add("SorthingAlgorythm1");
            cbSortinAlgorythm.SelectedItem = "SorthingAlgorythm1";
            this.Update();
        }

        internal void setSyncController(CreateFlashcardsController createFlashcardsController)
        {
            this.createFlashcardsController = createFlashcardsController;
        }

        private void btnCreateFlashcards_Click(object sender, EventArgs e)
        {
            string sortingAlgorithmString = cbSortinAlgorythm.Text;
            int gapLimit = Convert.ToInt32(nudGap.Value)*1000;
            int beforeLimit = Convert.ToInt32(nudBefore.Value) * 1000;
            int afterLimit = Convert.ToInt32(nudAfter.Value) * 1000;

            createFlashcardsController.createFlashcards(sortingAlgorithmString, gapLimit, beforeLimit, afterLimit);
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

            this.lblStatus.Text = v;
            this.Update();

        }

        private void CreateFlashcardsView_Load(object sender, EventArgs e)
        {
            if (ProgramController.DEBUGGING_CREATEFLASHCARDS)
            {
                btnCreateFlashcards.PerformClick();
            }            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
