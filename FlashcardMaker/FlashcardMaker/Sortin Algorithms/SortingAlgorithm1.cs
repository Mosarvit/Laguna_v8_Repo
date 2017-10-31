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

        public SortingAlgorithm1(IController controller)
        {
            this.controller = controller;
        }

        public List<ILinePack> SortedSubtitleLinePackList()
        {
            using(MyDbContext db = new MyDbContext())
            {
                controller.printLine("Sorting SubtitlelinePacks");

                var tempStlpList = new List<ILinePack>();

                UpdateStlps(db);

                List<SubtitleLinePack> stlpList = db.SubtitleLinePacks.OrderByDescending(c => c.DensityOfToLearnWords).ToList();

                if (stlpList.Count() == 0)
                {
                    controller.printLine("No SubtitlePacks in the Database, please fill contents first.");
                    return tempStlpList;
                }

                SubtitleLinePack mostDenseStlp = stlpList[0];

                int counter = 0;

                while (mostDenseStlp.DensityOfToLearnWords > 0 && counter++ < 10)
                {
                    foreach (var cw in mostDenseStlp.ChineseWords)
                        cw.AddedToTempSort = true;

                    db.SaveChanges();

                    tempStlpList.Add(mostDenseStlp);

                    UpdateStlps(db);

                    db.SaveChanges();

                    mostDenseStlp = db.SubtitleLinePacks.OrderByDescending(c => c.DensityOfToLearnWords).ToList()[0]; ;
                }

                foreach (var cw in db.ChineseWords.ToList())
                    cw.AddedToTempSort = false;

                db.SaveChanges();

                controller.printLine("Finished sorting SubtitlelinePacks");

                return tempStlpList;
            }                 
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
                stlp.DensityOfToLearnWords = (double)stlp.NumberOfNotYetInTempSortWords / (double)stlp.NumberOfCharacters;
            }
            else
            {
                stlp.DensityOfToLearnWords = 0;
            }
        }

        public void printLine(string str)
        {
            controller.printLine(str);
        }
    }
}
