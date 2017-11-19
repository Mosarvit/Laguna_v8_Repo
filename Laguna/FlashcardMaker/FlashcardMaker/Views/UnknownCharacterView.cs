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
    public partial class UnknownCharacterView : Form
    {
        public string answer = "Cancel";
        

        public UnknownCharacterView(string unknownCharacter)
        {
            InitializeComponent();
            lblQuestion.Text = "\"" + unknownCharacter + "\" is an unknown Character. What should be done?";
        }

        private void btnAddToNoneChineseCharacters_Click(object sender, EventArgs e)
        {
            this.answer = "AddToNoneChineseCharacters";
            this.Close();
        }

        private void btnAddToChineseCharacters_Click(object sender, EventArgs e)
        {
            this.answer = "AddToChineseCharacters";
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.answer = "Cancel";
            this.Close();
        }
    }
}
