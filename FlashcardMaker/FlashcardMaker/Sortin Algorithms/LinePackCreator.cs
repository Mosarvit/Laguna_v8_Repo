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
        public int GapLimitC { get; set; }
        public int BeforeLimitC { get; set; }
        public int AfterLimitC { get; set; }

        public bool trimByTime = true;
        public bool trimByCharacters = true;


        internal HashSet<SubtitleLinePack> CreateLinePacks( )
        {
            HashSet<List<SubtitleLine>> readableSubtitleLinePacks = SplitInReadableLinePacks(this.Movies);

            HashSet<SubtitleLinePack> trimmedSubtitleLinePacks = trimAccordingToLimits(readableSubtitleLinePacks);

            int totalNumberOfstls = 0;

            foreach (SubtitleLinePack stlp in trimmedSubtitleLinePacks)
            {
                ////DEBUG
                printLine("stlp.SubtitleLines.Count(): " + stlp.SubtitleLines.Count());

                foreach (SubtitleLine stl in stlp.SubtitleLines)
                {
                    ////DEBUG
                    printLine(stl.Position + " " + stl.TimeFrameString + " " + stl.NumberOfToLearnWords);
                    stlp.NumberOfCharacters += stl.NumberOfCharacters;
                    stlp.NumberOfToLearnWords += stl.NumberOfToLearnWords;
                    stlp.ChineseWords.AddRange(stl.ToLearnWords);
                    stlp.Movie = stl.Movie;
                    
                }

                //db.SubtitleLinePacks.Add(stlp);
                totalNumberOfstls += stlp.SubtitleLines.Count();
            }

            //db.SaveChanges();

            double averageNumberOfStls = totalNumberOfstls / trimmedSubtitleLinePacks.Count();

            printLine("trimmedSubtitleLinePacks.Count(): " + trimmedSubtitleLinePacks.Count());
            printLine("totalNumberOfStls: " + totalNumberOfstls);

            printLine("averageNumberOfStls: " + averageNumberOfStls);

            return trimmedSubtitleLinePacks;
        }
        
        private HashSet<SubtitleLinePack> trimAccordingToLimits(HashSet<List<SubtitleLine>> readableStlLists)
        {
            HashSet<SubtitleLinePack> trimmedSubtitleLinePacks = new HashSet<SubtitleLinePack>();

            foreach (List<SubtitleLine> stlList1 in readableStlLists)
            {
                HashSet<List<SubtitleLine>> stlLists = new HashSet<List<SubtitleLine>>();
                List<SubtitleLine> stlList = new List<SubtitleLine>();

                int largestI = 0;
                int smallestI = 0;
                int localBeforeLimitC = BeforeLimitC;
                int localAfterLimitC = AfterLimitC;
                int localBeforeLimitT = BeforeLimit;
                int localAfterLimit = AfterLimit;
                bool firstOne = true;
                int numberOfChars = 0;

                for (int i = 0; i < stlList1.Count(); i++)
                {
                    SubtitleLine stl = stlList1[i];

                    //DEBUG
                    //printLine("Position : " + stl.Position);

                    if (stl.NumberOfToLearnWords > 0)
                    {
                        numberOfChars += stl.NumberOfCharacters;

                        if (firstOne
                            || (trimByCharacters && numberOfChars >= GapLimitC)
                            || (trimByTime && stl.starttime - stlList1[largestI].endtime >= GapLimit))
                        {
                            if (firstOne)
                                firstOne = false;

                            // Create the next LinePack

                            //DEBUG

                            //printLine("new");
                            stlList = new List<SubtitleLine>();
                            stlList.Add(stl);
                            printLine("added as new " + stl.Position);
                            stlLists.Add(stlList);
                            numberOfChars = stl.NumberOfCharacters;

                            smallestI = i;
                        }
                        else
                        {
                            // fill up the Gap 

                            //DEBUG
                            //printLine("Filling gap");
                            for (int k = largestI + 1; k <= i; k++)
                            {
                                //DEBUG
                                stlList.Add(stlList1[k]);
                                printLine("added to fill gap : " + stlList1[k].Position);
                            }
                        }

                        largestI = i;
                    }
                    else
                    {
                        numberOfChars += stl.NumberOfCharacters;
                    }
                }

                ////DEBUG
                //foreach (SubtitleLine stl in stlList1)
                //{
                //    printLine("Position: " + stl.Position + " index: " + stlList1.FindIndex(x => x == stl));
                //}

                foreach (List<SubtitleLine> stlList2 in stlLists)
                {
                    int lastIndex = stlList2.Count() - 1;
                    numberOfChars = 0;

                    foreach (SubtitleLine stl in stlList2)
                    {
                        numberOfChars += stl.NumberOfCharacters;
                    }

                    // fill Up Before and After limits

                    // measure total time

                    int leftOverForBeforeAndAfterC = GapLimitC - numberOfChars;
                    localBeforeLimitC = leftOverForBeforeAndAfterC * BeforeLimitC / (BeforeLimitC + AfterLimitC);
                    localAfterLimitC = leftOverForBeforeAndAfterC * AfterLimitC / (BeforeLimitC + AfterLimitC);

                    int length = stlList2[lastIndex].endtime - stlList2[0].starttime;
                    int leftOverForBeforeAndAfter = GapLimit - length;
                    localBeforeLimitT = leftOverForBeforeAndAfter * BeforeLimit / (BeforeLimit + AfterLimit);
                    localAfterLimit = leftOverForBeforeAndAfter * AfterLimit / (BeforeLimit + AfterLimit);

                    ////DEBUG
                    //printLine("localBeforeLimit: " + localBeforeLimitC);

                    //fill Up localBeforeLimit

                    int numberOfCharsBefore = 0;

                    int j = stlList1.FindIndex(x => x == stlList2[0]) - 1;

                    while (j >= 0)
                    {
                        //    DEBUG
                        //printLine("Filling up before, position: " + stlList1[j].Position);
                        numberOfCharsBefore += stlList1[j].NumberOfCharacters;

                        if ((trimByCharacters && numberOfCharsBefore > localBeforeLimitC)
                            || (trimByTime && stlList2[0].starttime - stlList1[j].starttime > localBeforeLimitT))
                        {
                            break;
                        }
                        else
                        {
                            stlList2.Add(stlList1[j]);
                            printLine("added before : " + stlList1[j].Position);
                        }

                        j--;
                    }

                    //// fill Up localAfterLimit                                

                    int numberOfCharsAfter = 0;
                    stlList2.Sort();
                    
                    j = stlList1.FindIndex(x => x == stlList2[lastIndex]) + 1;

                    while (j < stlList1.Count())
                    {
                        //    DEBUG
                        //printLine("Filling up after, position: " + stlList1[j].Position);
                        numberOfCharsAfter += stlList1[j].NumberOfCharacters;

                        if (    (trimByCharacters   && numberOfCharsBefore > localAfterLimitC)
                            ||  (trimByTime         && stlList1[j].endtime - stlList2[lastIndex].endtime > localBeforeLimitT))
                        {
                            break;
                        }
                        else
                        {
                            stlList2.Add(stlList1[j]);
                            printLine("added after : " + stlList1[j].Position);
                        }

                        j++;
                    }

                }



                foreach (List<SubtitleLine> stlList2 in stlLists)
                {
                    SubtitleLinePack stlp2 = new SubtitleLinePack { SubtitleLines = stlList2 };
                    trimmedSubtitleLinePacks.Add(stlp2);
                }
            }



            return trimmedSubtitleLinePacks;
        }

        private HashSet<List<SubtitleLine>> SplitInReadableLinePacks(List<Movie> Movies)
        {
            HashSet<List<SubtitleLine>> readableStlps = new HashSet<List<SubtitleLine>>();

            int totalCount = 0;
            string str = "Splitting SubtitlelinePacks";
            printLine(str);

            foreach (Movie movie in Movies)
            {
                List<SubtitleLine> stlList = new List<SubtitleLine>();
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
                            stlList = new List<SubtitleLine>();
                            readableStlps.Add(stlList);
                        }

                        
                        stlList.Add(stl);

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
