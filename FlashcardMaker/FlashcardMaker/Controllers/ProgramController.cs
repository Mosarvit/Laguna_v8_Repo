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
        public static bool DEBUGGING = true;
        public static bool DEBUGGING_CREATEFLASHCARDS = true;

        public static int MAX_CHINESE_CHARACTERS_TO_LOAD = 15000;
        public static int MAX_CHINESE_WORDS_TO_LOAD = 15000;
        public static int MAX_SUBTITLES_TO_UPDATE = 0;
        public static int MAX_SUBTITLES_TO_LOAD = 500;
        public static int MAX_SUBTITLES_TO_UPDATE_TO_USE_FOR_PACKS = 500;

        private ISessionView view;
        private DataIOController mainFormController;

        public ProgramController(MainView mainForm, DataIOController mainFormController)
        {
            this.view = mainForm;
            this.mainFormController = mainFormController;
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

        internal void StartCreatingFlashcardsSession()
        {
            this.mainFormController.UpdateCanReadInSubtitleLines();

            CreateFlashcardsView createFlashcardsView = new CreateFlashcardsView();
            CreateFlashcardsController createFlashcardsController = new CreateFlashcardsController(createFlashcardsView);
            createFlashcardsView.setSyncController(createFlashcardsController);

            createFlashcardsView.StartPosition = FormStartPosition.Manual;
            createFlashcardsView.Location = new Point(10, 10);
            createFlashcardsView.Show();
        }

        public void printLine(string str)
        {
            view.printLine(str);
        }
    }
}
