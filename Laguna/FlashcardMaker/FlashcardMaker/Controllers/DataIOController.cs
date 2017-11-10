using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using FlashcardMaker.Models;
using System.IO;
using System.Collections;
using System.Data.Entity.Migrations;
using System.Data.OleDb;
using System.Data;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using MySql.Data.MySqlClient;
using FlashcardMaker.Views;
using System.Globalization;
using FlashcardMaker.Helpers;
using System.Net;
using System.Web;

namespace FlashcardMaker.Controllers
{
    public class DataIOController : IController
    {
        private ISessionView view;
        private ProgramController programController;

        public DataIOController(ISessionView mainForm, ProgramController programController)
        {
            view = mainForm;
            this.programController = programController;
        }

        internal void ReadInFromSubtitleFileToDb(string[] fullFileNames)
        {

            printLine("Adding Subtitles Process begins ");
            AddMoviesToDb(fullFileNames);
            AddSubtitles();
            UpdateSubtitleLines();

        }

        private void AddSubtitles()
        {
            MyDbContext db = new MyDbContext();

            printLine("Adding Subtitles ");

            var moviesToAdd = from b in db.Movies
                              where b.added.Equals(false)
                              select b;



            foreach (Movie movie in moviesToAdd.ToList())
            {
                String fullFileName = movie.fullFileName;

                printLine("fullFileName: " + fullFileName +
                            "\nfileName: " + movie.fileName +
                            "\nfileExtention: " + movie.fileExtention +
                            "\nadded: " + movie.added +
                            "\n isSRT: ");

                printLine("\n\n");

                int totalCount = 0;
                int counter = 0;
                string line;

                StreamReader file = new StreamReader(fullFileName);
                SubtitleLine subtitleLine = new SubtitleLine { };

                while ((line = file.ReadLine()) != null && totalCount < ProgramController.MAX_SUBTITLES_TO_LOAD)
                {
                    if (counter % 4 == 0)
                    {
                        subtitleLine = new SubtitleLine { MovieFileName = movie.fileName, Position = Int32.Parse(line), CanRead = 0 };
                    }
                    else if (counter % 4 == 1)
                    {
                        subtitleLine.TimeFrameString = line;
                        SaveStartAndEndTime(line, subtitleLine);
                    }
                    else if (counter % 4 == 2)
                    {
                        subtitleLine.Chinese = line;
                        movie.SubtitleLines.Add(subtitleLine);
                        db.SubtitleLines.AddOrUpdate(p => new { p.MovieFileName, p.Position }, subtitleLine);
                        db.SaveChanges();
                        view.printStatusLabel(totalCount + " subtitles added");
                        totalCount++;
                    }

                    counter++;
                }

                movie.added = true;

                file.Close();
            }

            db.SaveChanges();

            view.refresh();

            printLine("Done Adding Subtitles ");
        }

        private void SaveStartAndEndTime(string line, SubtitleLine stl)
        {
            string[] startAndEnd = line.Split(new string[] { " --> " }, StringSplitOptions.None);

            stl.starttime = ConvertTimeStringToLong(startAndEnd[0]);
            stl.endtime = ConvertTimeStringToLong(startAndEnd[1]);
        }

        private static int ConvertTimeStringToLong(string startString)
        {
            DateTime startDateTime = DateTime.ParseExact(startString, "HH:mm:ss,fff",
                                                    CultureInfo.InvariantCulture);
            int start = startDateTime.Hour * 3600000 + startDateTime.Minute * 60000 + startDateTime.Second * 1000 + startDateTime.Millisecond;
            return start;
        }

