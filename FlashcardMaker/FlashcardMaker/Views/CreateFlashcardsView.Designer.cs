using System.Windows.Forms;

namespace FlashcardMaker.Views
{
    partial class CreateFlashcardsView
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
            this.txtbxOutput = new System.Windows.Forms.TextBox();
            this.cbSortinAlgorythm = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCreateFlashcards = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.nudBefore = new System.Windows.Forms.NumericUpDown();
            this.nudAfter = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudGap = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.nudGapC = new System.Windows.Forms.NumericUpDown();
            this.nudBeforeC = new System.Windows.Forms.NumericUpDown();
            this.nudAfterC = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnCreateMediaFiles = new System.Windows.Forms.Button();
            this.nudPaddingBefore = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.nudPaddingAfter = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudBefore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAfter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGapC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBeforeC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAfterC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPaddingBefore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPaddingAfter)).BeginInit();
            this.SuspendLayout();
            // 
            // txtbxOutput
            // 
            this.txtbxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbxOutput.Location = new System.Drawing.Point(12, 370);
            this.txtbxOutput.Multiline = true;
            this.txtbxOutput.Name = "txtbxOutput";
            this.txtbxOutput.ReadOnly = true;
            this.txtbxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtbxOutput.Size = new System.Drawing.Size(806, 395);
            this.txtbxOutput.TabIndex = 21;
            this.txtbxOutput.WordWrap = false;
            // 
            // cbSortinAlgorythm
            // 
            this.cbSortinAlgorythm.FormattingEnabled = true;
            this.cbSortinAlgorythm.Location = new System.Drawing.Point(142, 34);
            this.cbSortinAlgorythm.Name = "cbSortinAlgorythm";
            this.cbSortinAlgorythm.Size = new System.Drawing.Size(121, 21);
            this.cbSortinAlgorythm.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Sorting algorithm:";
            // 
            // btnCreateFlashcards
            // 
            this.btnCreateFlashcards.Location = new System.Drawing.Point(220, 298);
            this.btnCreateFlashcards.Name = "btnCreateFlashcards";
            this.btnCreateFlashcards.Size = new System.Drawing.Size(133, 23);
            this.btnCreateFlashcards.TabIndex = 24;
            this.btnCreateFlashcards.Text = "Create Flashcards";
            this.btnCreateFlashcards.UseVisualStyleBackColor = true;
            this.btnCreateFlashcards.Click += new System.EventHandler(this.btnCreateFlashcards_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(13, 789);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(35, 13);
            this.lblStatus.TabIndex = 25;
            this.lblStatus.Text = "label2";
            this.lblStatus.Click += new System.EventHandler(this.label2_Click);
            // 
            // nudBefore
            // 
            this.nudBefore.Location = new System.Drawing.Point(220, 98);
            this.nudBefore.Name = "nudBefore";
            this.nudBefore.Size = new System.Drawing.Size(61, 20);
            this.nudBefore.TabIndex = 26;
            this.nudBefore.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // nudAfter
            // 
            this.nudAfter.Location = new System.Drawing.Point(220, 124);
            this.nudAfter.Name = "nudAfter";
            this.nudAfter.Size = new System.Drawing.Size(61, 20);
            this.nudAfter.TabIndex = 27;
            this.nudAfter.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Max forspan before relevant line";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(45, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(159, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Max extra time after relevant line";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // nudGap
            // 
            this.nudGap.Location = new System.Drawing.Point(220, 72);
            this.nudGap.Name = "nudGap";
            this.nudGap.Size = new System.Drawing.Size(61, 20);
            this.nudGap.TabIndex = 30;
            this.nudGap.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(157, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Max gap between relevant lines";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(287, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "seconds";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(287, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "seconds";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(287, 126);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 34;
            this.label7.Text = "seconds";
            // 
            // nudGapC
            // 
            this.nudGapC.Location = new System.Drawing.Point(413, 72);
            this.nudGapC.Name = "nudGapC";
            this.nudGapC.Size = new System.Drawing.Size(61, 20);
            this.nudGapC.TabIndex = 35;
            this.nudGapC.Value = new decimal(new int[] {
            35,
            0,
            0,
            0});
            // 
            // nudBeforeC
            // 
            this.nudBeforeC.Location = new System.Drawing.Point(413, 98);
            this.nudBeforeC.Name = "nudBeforeC";
            this.nudBeforeC.Size = new System.Drawing.Size(61, 20);
            this.nudBeforeC.TabIndex = 36;
            this.nudBeforeC.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // nudAfterC
            // 
            this.nudAfterC.Location = new System.Drawing.Point(413, 124);
            this.nudAfterC.Name = "nudAfterC";
            this.nudAfterC.Size = new System.Drawing.Size(61, 20);
            this.nudAfterC.TabIndex = 37;
            this.nudAfterC.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(480, 74);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "characters";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(480, 100);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 39;
            this.label9.Text = "characters";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(480, 126);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 13);
            this.label10.TabIndex = 40;
            this.label10.Text = "characters";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(358, 100);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(23, 13);
            this.label11.TabIndex = 41;
            this.label11.Text = "OR";
            // 
            // btnCreateMediaFiles
            // 
            this.btnCreateMediaFiles.Location = new System.Drawing.Point(380, 298);
            this.btnCreateMediaFiles.Name = "btnCreateMediaFiles";
            this.btnCreateMediaFiles.Size = new System.Drawing.Size(133, 23);
            this.btnCreateMediaFiles.TabIndex = 42;
            this.btnCreateMediaFiles.Text = "Create Media Files";
            this.btnCreateMediaFiles.UseVisualStyleBackColor = true;
            this.btnCreateMediaFiles.Click += new System.EventHandler(this.btnCreateMediaFiles_Click);
            // 
            // nudPaddingBefore
            // 
            this.nudPaddingBefore.Location = new System.Drawing.Point(117, 186);
            this.nudPaddingBefore.Name = "nudPaddingBefore";
            this.nudPaddingBefore.Size = new System.Drawing.Size(61, 20);
            this.nudPaddingBefore.TabIndex = 43;
            this.nudPaddingBefore.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(45, 161);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(74, 13);
            this.label12.TabIndex = 44;
            this.label12.Text = "Padding(max):";
            // 
            // nudPaddingAfter
            // 
            this.nudPaddingAfter.Location = new System.Drawing.Point(117, 212);
            this.nudPaddingAfter.Name = "nudPaddingAfter";
            this.nudPaddingAfter.Size = new System.Drawing.Size(61, 20);
            this.nudPaddingAfter.TabIndex = 45;
            this.nudPaddingAfter.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(45, 188);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(38, 13);
            this.label13.TabIndex = 46;
            this.label13.Text = "Before";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(45, 214);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(29, 13);
            this.label14.TabIndex = 47;
            this.label14.Text = "After";
            // 
            // CreateFlashcardsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 810);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.nudPaddingAfter);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.nudPaddingBefore);
            this.Controls.Add(this.btnCreateMediaFiles);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.nudAfterC);
            this.Controls.Add(this.nudBeforeC);
            this.Controls.Add(this.nudGapC);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nudGap);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudAfter);
            this.Controls.Add(this.nudBefore);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCreateFlashcards);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbSortinAlgorythm);
            this.Controls.Add(this.txtbxOutput);
            this.Name = "CreateFlashcardsView";
            this.Text = "CreateFlashcardsView";
            this.Load += new System.EventHandler(this.CreateFlashcardsView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudBefore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAfter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGapC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBeforeC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAfterC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPaddingBefore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPaddingAfter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtbxOutput;
        private ComboBox cbSortinAlgorythm;
        private Label label1;
        private Button btnCreateFlashcards;
        private Label lblStatus;
        private NumericUpDown nudBefore;
        private NumericUpDown nudAfter;
        private Label label2;
        private Label label3;
        private NumericUpDown nudGap;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private NumericUpDown nudGapC;
        private NumericUpDown nudBeforeC;
        private NumericUpDown nudAfterC;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Button btnCreateMediaFiles;
        private NumericUpDown nudPaddingBefore;
        private Label label12;
        private NumericUpDown nudPaddingAfter;
        private Label label13;
        private Label label14;
    }
}