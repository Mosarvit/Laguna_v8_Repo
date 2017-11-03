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
        public static bool DEBUGGING_CREATEFLASHCARDS = false;
        public static bool DEBUGGING_VIDEO_EDITOR = false;
        public static bool DEBUGGING_MEDIA_FILES = false;

        public static int MAX_CHINESE_CHARACTERS_TO_LOAD = 15000;
        public static int MAX_CHINESE_WORDS_TO_LOAD = 15000;
        public static int MAX_SUBTITLES_TO_LOAD = 50;
        public static int MAX_SUBTITLES_TO_UPDATE_TO_USE_FOR_PACKS = 500;
        public static int MAX_MEDIA_FILES_TO_CREATE = 1;

        private ISessionView view;
        private DataIOController mainFormController;

        public ProgramController(ISessionView view)
        {
            this.view = view;
        }

        public void startSyncSession()
        {
            SyncView syncView = new SyncView();
            SyncController syncController = new SyncController(syncView, this);
            syncView.setSyncController(syncController);

            syncView.StartPosition = FormStartPosition.Manual;
            syncView.Location = new Point(10, 10);
            syncView.Show();

            syncController.syncronize();
        }

        internal void StartCreatingFlashcardsSession()
        {
            CreateFlashcardsView createFlashcardsView = new CreateFlashcardsView();
            CreateFlashcardsController createFlashcardsController = new CreateFlashcardsController(createFlashcardsView, this);
            createFlashcardsView.setSyncController(createFlashcardsController);

            createFlashcardsView.StartPosition = FormStartPosition.Manual;
            createFlashcardsView.Location = new Point(10, 10);
            createFlashcardsView.Show();
        }

        public void printLine(string str)
        {
            view.printLine(str);
        }

        internal string askIfChineseCharacter(string cString)
        {
            UnknownCharacterView uc = new UnknownCharacterView(cString);
            uc.StartPosition = FormStartPosition.Manual;
            uc.Location = new Point(10, 10);
            uc.ShowDialog();

            return uc.answer;
        }

        internal static void startMediaCreationSession(ISessionView view)
        {
            MediaFilesController mediaFilesController = new MediaFilesController(view = view);
            mediaFilesController.creatMediaFiles();
        }

        internal static string decideAboutConradictions()
        {
            DecisionAboutContradictionsView dacv = new DecisionAboutContradictionsView();
            dacv.StartPosition = FormStartPosition.Manual;
            dacv.Location = new Point(10, 10);
            dacv.ShowDialog();

            return dacv.answer;
        }
    }
}
