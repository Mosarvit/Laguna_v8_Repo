namespace FlashcardMaker.Views
{
    partial class UnknownCharacterView
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
            this.lblQuestion = new System.Windows.Forms.Label();
            this.btnAddToNoneChineseCharacters = new System.Windows.Forms.Button();
            this.btnAddToChineseCharacters = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblQuestion
            // 
            this.lblQuestion.AutoSize = true;
            this.lblQuestion.Location = new System.Drawing.Point(85, 88);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(71, 13);
            this.lblQuestion.TabIndex = 0;
            this.lblQuestion.Text = "No messages";
            // 
            // btnAddToNoneChineseCharacters
            // 
            this.btnAddToNoneChineseCharacters.AllowDrop = true;
            this.btnAddToNoneChineseCharacters.Location = new System.Drawing.Point(45, 180);
            this.btnAddToNoneChineseCharacters.Name = "btnAddToNoneChineseCharacters";
            this.btnAddToNoneChineseCharacters.Size = new System.Drawing.Size(246, 23);
            this.btnAddToNoneChineseCharacters.TabIndex = 1;
            this.btnAddToNoneChineseCharacters.Text = "Add to non-Chinese Characters";
            this.btnAddToNoneChineseCharacters.UseVisualStyleBackColor = true;
            this.btnAddToNoneChineseCharacters.Click += new System.EventHandler(this.btnAddToNoneChineseCharacters_Click);
            // 
            // btnAddToChineseCharacters
            // 
            this.btnAddToChineseCharacters.Location = new System.Drawing.Point(45, 151);
            this.btnAddToChineseCharacters.Name = "btnAddToChineseCharacters";
            this.btnAddToChineseCharacters.Size = new System.Drawing.Size(246, 23);
            this.btnAddToChineseCharacters.TabIndex = 2;
            this.btnAddToChineseCharacters.Text = "Add to Chinese Characters";
            this.btnAddToChineseCharacters.UseVisualStyleBackColor = true;
            this.btnAddToChineseCharacters.Click += new System.EventHandler(this.btnAddToChineseCharacters_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(45, 209);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(246, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // UnknownCharacterView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 328);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAddToChineseCharacters);
            this.Controls.Add(this.btnAddToNoneChineseCharacters);
            this.Controls.Add(this.lblQuestion);
            this.Name = "UnknownCharacterView";
            this.Text = "unknownCharacter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblQuestion;
        private System.Windows.Forms.Button btnAddToNoneChineseCharacters;
        private System.Windows.Forms.Button btnAddToChineseCharacters;
        private System.Windows.Forms.Button btnCancel;
    }
}