using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashcardMaker.Models;
using FlashcardMaker.Interfaces;
using FlashcardMaker.Controllers;

namespace FlashcardMaker.Sortin_Algorithms
{
    class SortingAlgorithm1 : ISortingAlgorithm<ILinePack>
    {
        private IController controller;
        private int ImportanceOfDensity;

        public SortingAlgorithm1(IController controller)
        {
            this.controller = controller;
        }

        public List<ILinePack> SortedSubtitleLinePackList(MyDbContext db, int importanceOfDensity)
        {
            controller.printLine("Sorting SubtitlelinePacks");

            this.ImportanceOfDensity = importanceOfDensity;

            db.SubtitleLinePacks.ToList().ForEach(delegate (SubtitleLinePack stlp) { stlp.Rank = 0; });

            //foreach (SubtitleLinePack stlp in db.SubtitleLinePacks.ToList())
            //{
            //    stlp.Rank = 0;
            //}

            db.SaveChanges();

            var tempStlpList = new List<ILinePack>();

            UpdateStlps(db);

            List<SubtitleLinePack> stlpList = db.SubtitleLinePacks.OrderByDescending(c => c.importance).ToList();

            if (stlpList.Count() == 0)
            {
                controller.printLine("No SubtitlePacks in the Database, please fill contents first.");
                return tempStlpList;
            }

            SubtitleLinePack mostDenseStlp = stlpList[0];
             
            int rank = 1;

            while (mostDenseStlp.importance > 0 && rank < Properties.Settings.Default.DEBUG_MAX_SORT_SUBTITLE_LINEPACKS)
            {
                foreach (var cw in mostDenseStlp.ChineseWords)
                    cw.AddedToTempSort = true;

                db.SaveChanges();

                tempStlpList.Add(mostDenseStlp);
                mostDenseStlp.Rank = rank++;

                UpdateStlps(db);

                db.SaveChanges();

                mostDenseStlp = db.SubtitleLinePacks.OrderByDescending(c => c.importance).ToList()[0]; ;
            }

            foreach (var cw in db.ChineseWords.ToList())
                cw.AddedToTempSort = false;

            db.SaveChanges();

            controller.printLine("Finished sorting SubtitlelinePacks");

            return tempStlpList;

        }

        private void UpdateStlps(MyDbContext db)
        {

            foreach (SubtitleLinePack stlp in db.SubtitleLinePacks.ToList())
            {
                UpdateStlpsNumberOfNotYetInTempSortWords(stlp, db);
                UpdateStlpsDensity(stlp, db);
            }

            db.SaveChanges();
        }

        private static void UpdateStlpsNumberOfNotYetInTempSortWords(SubtitleLinePack stlp, MyDbContext db)
        {
            int notYetCount = 0;

            foreach (ChineseWord cw in stlp.ChineseWords.ToList())
            {
                if (!cw.AddedToTempSort)
                    notYetCount++;
            }

            stlp.NumberOfNotYetInTempSortWords = notYetCount;
        }

        private void UpdateStlpsDensity(SubtitleLinePack stlp, MyDbContext db)
        {
            if (stlp.NumberOfCharacters != 0)
            {
                stlp.importance = ((double)ImportanceOfDensity 
                    * ((double)stlp.NumberOfNotYetInTempSortWords / (double)stlp.NumberOfCharacters)) 
                    + (double)stlp.NumberOfNotYetInTempSortWords;
            }
            else
            {
                stlp.importance = 0;
            }
        }

        public void printLine(string str)
        {
            controller.printLine(str);
        }
    }
}
