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

        internal void createSubtitleLinePacks(int gapLimit, int beforeLimit, int afterLimit, int gapLimitC, int beforeLimitC, int afterLimitC, int paddingBefore, int paddingAfter)
        {

            using (var db = new MyDbContext())
            {
                view.printLine("Creating flashCards process begins");

                //new DataIOController(view, programController).UpdateSubtitleLines(db);

                view.printLine("Creating flashCards process begins");

                

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


                //flashcardcreator.CreateFlashcards(tempSortSubtitleLinePackList, this);


            }
        }

        internal void SortSubtitleLinePacks(CreateFlashcardsView createFlashcardsView, String sortingAlgorithmString, int importanceOfDensity)
        {
            sortingAlgorithm = new SortingAlgorithm1(this);

            if (sortingAlgorithmString.Equals("SorthingAlgorythm1"))
            {
                sortingAlgorithm = new SortingAlgorithm1(this);
            }

            using (var db = new MyDbContext())
            {
                List<ILinePack> tempSortSubtitleLinePackList = sortingAlgorithm.SortedSubtitleLinePackList(db, importanceOfDensity);

                view.printLine("Number of stlps: " + tempSortSubtitleLinePackList.Count());
            }
        }

        internal void CreateFlashcards(CreateFlashcardsView createFlashcardsView)
        {
            using (MyDbContext db = new MyDbContext())
            {



                int counter = 0;

                foreach (SubtitleLinePack stlp in db.SubtitleLinePacks.Where(p=>p.Rank>0).ToList())
                {
                    counter++;

                    if (counter > Properties.Settings.Default.DEBUG_MAX_CREATE_FLASHCARDS && Properties.Settings.Default.DEBUG_MAX_CREATE_FLASHCARDS!=0)
                    {
                        view.printLine("achieved MAX_CREATE_FLASHCARDS");
                        break;
                    }

                    if (Properties.Settings.Default.DEBUG_ONLY_CREATE_FLASHCARDS_WITH_MEDIAFILES && stlp.MediaFileSegments_remote_id == 0)
                    {
                        continue;
                    }
                    
                    String question = CreateQuestion(stlp);

                    Flashcard fc = Factory.InsertFlashcard(db, view, question, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), true);
                    if (stlp.MediaFileSegments_remote_id != 0)
                    {
                        fc.MediaFileSegment_remote_id = stlp.MediaFileSegments_remote_id;
                    }

                    
                }

                db.SaveChanges();
                printLine("Number of Flashcards created: " + counter);
            }

        }

        private string CreateQuestion(SubtitleLinePack stlp)
        {
            string header = "";
            string footer = "</div ></body></html>";

            string cwSpanS = "<span id=toLearnWord>";
            string cwSPanE = "</span>";

            string chineseDivS = "<span id=chinese>";
            string chineseDivE = "</span>";            
            string tanslitDivS = "<div id=translit>";
            string tanslitDivE = "</div>";
            string englishDivS = "<div id=english>";
            string englishDivE = "</div>";

            StringBuilder sb = new StringBuilder();
            sb.Append(header);

            foreach (SubtitleLine stl in stlp.SubtitleLines)
            {


                sb.Append(tanslitDivS+" - " + stl.Translit + tanslitDivE);

                String chineseLine = stl.Chinese;
                view.printLine(chineseLine);
                foreach (ChineseWord cw in stl.ToLearnWords)
                {
                    view.printLine(cw.Chinese);
                    int sIndex = chineseLine.IndexOf(cw.Chinese);
                    int eIndex = sIndex + cw.Chinese.Length;
                    chineseLine = chineseLine.Substring(0, sIndex) + cwSpanS + cw.Chinese + cwSPanE + chineseLine.Substring(eIndex, chineseLine.Length - eIndex);
                } 

                sb.Append(chineseDivS + " - " + chineseLine + chineseDivE);

                sb.Append(englishDivS+" - " + stl.English + englishDivE);
            }

            sb.Append(footer);

            printLine(sb.ToString());

            return sb.ToString();
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
