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
        public static bool LOCAL_DEBUG = false;
        public static int EXTRA_PADDING_CUT = 30;

        public IController Controller { get; set; }
        public List<Movie> Movies { get; set; }
        public int GapLimit { get; set; }
        public int BeforeLimit { get; set; }
        public int AfterLimit { get; set; }
        public int GapLimitC { get; set; }
        public int BeforeLimitC { get; set; }
        public int AfterLimitC { get; set; }
        public int PaddingBefore { get; internal set; }
        public int PaddingAfter { get; internal set; }
        public List<SubtitleLine> SubtitleLines { get; internal set; }

        public bool trimByTime = true;
        public bool trimByCharacters = true;


        internal HashSet<SubtitleLinePack> CreateLinePacks()
        {
            HashSet<List<SubtitleLine>> readableSubtitleLinePacks = SplitInReadableLinePacks(this.Movies);

            if (LOCAL_DEBUG)
            {
                foreach (var stlList in readableSubtitleLinePacks)
                {
                    printLine("next");

                    foreach (var stl in stlList)
                    {
                        printLine("" + stl.Position);
                    }
                }
            }




            HashSet<SubtitleLinePack> trimmedSubtitleLinePacks = trimAccordingToLimits(readableSubtitleLinePacks);

            int totalNumberOfstls = 0;

            foreach (SubtitleLinePack stlp in trimmedSubtitleLinePacks)
            {
                ////DEBUG
                printLine("stlp.SubtitleLines.Count(): " + stlp.SubtitleLines.Count());

                foreach (SubtitleLine stl in stlp.SubtitleLines)
                {
                    ////DEBUG
                    printLine(stl.Position + " " + stl.starttime + " - " + stl.endtime + " " + stl.Chinese);
                    stlp.NumberOfCharacters += stl.NumberOfCharacters;
                    stlp.NumberOfToLearnWords += stl.NumberOfToLearnWords;
                    stlp.ChineseWords.AddRange(stl.ToLearnWords);
                    stlp.Movie = stl.Movie;

                }

                //db.SubtitleLinePacks.Add(stlp);
                totalNumberOfstls += stlp.SubtitleLines.Count();

                //if (LOCAL_DEBUG)
                //{
                    printLine("startTime : " + stlp.StartTime);
                    printLine("endTime : " + stlp.EndTime);
                //}


            }

            //db.SaveChanges();

            double averageNumberOfStls = 0;

            if (trimmedSubtitleLinePacks.Count() > 0)
            {
                averageNumberOfStls = totalNumberOfstls / trimmedSubtitleLinePacks.Count();
            }
            else
            {
                averageNumberOfStls = 0;
            }            

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
                            if (LOCAL_DEBUG)
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
                                stlList.Add(stlList1[k]);
                                if (LOCAL_DEBUG)
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





                    //// fill Up localAfterLimit                                

                    int numberOfCharsAfter = 0;


                    int j = stlList1.FindIndex(x => x == stlList2[lastIndex]) + 1;

                    while (j < stlList1.Count())
                    {
                        //    DEBUG
                        //printLine("Filling up after, position: " + stlList1[j].Position);
                        numberOfCharsAfter += stlList1[j].NumberOfCharacters;

                        if ((trimByCharacters && numberOfCharsAfter > localAfterLimitC)
                            || (trimByTime && stlList1[j].endtime - stlList2[lastIndex].endtime > localBeforeLimitT))
                        {
                            break;
                        }
                        else
                        {
                            stlList2.Add(stlList1[j]);
                            if (LOCAL_DEBUG)
                                printLine("added after : " + stlList1[j].Position);
                        }

                        j++;
                    }


                    ////DEBUG
                    //printLine("localBeforeLimit: " + localBeforeLimitC);

                    //fill Up localBeforeLimit

                    int numberOfCharsBefore = 0;

                    j = stlList1.FindIndex(x => x == stlList2[0]) - 1;

                    while (j >= 0)
                    {
                        ////    DEBUG
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
                            if (LOCAL_DEBUG)
                                printLine("added before : " + stlList1[j].Position);
                        }

                        j--;
                    }

                    stlList2.Sort();

                    // Set StartTime and EndTime

                    int startTime = 0;
                    int endTime = 0;
                    int distanceToPrevious = 0;
                    int distanceToNext = 0;

                    var firstStl = stlList2[0];
                    var lastStl = stlList2[stlList2.Count() - 1];

                    var relevantStls = SubtitleLines.Where(x => x.Movie == firstStl.Movie);

                    // Set StartTime and EndTime

                    if (firstStl.Position > relevantStls.OrderBy(c => c.Position).First().Position)
                    {
                        var peviousStl = relevantStls.Where(x => x.Position == firstStl.Position - 1).FirstOrDefault();
                        distanceToPrevious = firstStl.starttime - peviousStl.endtime - Properties.Settings.Default.ExtraPaddingCut;
                    }
                    else
                    {
                        distanceToPrevious = firstStl.starttime;
                    }

                    startTime = firstStl.starttime - Math.Min(PaddingBefore, distanceToPrevious);

                    // Set EndTime and EndTime

                    if (lastStl.Position < relevantStls.OrderByDescending(c => c.Position).First().Position)
                    {
                        var nextStl = relevantStls.Where(x => x.Position == lastStl.Position + 1).FirstOrDefault();
                        distanceToNext = nextStl.starttime - lastStl.endtime - Properties.Settings.Default.ExtraPaddingCut;
                    }
                    else
                    {
                        // TO-DO: consider Movie-Length
                        distanceToPrevious = 0;
                    }

                    endTime = lastStl.endtime + Math.Min(PaddingAfter, distanceToNext);

                    if (LOCAL_DEBUG)
                    {
                        printLine("startTime : " + startTime);
                        printLine("endTime : " + endTime);
                    }



                    // Create SubtitleLinePack

                    SubtitleLinePack stlp2 = new SubtitleLinePack { SubtitleLines = stlList2, StartTime = startTime, EndTime = endTime };
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

                var allStlsInMovie = movie.SubtitleLines.ToList();
                allStlsInMovie.Sort();

                foreach (SubtitleLine stl in allStlsInMovie)
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

                stlList.Sort();
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
