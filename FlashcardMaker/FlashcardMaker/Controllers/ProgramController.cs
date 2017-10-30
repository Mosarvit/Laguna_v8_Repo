using FlashcardMaker.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashcardMaker.Controllers
{
    public class ProgramController
    {
        private MainView mainForm;

        public ProgramController(MainView mainForm)
        {
            this.mainForm = mainForm;
        }

        public void startSyncSession()
        {
            SyncView syncView = new SyncView();
            SyncController syncController = new SyncController(syncView);
            syncView.setSyncController(syncController);

            syncView.StartPosition = FormStartPosition.Manual;
            syncView.Location = new Point(10, 10);
            syncView.Show();

            syncController.sync();
        }
    }
}
