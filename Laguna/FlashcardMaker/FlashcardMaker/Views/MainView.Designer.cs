namespace FlashcardMaker
{
    partial class MainView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
            this.btnBrowseAddToDatabase = new System.Windows.Forms.Button();
            this.btnAddToDatabase = new System.Windows.Forms.Button();
            this.txtbxAddToDatabase = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.test = new System.Windows.Forms.Button();
            this.lblMovieList = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtbxAddCharactersToDatabase = new System.Windows.Forms.TextBox();
            this.btnBrowseAddCharactersToDatabase = new System.Windows.Forms.Button();
            this.btnAddCharactersToDatabase = new System.Windows.Forms.Button();
            this.txtbxAddWordsToDatabase = new System.Windows.Forms.TextBox();
            this.btnBrowseAddWordsToDatabase = new System.Windows.Forms.Button();
            this.btnAddWordsToDatabase = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.btnCreateFlashcards = new System.Windows.Forms.Button();
            this.btnClearSubtitles = new System.Windows.Forms.Button();
            this.txtbxOutput = new System.Windows.Forms.TextBox();
            this.btnDeleteAllFlashCards = new System.Windows.Forms.Button();
            this.btnSync = new System.Windows.Forms.Button();
            this.btnClearCharacters = new System.Windows.Forms.Button();
            this.btnClearWords = new System.Windows.Forms.Button();
            this.txtbxMediaFolder = new System.Windows.Forms.TextBox();
            this.btnBrowseMediaFolder = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnRefreshSubtitleLines = new System.Windows.Forms.Button();
            this.btnDeleteAllMediaFiles = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnBrowseAddToDatabase
            // 
            this.btnBrowseAddToDatabase.Location = new System.Drawing.Point(447, 38);
            this.btnBrowseAddToDatabase.Name = "btnBrowseAddToDatabase";
            this.btnBrowseAddToDatabase.Size = new System.Drawing.Size(102, 23);
            this.btnBrowseAddToDatabase.TabIndex = 0;
            this.btnBrowseAddToDatabase.Text = "Browse...";
            this.btnBrowseAddToDatabase.UseVisualStyleBackColor = true;
            this.btnBrowseAddToDatabase.Click += new System.EventHandler(this.btnBrowseAddToDatabase_Click);
            // 
            // btnAddToDatabase
            // 
            this.btnAddToDatabase.Location = new System.Drawing.Point(555, 38);
            this.btnAddToDatabase.Name = "btnAddToDatabase";
            this.btnAddToDatabase.Size = new System.Drawing.Size(166, 23);
            this.btnAddToDatabase.TabIndex = 1;
            this.btnAddToDatabase.Text = "Add Subtitles to Database";
            this.btnAddToDatabase.UseVisualStyleBackColor = true;
            this.btnAddToDatabase.Click += new System.EventHandler(this.btnAddToDatabase_Click);
            // 
            // txtbxAddToDatabase
            // 
            this.txtbxAddToDatabase.Location = new System.Drawing.Point(18, 40);
            this.txtbxAddToDatabase.Name = "txtbxAddToDatabase";
            this.txtbxAddToDatabase.Size = new System.Drawing.Size(423, 20);
            this.txtbxAddToDatabase.TabIndex = 2;
            this.txtbxAddToDatabase.Text = resources.GetString("txtbxAddToDatabase.Text");
            this.txtbxAddToDatabase.TextChanged += new System.EventHandler(this.txtbxAddToDatabase_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Add Subtitles to Database";
            // 
            // test
            // 
            this.test.Location = new System.Drawing.Point(1065, 40);
            this.test.Name = "test";
            this.test.Size = new System.Drawing.Size(116, 23);
            this.test.TabIndex = 5;
            this.test.Text = "test";
            this.test.UseVisualStyleBackColor = true;
            this.test.Click += new System.EventHandler(this.test_Click);
            // 
            // lblMovieList
            // 
            this.lblMovieList.Location = new System.Drawing.Point(546, 215);
            this.lblMovieList.Name = "lblMovieList";
            this.lblMovieList.Size = new System.Drawing.Size(338, 432);
            this.lblMovieList.TabIndex = 6;
            this.lblMovieList.Text = "label2";
            this.lblMovieList.Click += new System.EventHandler(this.label2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(549, 202);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(163, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "All Movies in the Database:";
            this.label2.Click += new System.EventHandler(this.label2_Click_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 202);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Main Output Window:";
            // 
            // txtbxAddCharactersToDatabase
            // 
            this.txtbxAddCharactersToDatabase.Location = new System.Drawing.Point(18, 107);
            this.txtbxAddCharactersToDatabase.Name = "txtbxAddCharactersToDatabase";
            this.txtbxAddCharactersToDatabase.Size = new System.Drawing.Size(423, 20);
            this.txtbxAddCharactersToDatabase.TabIndex = 9;
            this.txtbxAddCharactersToDatabase.Text = "E:\\Users\\Mosarvit\\Documents\\SharedFolder\\10000.xls";
            this.txtbxAddCharactersToDatabase.TextChanged += new System.EventHandler(this.txtbxAddCharactersToDatabase_TextChanged);
            // 
            // btnBrowseAddCharactersToDatabase
            // 
            this.btnBrowseAddCharactersToDatabase.Location = new System.Drawing.Point(447, 107);
            this.btnBrowseAddCharactersToDatabase.Name = "btnBrowseAddCharactersToDatabase";
            this.btnBrowseAddCharactersToDatabase.Size = new System.Drawing.Size(102, 23);
            this.btnBrowseAddCharactersToDatabase.TabIndex = 10;
            this.btnBrowseAddCharactersToDatabase.Text = "Browse...";
            this.btnBrowseAddCharactersToDatabase.UseVisualStyleBackColor = true;
            this.btnBrowseAddCharactersToDatabase.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnAddCharactersToDatabase
            // 
            this.btnAddCharactersToDatabase.Location = new System.Drawing.Point(555, 107);
            this.btnAddCharactersToDatabase.Name = "btnAddCharactersToDatabase";
            this.btnAddCharactersToDatabase.Size = new System.Drawing.Size(166, 23);
            this.btnAddCharactersToDatabase.TabIndex = 11;
            this.btnAddCharactersToDatabase.Text = "Add Character list to Database";
            this.btnAddCharactersToDatabase.UseVisualStyleBackColor = true;
            this.btnAddCharactersToDatabase.Click += new System.EventHandler(this.btnAddCharactersToDatabase_Click);
            // 
            // txtbxAddWordsToDatabase
            // 
            this.txtbxAddWordsToDatabase.Location = new System.Drawing.Point(18, 133);
            this.txtbxAddWordsToDatabase.Name = "txtbxAddWordsToDatabase";
            this.txtbxAddWordsToDatabase.Size = new System.Drawing.Size(423, 20);
            this.txtbxAddWordsToDatabase.TabIndex = 12;
            this.txtbxAddWordsToDatabase.Text = "E:\\Users\\Mosarvit\\Documents\\SharedFolder\\chineseWords.xls";
            // 
            // btnBrowseAddWordsToDatabase
            // 
            this.btnBrowseAddWordsToDatabase.Location = new System.Drawing.Point(447, 133);
            this.btnBrowseAddWordsToDatabase.Name = "btnBrowseAddWordsToDatabase";
            this.btnBrowseAddWordsToDatabase.Size = new System.Drawing.Size(102, 23);
            this.btnBrowseAddWordsToDatabase.TabIndex = 13;
            this.btnBrowseAddWordsToDatabase.Text = "Browse...";
            this.btnBrowseAddWordsToDatabase.UseVisualStyleBackColor = true;
            this.btnBrowseAddWordsToDatabase.Click += new System.EventHandler(this.btnBrowseAddWordsToDatabase_Click);
            // 
            // btnAddWordsToDatabase
            // 
            this.btnAddWordsToDatabase.Location = new System.Drawing.Point(555, 133);
            this.btnAddWordsToDatabase.Name = "btnAddWordsToDatabase";
            this.btnAddWordsToDatabase.Size = new System.Drawing.Size(166, 23);
            this.btnAddWordsToDatabase.TabIndex = 14;
            this.btnAddWordsToDatabase.Text = "Add Words list to Database";
            this.btnAddWordsToDatabase.UseVisualStyleBackColor = true;
            this.btnAddWordsToDatabase.Click += new System.EventHandler(this.btnAddWordsToDatabase_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Location = new System.Drawing.Point(0, 657);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(56, 13);
            this.lblStatus.TabIndex = 15;
            this.lblStatus.Text = "No Output";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(727, 162);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(116, 23);
            this.btnRefresh.TabIndex = 16;
            this.btnRefresh.Text = "Refresh All";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(555, 171);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(116, 23);
            this.btnClearAll.TabIndex = 17;
            this.btnClearAll.Text = "Clear All";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // btnCreateFlashcards
            // 
            this.btnCreateFlashcards.Location = new System.Drawing.Point(1065, 147);
            this.btnCreateFlashcards.Name = "btnCreateFlashcards";
            this.btnCreateFlashcards.Size = new System.Drawing.Size(116, 23);
            this.btnCreateFlashcards.TabIndex = 18;
            this.btnCreateFlashcards.Text = "Create Flashcards";
            this.btnCreateFlashcards.UseVisualStyleBackColor = true;
            this.btnCreateFlashcards.Click += new System.EventHandler(this.btnCreateFlashcards_Click);
            // 
            // btnClearSubtitles
            // 
            this.btnClearSubtitles.Location = new System.Drawing.Point(727, 38);
            this.btnClearSubtitles.Name = "btnClearSubtitles";
            this.btnClearSubtitles.Size = new System.Drawing.Size(116, 23);
            this.btnClearSubtitles.TabIndex = 19;
            this.btnClearSubtitles.Text = "Clear Subtitles";
            this.btnClearSubtitles.UseVisualStyleBackColor = true;
            this.btnClearSubtitles.Click += new System.EventHandler(this.btnClearSubtitles_Click);
            // 
            // txtbxOutput
            // 
            this.txtbxOutput.Location = new System.Drawing.Point(12, 222);
            this.txtbxOutput.Multiline = true;
            this.txtbxOutput.Name = "txtbxOutput";
            this.txtbxOutput.ReadOnly = true;
            this.txtbxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtbxOutput.Size = new System.Drawing.Size(525, 425);
            this.txtbxOutput.TabIndex = 20;
            this.txtbxOutput.WordWrap = false;
            // 
            // btnDeleteAllFlashCards
            // 
            this.btnDeleteAllFlashCards.Location = new System.Drawing.Point(1218, 147);
            this.btnDeleteAllFlashCards.Name = "btnDeleteAllFlashCards";
            this.btnDeleteAllFlashCards.Size = new System.Drawing.Size(116, 23);
            this.btnDeleteAllFlashCards.TabIndex = 21;
            this.btnDeleteAllFlashCards.Text = "Delete All Flashcards";
            this.btnDeleteAllFlashCards.UseVisualStyleBackColor = true;
            this.btnDeleteAllFlashCards.Click += new System.EventHandler(this.btnDeleteAllFlashCards_Click);
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(1218, 39);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(116, 23);
            this.btnSync.TabIndex = 22;
            this.btnSync.Text = "Sync";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // btnClearCharacters
            // 
            this.btnClearCharacters.Location = new System.Drawing.Point(727, 107);
            this.btnClearCharacters.Name = "btnClearCharacters";
            this.btnClearCharacters.Size = new System.Drawing.Size(116, 23);
            this.btnClearCharacters.TabIndex = 23;
            this.btnClearCharacters.Text = "Clear Characters";
            this.btnClearCharacters.UseVisualStyleBackColor = true;
            this.btnClearCharacters.Click += new System.EventHandler(this.btnClearCharacters_Click);
            // 
            // btnClearWords
            // 
            this.btnClearWords.Location = new System.Drawing.Point(727, 133);
            this.btnClearWords.Name = "btnClearWords";
            this.btnClearWords.Size = new System.Drawing.Size(116, 23);
            this.btnClearWords.TabIndex = 24;
            this.btnClearWords.Text = "Clear Words";
            this.btnClearWords.UseVisualStyleBackColor = true;
            this.btnClearWords.Click += new System.EventHandler(this.btnClearWords_Click);
            // 
            // txtbxMediaFolder
            // 
            this.txtbxMediaFolder.Location = new System.Drawing.Point(18, 81);
            this.txtbxMediaFolder.Name = "txtbxMediaFolder";
            this.txtbxMediaFolder.Size = new System.Drawing.Size(423, 20);
            this.txtbxMediaFolder.TabIndex = 25;
            this.txtbxMediaFolder.Text = "E:\\Users\\Mosarvit\\Documents\\SharedFolder\\subtitles\\Media";
            // 
            // btnBrowseMediaFolder
            // 
            this.btnBrowseMediaFolder.Location = new System.Drawing.Point(447, 79);
            this.btnBrowseMediaFolder.Name = "btnBrowseMediaFolder";
            this.btnBrowseMediaFolder.Size = new System.Drawing.Size(102, 23);
            this.btnBrowseMediaFolder.TabIndex = 26;
            this.btnBrowseMediaFolder.Text = "Browse...";
            this.btnBrowseMediaFolder.UseVisualStyleBackColor = true;
            this.btnBrowseMediaFolder.Click += new System.EventHandler(this.btnBrowseMediaFolder_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Select Media Folder";
            // 
            // btnRefreshSubtitleLines
            // 
            this.btnRefreshSubtitleLines.Location = new System.Drawing.Point(849, 38);
            this.btnRefreshSubtitleLines.Name = "btnRefreshSubtitleLines";
            this.btnRefreshSubtitleLines.Size = new System.Drawing.Size(116, 23);
            this.btnRefreshSubtitleLines.TabIndex = 28;
            this.btnRefreshSubtitleLines.Text = "Refresh SubtitleLines";
            this.btnRefreshSubtitleLines.UseVisualStyleBackColor = true;
            this.btnRefreshSubtitleLines.Click += new System.EventHandler(this.btnRefreshSubtitleLines_Click);
            // 
            // btnDeleteAllMediaFiles
            // 
            this.btnDeleteAllMediaFiles.Location = new System.Drawing.Point(1218, 176);
            this.btnDeleteAllMediaFiles.Name = "btnDeleteAllMediaFiles";
            this.btnDeleteAllMediaFiles.Size = new System.Drawing.Size(116, 23);
            this.btnDeleteAllMediaFiles.TabIndex = 29;
            this.btnDeleteAllMediaFiles.Text = "Delete All MediaFiles";
            this.btnDeleteAllMediaFiles.UseVisualStyleBackColor = true;
            this.btnDeleteAllMediaFiles.Click += new System.EventHandler(this.btnDeleteAllMediaFiles_Click);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1382, 670);
            this.Controls.Add(this.btnDeleteAllMediaFiles);
            this.Controls.Add(this.btnRefreshSubtitleLines);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnBrowseMediaFolder);
            this.Controls.Add(this.txtbxMediaFolder);
            this.Controls.Add(this.btnClearWords);
            this.Controls.Add(this.btnClearCharacters);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.btnDeleteAllFlashCards);
            this.Controls.Add(this.txtbxOutput);
            this.Controls.Add(this.btnClearSubtitles);
            this.Controls.Add(this.btnCreateFlashcards);
            this.Controls.Add(this.btnClearAll);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnAddWordsToDatabase);
            this.Controls.Add(this.btnBrowseAddWordsToDatabase);
            this.Controls.Add(this.txtbxAddWordsToDatabase);
            this.Controls.Add(this.btnAddCharactersToDatabase);
            this.Controls.Add(this.btnBrowseAddCharactersToDatabase);
            this.Controls.Add(this.txtbxAddCharactersToDatabase);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblMovieList);
            this.Controls.Add(this.test);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtbxAddToDatabase);
            this.Controls.Add(this.btnAddToDatabase);
            this.Controls.Add(this.btnBrowseAddToDatabase);
            this.Name = "MainView";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowseAddToDatabase;
        private System.Windows.Forms.Button btnAddToDatabase;
        private System.Windows.Forms.TextBox txtbxAddToDatabase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button test;
        private System.Windows.Forms.Label lblMovieList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtbxAddCharactersToDatabase;
        private System.Windows.Forms.Button btnBrowseAddCharactersToDatabase;
        private System.Windows.Forms.Button btnAddCharactersToDatabase;
        private System.Windows.Forms.TextBox txtbxAddWordsToDatabase;
        private System.Windows.Forms.Button btnBrowseAddWordsToDatabase;
        private System.Windows.Forms.Button btnAddWordsToDatabase;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Button btnCreateFlashcards;
        private System.Windows.Forms.Button btnClearSubtitles;
        private System.Windows.Forms.TextBox txtbxOutput;
        private System.Windows.Forms.Button btnDeleteAllFlashCards;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Button btnClearCharacters;
        private System.Windows.Forms.Button btnClearWords;
        private System.Windows.Forms.TextBox txtbxMediaFolder;
        private System.Windows.Forms.Button btnBrowseMediaFolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRefreshSubtitleLines;
        private System.Windows.Forms.Button btnDeleteAllMediaFiles;
    }
}

