using FlashcardMaker.Controllers;
using FlashcardMaker.Helpers;
using FlashcardMaker.Models;
using FlashcardMaker.Views;
//using FlashcardMaker.Views;
using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace FlashcardMaker
{
    public partial class MainView : Form, ISessionView
    {
        ProgramController programController;
        DataIOController dataIOController;
        OpenFileDialog ofd1 = new OpenFileDialog();

        //private bool repeatingOutput = false;

        public MainView()
        {
            InitializeComponent();
            InitializeOpenFileDialog();
            DefaultTestStuff();
        }

        public void setDataIOController(DataIOController dataIOController)
        {
            this.dataIOController = dataIOController;
        }

        public void setProgramController(ProgramController programController)
        {
            this.programController = programController;
        }

        public void DefaultTestStuff()
        {
            refresh();
        }

        public void refresh()
        {
            refreshMovieList();
        }

        private void refreshMovieList()
        {
            StringBuilder stringBuilder2 = new StringBuilder();

            var db = new MyDbContext();

            var allMovies = from b in db.Movies
                            select b;


            foreach (Movie movie in allMovies)
            {
                stringBuilder2.Append(movie.fileName);
                stringBuilder2.Append("\n");
            }


            if (stringBuilder2.Length == 0)
            {
                stringBuilder2.Append("No Movies in the Database");
            }

            printMovieList(stringBuilder2.ToString());
        }

        public void printLine(string v)
        {
            //repeatingOutput = false;
            txtbxOutput.AppendText(v);
            txtbxOutput.AppendText(Environment.NewLine);
            this.Update();
        }

        //public void printInLineInMainTextLabel(string v)
        //{
        //    repeatingOutput = false;
        //    txtbxOutput.AppendText(v);
        //    this.Update();
        //}

        public void printStatusLabel(string v)
        {
            this.lblStatus.Text = v;
            this.Update();
        }

        public void printMovieList(string v)
        {
            lblMovieList.Text = v;
            this.Update();
        }

        private void InitializeOpenFileDialog()
        {
            // Set the file dialog to filter for graphics files.
            this.ofd1.Filter =
                "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +
                "All files (*.*)|*.*";

            // Allow the user to select multiple images.
            this.ofd1.Multiselect = true;
            this.ofd1.Title = "My Image Browser";
        }

        private void btnBrowseAddToDatabase_Click(object sender, EventArgs e)
        {
            bool multiSelect = true;
            TextBox textBox = txtbxAddToDatabase;
            browseAndPutInTextBox(multiSelect, textBox);
        }

        private void browseAndPutInTextBox(bool multiSelect, TextBox textBox)
        {
            this.ofd1.Filter = "All files (*.*)|*.*";
            this.ofd1.Multiselect = multiSelect;

            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                if (ofd1.FileNames.Length > 1)
                {
                    StringBuilder StringBuilder1 = new StringBuilder();

                    foreach (String file in ofd1.FileNames)
                    {
                        StringBuilder1.Append("\"" + file + "\" ; ");
                    }

                    StringBuilder1.Remove(StringBuilder1.Length - 3, 3);

                    textBox.Text = StringBuilder1.ToString();
                }
                else
                {
                    textBox.Text = ofd1.FileNames[0];
                }
            }
        }

        private void btnAddToDatabase_Click(object sender, EventArgs e)
        {
            string[] FileNames = extractFileNamesFromTextBox(txtbxAddToDatabase);

            dataIOController.ReadInFromSubtitleFileToDb(FileNames);
        }

        private void test_Click(object sender, EventArgs e)
        {
            dataIOController.Test();
        }

        private void txtbxAddToDatabase_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblOutput_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void MainView_Load(object sender, EventArgs e)
        {
            if (ProgramController.DEBUGGING_CREATEFLASHCARDS)
            {
                btnCreateFlashcards.PerformClick();
            }
            else if(ProgramController.DEBUGGING_VIDEO_EDITOR)
            {

                btnCreateFlashcards.PerformClick();
            }

            nudMaxCCsToLoad.Value = Properties.Settings.Default.DEBUG_MAX_CCS_TO_LOAD;

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            browseAndPutInTextBox(false, txtbxAddCharactersToDatabase);

        }

        private void btnAddCharactersToDatabase_Click(object sender, EventArgs e)
        {
            string[] FileNames = extractFileNamesFromTextBox(txtbxAddCharactersToDatabase);

            dataIOController.AddExcelDataToDatabase<ChineseCharacter>(FileNames[0]);
        }

        private static string[] extractFileNamesFromTextBox(TextBox textBox)
        {
            ArrayList Files = new ArrayList();
            String[] FileNames = textBox.Text.Replace("\"", "").Replace(" ", "").Split(';');
            return FileNames;
        }

        private void txtbxAddCharactersToDatabase_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnBrowseAddWordsToDatabase_Click(object sender, EventArgs e)
        {
            browseAndPutInTextBox(false, txtbxAddCharactersToDatabase);
        }

        private void btnAddWordsToDatabase_Click(object sender, EventArgs e)
        {
            string[] FileNames = extractFileNamesFromTextBox(txtbxAddWordsToDatabase);

            dataIOController.AddExcelDataToDatabase<ChineseWord>(FileNames[0]);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            dataIOController.Refresh();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            dataIOController.ClearAll();
        }

        private void btnCreateFlashcards_Click(object sender, EventArgs e)
        {
            programController.StartCreatingFlashcardsSession();
        }



        private void btnClearSubtitles_Click(object sender, EventArgs e)
        {
            dataIOController.ClearSubtitles();
        }

        private void btnDeleteAllFlashCards_Click(object sender, EventArgs e)
        {
            dataIOController.DeleteAllFlashcards();
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            programController.startSyncSession();
        }

        private void btnClearCharacters_Click(object sender, EventArgs e)
        {
            dataIOController.ClearCharacters();
        }

        private void btnClearWords_Click(object sender, EventArgs e)
        {
            dataIOController.ClearWords();
        }

        private void btnBrowseMediaFolder_Click(object sender, EventArgs e)
        {

        }

        private void btnRefreshSubtitleLines_Click(object sender, EventArgs e)
        {
            dataIOController.UpdateSubtitleLines();
        }

        private void btnDeleteAllMediaFiles_Click(object sender, EventArgs e)
        {
            dataIOController.DeleteAllMediaFiles();
        }

        private void nudMaxCCsToLoad_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DEBUG_MAX_CCS_TO_LOAD = (int)nudMaxCCsToLoad.Value;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
        }
    }
}