        private void AddMoviesToDb(string[] fullFileNames)
        {
            MyDbContext db = new MyDbContext();

            printLine("Adding Movies to Movies Table: ");

            foreach (String fullFileName in fullFileNames)
            {
                string fileExtention = Path.GetExtension(fullFileName);
                string fileName = Path.GetFileName(fullFileName);
                bool added = false;

                var result = db.Movies.SingleOrDefault(b => b.fileName == fileName);
                if (result != null)
                {
                    added = result.added;
                }

                var movie = db.Movies.Create();
                movie.added = added;
                movie.fullFileName = fullFileName;
                movie.fileExtention = fileExtention;
                movie.fileName = fileName;
                movie.SubtitleLines = new List<SubtitleLine>();
                db.Movies.AddOrUpdate(p => p.fileName, movie);
                db.SaveChanges();

                printLine("Added: " + movie.fileName);
            }

            printLine("Done Adding Movies to Movies Table: ");
        }

        internal void Test()
        {

            //VideoEditor sv = new VideoEditor(view);
            //sv.test(this);
            using (OurWebClient client = new OurWebClient())
            using (MyDbContext db = new MyDbContext())
            {
                string result = "";

                string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", "你放屁了吗", "de#zh-CN");
                WebClient webClient = new WebClient();
                webClient.Encoding = System.Text.Encoding.UTF8;
                string answer = webClient.DownloadString(url);

                HttpDownloader hd = new HttpDownloader(url, "", "");
                answer = hd.GetPage();


                printLine(answer);
                result = answer.Substring(answer.IndexOf("<span title=\"") + "<span title=\"".Length);
                result = result.Substring(result.IndexOf(">") + 1);
                result = result.Substring(0, result.IndexOf("</span>"));





                printLine(result.Trim());

                result = answer.Substring(answer.IndexOf("src-translit") + "< div id=src-translit".Length);
                result = result.Substring(result.IndexOf(">") + 1);
                result = result.Substring(0, result.IndexOf("</div>"));

                printLine(HttpUtility.HtmlDecode(result.Trim()));

                Flashcard fc = Factory.InsertFlashcard(db, view, HttpUtility.HtmlDecode(result.Trim()), 354, true);

            }




            //printLine("Finished");
        }

        internal void AddExcelDataToDatabase<fieldClass>(string fileName)
        {
            ReadInFromExcelToDb<fieldClass>(fileName);
            CheckForDuplicates<fieldClass>();
        }

        private void ReadInFromExcelToDb<fieldClass>(string fileName)
        {
            MyDbContext db = new MyDbContext();

            printLine("Reading in from " + fileName + "\n");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Application xlApp = new Application();
            Workbook xlWorkbook = xlApp.Workbooks.Open(fileName);
            Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;

            int totalCount = 0;

            for (int i = 1; i <= rowCount; i++)
            {
                view.printStatusLabel("\nReading from Excel. Row: " + i.ToString() + "\nTime elapsed: " + stopWatch.Elapsed);

                if (typeof(fieldClass) == typeof(ChineseCharacter))
                {
                    if (ProgramController.DEBUGGING == true && ++totalCount > ProgramController.MAX_CHINESE_CHARACTERS_TO_LOAD) break;

                    var chineseCharacter = new ChineseCharacter { };

                    chineseCharacter.Rank = Int32.Parse(xlRange.Cells[i, 1].Value2.ToString());
                    chineseCharacter.Chinese = xlRange.Cells[i, 2].Value2.ToString();

                    if (xlRange.Cells[i, 5].Value2 != null)
                    {
                        chineseCharacter.PinYin = xlRange.Cells[i, 5].Value2.ToString();
                    }

                    if (xlRange.Cells[i, 6].Value2 != null)
                    {
                        chineseCharacter.English = xlRange.Cells[i, 6].Value2.ToString();
                    }

                    db.ChineseCharacters.AddOrUpdate(p => p.Chinese, chineseCharacter);
                }
                else if (typeof(fieldClass) == typeof(ChineseWord))
                {
                    if (ProgramController.DEBUGGING == true && ++totalCount > ProgramController.MAX_CHINESE_WORDS_TO_LOAD) break;

                    var cw = new ChineseWord { };

                    cw.position = Int32.Parse(xlRange.Cells[i, 1].Value2.ToString());
                    cw.Chinese = xlRange.Cells[i, 2].Value2.ToString();
                    cw.knowLevel = Int32.Parse(xlRange.Cells[i, 3].Value2.ToString());
                    cw.PinYin = xlRange.Cells[i, 4].Value2.ToString();
                    cw.English = xlRange.Cells[i, 5].Value2.ToString();

                    db.ChineseWords.AddOrUpdate(p => p.position, cw);
                }
            }

            xlWorkbook.Close();

            db.SaveChanges();

            stopWatch.Stop();
            //view.printInStatusLabel("Done" + "\nTime elapsed: " + stopWatch.Elapsed);

            printLine(totalCount - 1 + " items Added\n");
        }

