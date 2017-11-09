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

        //private bool repeatingOutput = false;

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

        private void btnCreateSubtitleLinePacks_Click(object sender, EventArgs e)
        { 
            int gapLimit = Convert.ToInt32(nudGap.Value)*1000;
            int beforeLimit = Convert.ToInt32(nudBefore.Value) * 1000;
            int afterLimit = Convert.ToInt32(nudAfter.Value) * 1000;
            int paddingBefore = Convert.ToInt32(nudPaddingBefore.Value) * 1000;
            int paddingAfter = Convert.ToInt32(nudPaddingAfter.Value) * 1000;
            int gapLimitC = Convert.ToInt32(nudGapC.Value);
            int beforeLimitC = Convert.ToInt32(nudBeforeC.Value);
            int afterLimitC = Convert.ToInt32(nudAfterC.Value);

            createFlashcardsController.createSubtitleLinePacks(gapLimit, beforeLimit, afterLimit, gapLimitC, beforeLimitC, afterLimitC, paddingBefore, paddingAfter);
        }

        public void printLine(string v)
        {
            //repeatingOutput = false;
            txtbxOutput.AppendText(v);
            txtbxOutput.AppendText(Environment.NewLine);
            this.Update();
        }

        public void printInLineInMainTextLabel(string v)
        {
            //repeatingOutput = false;
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
                btnCreateSubtitleLinePacks.PerformClick();
            }
            if (ProgramController.DEBUGGING_VIDEO_EDITOR)
            {
                btnCreateMediaFiles.PerformClick();
            }
        }


        public void refresh()
        {
            throw new NotImplementedException();
        }

        private void btnCreateMediaFiles_Click(object sender, EventArgs e)
        {
            ProgramController.startMediaCreationSession(this);
        }

        private void btnCreateFlashcards_Click_1(object sender, EventArgs e)
        {
            createFlashcardsController.CreateFlashcards(this);
        }

        private void btnSortSubtitleLinePacks_Click(object sender, EventArgs e)
        {
            createFlashcardsController.SortSubtitleLinePacks(this, cbSortinAlgorythm.Text, Convert.ToInt32(nudImportanceOfDensity.Value));
        }
    }
}
