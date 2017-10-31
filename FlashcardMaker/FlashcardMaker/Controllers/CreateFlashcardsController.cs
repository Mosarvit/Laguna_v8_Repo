using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashcardMaker.Views;
using FlashcardMaker.Sortin_Algorithms;
using FlashcardMaker.Models;
using FlashcardMaker.Interfaces;
using System.Diagnostics;
using FlashcardMaker.Helpers;

namespace FlashcardMaker.Controllers
{
    public class CreateFlashcardsController : IController
    {
        private LinePackCreator linePackCreator;
        private ISessionView view;
        private ISortingAlgorithm<ILinePack> sortingAlgorithm;
        private FlashcardCreator flashcardcreator = new FlashcardCreator();

        public CreateFlashcardsController(CreateFlashcardsView createFlashcardsView)
        {
            this.view = createFlashcardsView;
        }

        internal void createFlashcards(string sortingAlgorithmString, int gap, int before, int after)
        {
            sortingAlgorithm = new SortingAlgorithm1(this);

            if (sortingAlgorithmString.Equals("SorthingAlgorythm1"))
            {
                sortingAlgorithm = new SortingAlgorithm1(this);
            }


            view.printLine("Creating flashCards process begins");

            var db = new MyDbContext();

            linePackCreator = new LinePackCreator { GapLimit = gap, BeforeLimit = before, AfterLimit = after, Movies = db.Movies.ToList(), Controller = this};
            linePackCreator.CreateLinePacks(db);

            //CreateSubtitlePacks();
            List<ILinePack> tempSortSubtitleLinePackList = sortingAlgorithm.SortedSubtitleLinePackList();

            view.printLine("Number of stlps: " + tempSortSubtitleLinePackList.Count());

            //flashcardcreator.CreateFlashcards(tempSortSubtitleLinePackList, this);

            view.printLine("Done with creating Flashcards process!\n");
        }

        private void CreateSubtitlePacks()
        {

            

            DeleteAllSubtitleLinePacksInDb();

            int totalCount = 0;

            using (MyDbContext db = new MyDbContext())
            {
                printLine("Creating SubtitlelinePacks");
                printLine("Number of SubtitlelinePacks: " + db.SubtitleLinePacks.Count());

                foreach (Movie movie in db.Movies.ToList())
                {
                    IList<SubtitleLine> allSubtitleLineList = movie.SubtitleLines.OrderBy(c => c.Position).ToList();
                    SubtitleLinePack stlp = new SubtitleLinePack { SubtitleLines = new List<SubtitleLine>(), ChineseWords = new List<ChineseWord>(), AddedToFlashcards = false };
                    db.SubtitleLinePacks.Add(stlp);
                    bool firstZero = false;

                    foreach (SubtitleLine stl in allSubtitleLineList)
                    {
                        if (stl.CanRead == 0 && firstZero)
                        {
                            db.SubtitleLinePacks.Add(stlp);
                            stlp = new SubtitleLinePack { SubtitleLines = new List<SubtitleLine>(), ChineseWords = new List<ChineseWord>() };
                            firstZero = false;
                        }
                        else if (stl.CanRead == 1)
                        {
                            firstZero = true;
                            stlp.SubtitleLines.Add(stl);
                            stlp.ChineseWords.AddRange(stl.ToLearnWords);
                            stlp.NumberOfCharacters += stl.NumberOfCharacters;
                        }

                        view.printStatusLabel("SubtitleLines analyzed: " + totalCount++);

                        if (ProgramController.DEBUGGING == true && totalCount > ProgramController.MAX_SUBTITLES_TO_UPDATE)
                        {
                            db.SaveChanges();
                            return;
                        }
                    }
                }

                db.SaveChanges();
            }



            printLine("Done Creating SubtitlelinePacks");
        }

        private static void DeleteAllSubtitleLinePacksInDb()
        {
            using (MyDbContext db = new MyDbContext())
            {
                foreach (SubtitleLinePack stlp in db.SubtitleLinePacks.ToList())
                {
                    //printLine("Clearing, Id: " + stlp.Id + " " + stlp.NumberOfCharacters + " " + stlp.NumberOfToLearnWords);
                    foreach (var stl in stlp.SubtitleLines.ToList())
                    {
                        stlp.SubtitleLines.Remove(stl);
                    }

                    foreach (var fc in stlp.Flashcards.ToList())
                    {
                        stlp.Flashcards.Remove(fc);
                        //db.Flashcards.Remove(fc);
                    }

                    db.SubtitleLinePacks.Remove(stlp);
                }
                db.SaveChanges();

                db.SubtitleLinePacks.RemoveRange(db.SubtitleLinePacks);
                db.SaveChanges();
            }

        }








        public void printLine(string str)
        {
            view.printLine(str);
        }

        public void printStatusLabel(string str)
        {
            view.printStatusLabel(str);
        }
        

        //void IController.printLine(string str)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
