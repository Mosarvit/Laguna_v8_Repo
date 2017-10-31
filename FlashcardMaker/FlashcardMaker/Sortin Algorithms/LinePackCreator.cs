using FlashcardMaker.Controllers;
using FlashcardMaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Sortin_Algorithms
{
    public class LinePackCreator
    {
        public IController Controller { get; set; }
        public List<Movie> Movies { get; set; }
        public int GapLimit { get; set; }
        public int BeforeLimit { get; set; }
        public int AfterLimit { get; set; }

        internal void CreateLinePacks(MyDbContext db)
        {
            HashSet<SubtitleLinePack> readableSubtitleLinePacks = SplitIntorReadableLinePacks(this.Movies, db);
            HashSet<SubtitleLinePack> trimmedSubtitleLinePacks = trimAccordingToBeforeAfterAndGapLimits(readableSubtitleLinePacks);

            foreach (SubtitleLinePack stlp in trimmedSubtitleLinePacks)
            {
                printLine("next");

                foreach (SubtitleLine spl in stlp.SubtitleLines)
                {
                    printLine(spl.Position + " " + spl.TimeFrameString + " " + spl.NumberOfToLearnWords);
                }
            }


            trimmedSubtitleLinePacks.Count();


        }

        private HashSet<SubtitleLinePack> trimAccordingToBeforeAfterAndGapLimits(HashSet<SubtitleLinePack> readableSubtitleLinePacks)
        {
            HashSet<SubtitleLinePack> trimmedSubtitleLinePacks = new HashSet<SubtitleLinePack>();

            foreach (SubtitleLinePack stlp in readableSubtitleLinePacks)
            {
                SortedSet<SubtitleLine> relevantStls = new SortedSet<SubtitleLine>();
                List<SubtitleLine> stls = stlp.SubtitleLines;
                SubtitleLinePack stlp2 = new SubtitleLinePack { SubtitleLines = new List<SubtitleLine>() };

                long largestEndTime = 0;
                int largestI = 0;
                bool firstOne = true;

                for (int i = 0; i < stls.Count(); i++)
                {
                    SubtitleLine stl = stls[i];

                    printLine("Position : " + stl.Position);

                    if (stl.NumberOfToLearnWords > 0)
                    {
                        if (firstOne)
                        {
                            firstOne = false;

                            relevantStls.Add(stl);
                            // add All to Fill up the BeforeLimit
                            int j = i;
                            while (stl.starttime - stls[j].starttime > BeforeLimit && j >= 0)
                            {
                                relevantStls.Add(stls[j]);
                                j--;
                            }

                            stlp2 = new SubtitleLinePack { SubtitleLines = new List<SubtitleLine>() };
                            trimmedSubtitleLinePacks.Add(stlp2);
                            stlp2.SubtitleLines.Add(stl);
                        }
                        else
                        {
                            if (stl.starttime - largestEndTime <= GapLimit)
                            {
                                // Add all Lines inside the gap

                                printLine("Fill gap");
                                for (int k = largestI + 1; k < i; k++)
                                {
                                    relevantStls.Add(stls[k]);
                                }

                                stlp2.SubtitleLines.Add(stl);
                                printLine(relevantStls.Count() + "");
                            }
                            else
                            {
                                // Create the next LinePack
                                stlp2 = new SubtitleLinePack { SubtitleLines = new List<SubtitleLine>() };
                                trimmedSubtitleLinePacks.Add(stlp2);
                                stlp2.SubtitleLines.Add(stl);
                            }
                        }

                        largestEndTime = stl.endtime;
                        largestI = i;
                    }
                    else
                    {
                        if (stl.endtime - largestEndTime <= AfterLimit)
                        {
                            stlp2.SubtitleLines.Add(stl);
                        }
                    }
                }
            }

            return trimmedSubtitleLinePacks;
        }

        private HashSet<SubtitleLinePack> SplitIntorReadableLinePacks(List<Movie> Movies, MyDbContext db)
        {
            HashSet<SubtitleLinePack> readableStlps = new HashSet<SubtitleLinePack>();

            int totalCount = 0;
            string str = "Splitting SubtitlelinePacks";
            printLine(str);
            printLine("Number of SubtitlelinePacks ind Database: " + db.SubtitleLinePacks.Count());

            foreach (Movie movie in Movies)
            {
                SubtitleLinePack stlp = new SubtitleLinePack();
                bool firstOne = true;

                printLine("movie.SubtitleLines.Count() : " + movie.SubtitleLines.Count());

                foreach (SubtitleLine stl in movie.SubtitleLines)
                {
                    if (stl.CanRead == 0)
                    {
                        firstOne = true;
                    }
                    else if (stl.CanRead == 1)
                    {
                        if (firstOne)
                        {
                            stlp = new SubtitleLinePack { SubtitleLines = new List<SubtitleLine>(), ChineseWords = new List<ChineseWord>() };
                            readableStlps.Add(stlp);
                        }

                        stlp.SubtitleLines.Add(stl);
                        stlp.ChineseWords.AddRange(stl.ToLearnWords);
                        stlp.NumberOfCharacters += stl.NumberOfCharacters;

                        firstOne = false;
                    }

                    Controller.printStatusLabel("SubtitleLines analyzed: " + totalCount++);

                    if (ProgramController.DEBUGGING == true && totalCount > ProgramController.MAX_SUBTITLES_TO_UPDATE_TO_USE_FOR_PACKS)
                    {
                        break;
                    }
                }
            }

            Controller.printLine("Number of readable SubtitlelinePacks found: " + readableStlps.Count());

            return readableStlps;
        }

        private void printLine(string str)
        {
            Controller.printLine(str);
        }
    }
}