        internal void DeleteAllMediaFiles()
        {
            using (MyDbContext db = new MyDbContext())
            {
                foreach (SubtitleLinePack stlp in db.SubtitleLinePacks)
                {
                    stlp.MediaFileSegments_remote_id = 0;
                }
                db.SaveChanges();

                db.Database.ExecuteSqlCommand("DELETE FROM MediaFileSegments");

                printLine("MediaFiles deleted");
                Updater.updateDbMediaFiles(db, view);
            }
        }

        internal void DeleteAllFlashcards()
        {
            printLine("Deleting all flashcards");

            using (MyDbContext db = new MyDbContext())
            {
                db.Flashcards.RemoveRange(db.Flashcards);
                db.SaveChanges();
            }

            printLine("All flashcards deleted");
        }

        public void UpdateSubtitleLines()
        {
            //UpdateToLearnWordsAndCharNumber();
            UpdateTranslationAndTranslit();
        }

        private void UpdateTranslationAndTranslit()
        {
            using (OurWebClient webClient = new OurWebClient())
            using (MyDbContext db = new MyDbContext())
            {
                int counter = 0;

                foreach (SubtitleLine stl in db.SubtitleLines.Where(p=>p.English == null && p.Translit == null).ToList())
                {
                    printStatusLabel("Updating: " + ++counter);
                    //if (counter++ > 20)
                    //    break;

                    String English = "";
                    String Translit = "";

                    string result = "";

                    string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", stl.Chinese, "zh-CN/en");
                    webClient.Encoding = System.Text.Encoding.UTF8;
                    string answer = webClient.DownloadString(url);

                    HttpDownloader hd = new HttpDownloader(url, "", "");
                    answer = hd.GetPage();


                    //printLine(answer);
                    result = answer.Substring(answer.IndexOf("<span title=\"") + "<span title=\"".Length);
                    result = result.Substring(result.IndexOf(">") + 1);
                    result = result.Substring(0, result.IndexOf("</span>"));

                    English = HttpUtility.HtmlDecode(result.Trim());

                    result = answer.Substring(answer.IndexOf("src-translit") + "< div id=src-translit".Length);
                    result = result.Substring(result.IndexOf(">") + 1);
                    result = result.Substring(0, result.IndexOf("</div>"));

                    Translit = HttpUtility.HtmlDecode(result.Trim());

                    stl.Translit = Translit;
                    stl.English = English;

                    printLine(Translit);
                    printLine(English);

                    db.SaveChanges();
                }
            }
        }

