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

            nudMaxCreateSubtitleLinePacks.Value = Properties.Settings.Default.DEBUG_MAX_CREATE_SUBTITLE_LINEPACKS;
            nudMaxSortSubtitleLinePacks.Value = Properties.Settings.Default.DEBUG_MAX_SORT_SUBTITLE_LINEPACKS;
            nudMaxMediaFiles.Value = Properties.Settings.Default.DEBUG_MAX_CREATE_MEDIA_FILES;
            nudMaxFlashcards.Value = Properties.Settings.Default.DEBUG_MAX_CREATE_FLASHCARDS;
            cbOnlyWithMediaFiles.Checked = Properties.Settings.Default.DEBUG_ONLY_CREATE_FLASHCARDS_WITH_MEDIAFILES;
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

        private void nudMaxCreateSubtitleLinePacks_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DEBUG_MAX_CREATE_SUBTITLE_LINEPACKS = (int)nudMaxCreateSubtitleLinePacks.Value;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
        }

        private void nudMaxSortSubtitleLinePacks_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DEBUG_MAX_SORT_SUBTITLE_LINEPACKS = (int)nudMaxSortSubtitleLinePacks.Value;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
        }

        private void nudMaxMediaFiles_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DEBUG_MAX_CREATE_MEDIA_FILES = (int)nudMaxMediaFiles.Value;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
        }

        private void nudMaxFlashcards_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DEBUG_MAX_CREATE_FLASHCARDS = (int)nudMaxFlashcards.Value;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
        }

        private void cbOnlyWithMediaFiles_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DEBUG_ONLY_CREATE_FLASHCARDS_WITH_MEDIAFILES = cbOnlyWithMediaFiles.Checked;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
        }
    }
}
