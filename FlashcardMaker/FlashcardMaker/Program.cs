using FlashcardMaker.Controllers;
using FlashcardMaker.Views;
//using FlashcardMaker.Migrations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashcardMaker
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainView mainForm = new MainView();

            DataIOController mainFormController = new DataIOController(mainForm);
            ProgramController programController = new ProgramController(mainForm, mainFormController);
                       
            mainForm.setDataIOController(mainFormController);
            mainForm.setProgramController(programController);
            Application.Run(mainForm);


            
        }
    }
}