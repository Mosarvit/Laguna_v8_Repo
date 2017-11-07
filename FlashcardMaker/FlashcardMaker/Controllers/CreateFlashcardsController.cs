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

        internal void createSubtitleLinePacks(string sortingAlgorithmString, int gapLimit, int beforeLimit, int afterLimit, int gapLimitC, int beforeLimitC, int afterLimitC, int paddingBefore, int paddingAfter)
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

                var allMovies = db.Movies;

                var relevantStls = (from SubtitleLine in db.SubtitleLines
                                    where allMovies.Contains(SubtitleLine.Movie)
                                    select SubtitleLine).ToList();


                linePackCreator = new LinePackCreator
                {
                    GapLimit = gapLimit,
                    BeforeLimit = beforeLimit,
                    AfterLimit = afterLimit,

                    GapLimitC = gapLimitC,
                    BeforeLimitC = beforeLimitC,
                    AfterLimitC = afterLimitC,

                    PaddingBefore = paddingBefore,
                    PaddingAfter = paddingAfter,

                    Movies = allMovies.ToList(),
                    SubtitleLines = relevantStls,
                    Controller = this
                };
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

        internal void createFlashcards(CreateFlashcardsView createFlashcardsView)
        {
            using (MyDbContext db = new MyDbContext())
            {
                foreach (SubtitleLinePack stlp in db.SubtitleLinePacks.ToList())
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (SubtitleLine stl in stlp.SubtitleLines)
                    {
                        sb.Append(stl.Chinese + "\n");
                    }
                    String question = sb.ToString();

                    Flashcard fc = Factory.InsertFlashcard(db, view, question, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), true);
                    if (stlp.MediaFileSegments_remote_id != null)
                    {
                        fc.MediaFileSegment_remote_id = stlp.MediaFileSegments_remote_id;
                        fc.MediaFileSegment_remote_id = stlp.MediaFileSegments_remote_id;
                    }
                }
            }
            
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
