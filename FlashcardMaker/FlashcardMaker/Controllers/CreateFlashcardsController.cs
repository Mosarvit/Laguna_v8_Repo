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
        ProgramController programController;
        private ISortingAlgorithm<ILinePack> sortingAlgorithm;
        private FlashcardCreator flashcardcreator = new FlashcardCreator();

        public CreateFlashcardsController(CreateFlashcardsView createFlashcardsView, ProgramController programController)
        {
            this.view = createFlashcardsView;
            this.programController = programController;
        }

        internal void createFlashcards(string sortingAlgorithmString, int gapLimit, int beforeLimit, int afterLimit, int gapLimitC, int beforeLimitC, int afterLimitC)
        {
            
            using (var db = new MyDbContext())
            {
                view.printLine("Creating flashCards process begins");

                //new DataIOController(view, programController).UpdateSubtitleLines(db);

                view.printLine("Creating flashCards process begins");

                sortingAlgorithm = new SortingAlgorithm1(this);

                if (sortingAlgorithmString.Equals("SorthingAlgorythm1"))
                {
                    sortingAlgorithm = new SortingAlgorithm1(this);
                }

                view.printLine("Deleting all SubtitleLinePacks");

                DeleteAllSubtitleLinePacksInDb();

                view.printLine("Done deleting all SubtitleLinePacks");

                linePackCreator = new LinePackCreator { GapLimit = gapLimit, BeforeLimit = beforeLimit, AfterLimit = afterLimit, GapLimitC = gapLimitC, BeforeLimitC = beforeLimitC, AfterLimitC = afterLimitC, Movies = db.Movies.ToList(), Controller = this };
                HashSet<SubtitleLinePack> newSuptitleLinePacks = linePackCreator.CreateLinePacks();

                //foreach(SubtitleLinePack stlp1 in newSuptitleLinePacks)
                //{
                //    SubtitleLinePack stlp2 =  db.SubtitleLinePacks.Create();
                //    stlp2.ChineseWords.AddRange(stlp1.ChineseWords);
                //    stlp2.Movie = stlp1.Movie;
                //    db.SubtitleLinePacks.Add(stlp2);

                //}

                db.SubtitleLinePacks.AddRange(newSuptitleLinePacks);
                db.SaveChanges();

                //CreateSubtitlePacks();
                List<ILinePack> tempSortSubtitleLinePackList = sortingAlgorithm.SortedSubtitleLinePackList(db);

                view.printLine("Number of stlps: " + tempSortSubtitleLinePackList.Count());

                //flashcardcreator.CreateFlashcards(tempSortSubtitleLinePackList, this);

                view.printLine("Done with creating Flashcards process!\n");

            }
        }

        //private void CreateSubtitlePacks()
        //{

            

            

        //    int totalCount = 0;

        //    using (MyDbContext db = new MyDbContext())
        //    {
        //        printLine("Creating SubtitlelinePacks");
        //        printLine("Number of SubtitlelinePacks: " + db.SubtitleLinePacks.Count());

        //        foreach (Movie movie in db.Movies.ToList())
        //        {
        //            IList<SubtitleLine> allSubtitleLineList = movie.SubtitleLines.OrderBy(c => c.Position).ToList();
        //            SubtitleLinePack stlp = new SubtitleLinePack { SubtitleLines = new List<SubtitleLine>(), ChineseWords = new List<ChineseWord>(), AddedToFlashcards = false };
        //            db.SubtitleLinePacks.Add(stlp);
        //            bool firstZero = false;

        //            foreach (SubtitleLine stl in allSubtitleLineList)
        //            {
        //                if (stl.CanRead == 0 && firstZero)
        //                {
        //                    db.SubtitleLinePacks.Add(stlp);
        //                    stlp = new SubtitleLinePack { SubtitleLines = new List<SubtitleLine>(), ChineseWords = new List<ChineseWord>() };
        //                    firstZero = false;
        //                }
        //                else if (stl.CanRead == 1)
        //                {
        //                    firstZero = true;
        //                    stlp.SubtitleLines.Add(stl);
        //                    stlp.ChineseWords.AddRange(stl.ToLearnWords);
        //                    stlp.NumberOfCharacters += stl.NumberOfCharacters;
        //                }

        //                view.printStatusLabel("SubtitleLines analyzed: " + totalCount++);

        //                if (ProgramController.DEBUGGING == true && totalCount > ProgramController.MAX_SUBTITLES_TO_UPDATE)
        //                {
        //                    db.SaveChanges();
        //                    return;
        //                }
        //            }
        //        }

        //        db.SaveChanges();
        //    }



        //    printLine("Done Creating SubtitlelinePacks");
        //}

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