        private void UpdateToLearnWordsAndCharNumber()
        {
            using (MyDbContext db = new MyDbContext())
            {
                printLine("Updating canRead in SubtitleLines : " + db.SubtitleLines.Count());

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                int totalCount = 0;

                var wordsToLearn = from t in db.ChineseWords
                                   where t.knowLevel == 0
                                   select t;

                int i = 0;
                foreach (SubtitleLine stl in db.SubtitleLines.ToList())
                {

                    if (ProgramController.DEBUGGING_SUBTITLELINES && i++ > ProgramController.MAX_SUBTITLELINES_TO_PROCESS)
                        break;

                    stl.CanRead = 0;
                    stl.ToLearnWords.Clear();
                    stl.NumberOfToLearnWords = 0;
                    stl.NumberOfCharacters = 0;

                    string subtitleLineChineseCopy = String.Copy(stl.Chinese);

                    foreach (ChineseWord cw in wordsToLearn.ToList())
                    {
                        if (subtitleLineChineseCopy.Contains(cw.Chinese))
                        {
                            stl.ToLearnWords.Add(cw);
                        }

                        subtitleLineChineseCopy = subtitleLineChineseCopy.Replace(cw.Chinese, "");
                    }

                    foreach (NoneChineseCharacter ncc in db.NoneChineseCharacters.ToList())
                    {
                        subtitleLineChineseCopy = subtitleLineChineseCopy.Replace(ncc.Character, "");
                    }

                    stl.NumberOfCharacters = subtitleLineChineseCopy.Length;
                    stl.NumberOfToLearnWords = stl.ToLearnWords.Count();



                    int iDontKnowCCNumber = 0;

                    foreach (char c in subtitleLineChineseCopy)
                    {
                        string cString = c.ToString();

                        var ccInDb = db.ChineseCharacters.SingleOrDefault(d => d.Chinese == cString);
                        var cInNoneChinese = db.NoneChineseCharacters.SingleOrDefault(d => d.Character == cString);

                        if (ccInDb != null)
                        {
                            if (ccInDb.KnowLevel == 0)
                            {
                                iDontKnowCCNumber++;
                            }
                        }
                        else if (cInNoneChinese == null)
                        {
                            string answer = programController.askIfChineseCharacter(cString);

                            if (answer == "AddToChineseCharacters")
                            {
                                db.ChineseCharacters.AddOrUpdate(p => p.Chinese, new ChineseCharacter { Chinese = cString, KnowLevel = 0, Rank = db.ChineseCharacters.Max(p => p.Rank) + 1 });
                                db.SaveChanges();
                                printLine("Added to Chinese Characters: \"" + cString + "\"");
                            }
                            else if (answer == "AddToNoneChineseCharacters")
                            {
                                db.NoneChineseCharacters.AddOrUpdate(p => p.Character, new NoneChineseCharacter { Character = cString });
                                db.SaveChanges();
                                printLine("Will add do none-Chinese Characters: " + cString);

                            }
                            else if (answer == "Cancel")
                            {
                                printLine("Canceld by user");
                                return;
                            }

                        }
                    }

                    totalCount++;

                    if (iDontKnowCCNumber == 0)
                    {
                        stl.CanRead = 1;
                        db.SaveChanges();
                    }
                    //else
                    //{
                    //    printLine("cant Read : " + stl.Chinese + " iDontKnowCCNumber: " + iDontKnowCCNumber);
                    //}                    

                    if (totalCount % 10 == 0)
                        view.printStatusLabel("Updating canRead in SubtitleLines: " + totalCount + "\nTime elapsed: " + stopWatch.Elapsed);

                }

                db.SaveChanges();

                stopWatch.Stop();

                printLine("Done updating canRead in SubtitleLines" + "\nTime elapsed: " + stopWatch.Elapsed);
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

        internal void ClearAll()
        {
            using (MyDbContext db = new MyDbContext())
            {
                printLine("\n");

                ClearSubtitles();

                ClearCharacters();

                ClearWords();

                db.SaveChanges();

                printLine("\nDone!\n");
            }


        }

        internal void ClearWords()
        {
            using (MyDbContext db = new MyDbContext())
            {
                printLine("Clearing Table ChineseWords");
                db.Database.ExecuteSqlCommand("DELETE FROM ChineseWords");
                printLine("Finished clearing Table ChineseWords");
            }
        }

        internal void ClearCharacters()
        {
            using (MyDbContext db = new MyDbContext())
            {
                printLine("Clearing Table ChineseCharacters");
                db.Database.ExecuteSqlCommand("DELETE FROM ChineseCharacters");
                printLine("Finished clearing Table ChineseCharacters");
            }

        }

        internal void ClearSubtitles()
        {
            using (MyDbContext db = new MyDbContext())
            {
                foreach (Movie movie in db.Movies.ToList())
                {
                    movie.SubtitleLines.Clear();
                    movie.SubtitleLinePacks.Clear();
                    db.SaveChanges();
                }
            }
            using (MyDbContext db = new MyDbContext())
            {
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("DELETE FROM Movies");
                printLine("Cleared Table Movies");

                db.Database.ExecuteSqlCommand("DELETE FROM SubtitleLines");
                printLine("Cleared Table SubtitleLines");

                db.SaveChanges();
                view.refresh();
            }
        }

        internal void Refresh()
        {
            RefreshChineseCharactersToChineseWordsReferences();
            RefreshKnowLevels();
        }

        private void RefreshKnowLevels()
        {
            MyDbContext db = new MyDbContext();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            int totalCount = 0;

            printLine("Refreshing knowLevels\n");

            foreach (ChineseCharacter cw in db.ChineseCharacters)
            {
                cw.KnowLevel = 0;
            }

            foreach (ChineseWord cw in db.ChineseWords.ToList())
            {
                view.printStatusLabel("\nUpdating ChineseWords: " + totalCount++.ToString() + "\nTime elapsed: " + stopWatch.Elapsed);

                foreach (ChineseCharacter cc in cw.ChineseCharacters.ToList())
                {
                    cc.KnowLevel++;
                }
            }

            db.SaveChanges();

            printLine("\nDone refreshing knowLevels!\n");

            stopWatch.Stop();
            view.printStatusLabel("Done refreshing knowLevels" + "\nTime elapsed: " + stopWatch.Elapsed);
        }

        private void RefreshChineseCharactersToChineseWordsReferences()
        {
            MyDbContext db = new MyDbContext();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            int totalCount = 0;

            printLine("Referrencing ChineseCharacters to ChineseWords\n");

            foreach (ChineseWord cw in db.ChineseWords.ToList())
            {

                view.printStatusLabel("\nUpdating ChineseWords: " + totalCount++.ToString() + "\nTime elapsed: " + stopWatch.Elapsed);

                cw.ChineseCharacters = new List<ChineseCharacter>();

                foreach (ChineseCharacter cc in db.ChineseCharacters.ToList())
                {
                    if (cw.Chinese.Contains(cc.Chinese))
                    {
                        cw.ChineseCharacters.Add(cc);
                    }
                }
            }

            db.SaveChanges();

            printLine("\nDone Referrencing ChineseCharacters to ChineseWords!\n");

            stopWatch.Stop();
            view.printStatusLabel("Done" + "\nTime elapsed: " + stopWatch.Elapsed);
        }

        private void CheckForDuplicates<fieldClass>()
        {
            MyDbContext db = new MyDbContext();

            StringBuilder stringBuilder = new StringBuilder();
            printLine("\nChecing for duplicates:\n");

            IQueryable<string> duplicates = null;


            if (typeof(fieldClass) == typeof(ChineseCharacter))
            {
                duplicates = db.ChineseCharacters.GroupBy(i => i.Chinese)
                                                 .Where(x => x.Count() > 1)
                                                 .Select(val => val.Key);
            }
            else if (typeof(fieldClass) == typeof(ChineseWord))
            {
                duplicates = db.ChineseWords.GroupBy(i => i.Chinese)
                                                 .Where(x => x.Count() > 1)
                                                 .Select(val => val.Key);
            }

            if (duplicates == null)
            {
                printLine("\n variable duplicates is null for some reason");
            }
            else if (duplicates.Count() == 0)
            {
                printLine("\nno duplicates found");
            }
            else
            {
                foreach (var v in duplicates)
                {
                    printLine("\n" + v.ToString());
                }
            }
        }
    }
}


