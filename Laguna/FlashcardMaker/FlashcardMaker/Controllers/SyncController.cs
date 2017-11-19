using FlashcardMaker.Helpers;
using FlashcardMaker.Models;
using FlashcardMaker.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashcardMaker.Controllers
{
    public class SyncController : IController
    {
        private ISessionView view;
        private ProgramController programController;

        public SyncController(ISessionView view, ProgramController programController)
        {
            this.view = view;
            this.programController = programController;
        }

        internal void syncronize()
        {
            using (MyDbContext db = new MyDbContext())
            {
                Syncronizer<Flashcard> sfc = new Syncronizer<Flashcard>(view, db);
                sfc.syncronize();

                Syncronizer<MediaFileSegment> smfs = new Syncronizer<MediaFileSegment>(view, db);
                smfs.syncronize();
            }


        }

        public void printLine(string str)
        {
            view.printLine(str);
        }

        public void printStatusLabel(string v)
        {
            view.printStatusLabel(v);
        }
    }
}
