namespace FlashcardMaker.Views
{
    partial class SyncView
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtbxOutput
            // 
            this.txtbxOutput.Location = new System.Drawing.Point(12, 12);
            this.txtbxOutput.Multiline = true;
            this.txtbxOutput.Name = "txtbxOutput";
            this.txtbxOutput.ReadOnly = true;
            this.txtbxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtbxOutput.Size = new System.Drawing.Size(733, 352);
            this.txtbxOutput.TabIndex = 20;
            this.txtbxOutput.WordWrap = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(344, 386);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SyncView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 421);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtbxOutput);
            this.Name = "SyncView";
            this.Text = "SyncView";
            this.Load += new System.EventHandler(this.SyncView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtbxOutput;
        private System.Windows.Forms.Button btnCancel;
    }
}