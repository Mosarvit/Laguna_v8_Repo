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

namespace FlashcardMaker.Controllers
{
    public class DataIOController
    {
        MainView view;
         
        public static bool DEBUGGING = true;

        public static int MAX_CHINESE_CHARACTERS_TO_LOAD = 15000;
        public static int MAX_CHINESE_WORDS_TO_LOAD = 15000;
        public static int MAX_SUBTITLES_TO_UPDATE = 100;
        public static int MAX_SUBTITLES_TO_LOAD = 100;

        public DataIOController(MainView mainForm)
        {
            this.view = mainForm;
        }

        internal void ReadInFromSubtitleFileToDb(string[] fullFileNames)
        {
            view.printInMainTextLabel("Adding Subtitles Process begins ");
            AddMoviesToDb(fullFileNames);
            AddSubtitles();
        }

        private void AddSubtitles()
        {
            MyDbContext db = new MyDbContext();

            view.printInMainTextLabel("Adding Subtitles ");

            var moviesToAdd = from b in db.Movies
                              where b.added.Equals(false)
                              select b;

            int totalCount = 0;

            foreach (Movie movie in moviesToAdd.ToList())
            {
                String fullFileName = movie.fullFileName;

                view.printInMainTextLabel("fullFileName: " + fullFileName +
                                                            "\nfileName: " + movie.fileName +
                                                            "\nfileExtention: " + movie.fileExtention +
                                                            "\nadded: " + movie.added +
                                                            "\n isSRT: ");

                if (movie.fileExtention == ".srt")
                {
                    view.printInLineInMainTextLabel("yes ");
                }
                else
                {
                    view.printInLineInMainTextLabel("no ");
                }

                view.printInMainTextLabel("\n\n");

                int counter = 0;
                string line;

                System.IO.StreamReader file = new System.IO.StreamReader(fullFileName);
                SubtitleLine subtitleLine = new SubtitleLine { };

                while ((line = file.ReadLine()) != null)
                {
                    if (counter % 4 == 0)
                    {
                        subtitleLine = new SubtitleLine { MovieFileName = movie.fileName, Position = Int32.Parse(line), CanRead = 0 };
                    }
                    else if (counter % 4 == 1)
                    {
                        subtitleLine.TimeFrameString = line;
                    }
                    else if (counter % 4 == 2)
                    {
                        subtitleLine.Chinese = line;
                        movie.SubtitleLines.Add(subtitleLine);
                        db.SubtitleLines.AddOrUpdate(p => new { p.MovieFileName, p.Position }, subtitleLine);
                        db.SaveChanges();
                        view.printInStatusLabel(totalCount++ + " subtitles added");
                    }

                    counter++;

                    if (DEBUGGING == true && totalCount > MAX_SUBTITLES_TO_LOAD)
                        break;
                }

                movie.added = true;

                file.Close();
            }

            db.SaveChanges();

            view.refresh();

            view.printInMainTextLabel("Done Adding Subtitles ");
        }

        private void AddMoviesToDb(string[] fullFileNames)
        {
            MyDbContext db = new MyDbContext();

            view.printInMainTextLabel("Adding Movies to Movies Table: ");

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

                var movie = new Movie { added = added, fullFileName = fullFileName, fileExtention = fileExtention, fileName = fileName, SubtitleLines = new List<SubtitleLine>() };
                db.Movies.AddOrUpdate(p => p.fileName, movie);
                db.SaveChanges();

                view.printInMainTextLabel("Added: " + movie.fileName);
            }

            view.printInMainTextLabel("Done Adding Movies to Movies Table: ");
        }

        internal void test()
        {
            using (MyDbContext db = new MyDbContext())
            {

                int remote_id;

                if (db.Flashcards.Count() == 0)
                {
                    remote_id = 0;
                }
                else
                {
                    remote_id = db.Flashcards.Max(u => u.remote_id) + 1;
                }

                var fc = new Flashcard { question = "好", remote_id = remote_id, newFc = true };
                fc.question = "好";
                fc.newFc = true;
                fc.utlocal = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                fc.utserverwhenloaded = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                fc.setToDelete(false);

                db.Flashcards.Add(fc);
                

                try
                {
                    int result = db.SaveChanges();
                    view.printInMainTextLabel("Number of changes executed: " + result);
                }
                catch (Exception)
                {
                    throw;
                }


            }

            //view.printInMainTextLabel("Trying to connect to db");

            //MySqlConnection conn = new MySqlConnection();
            //string connString;

            //connString = "server=mosarvit.heliohost.org;DATABASE=mosarvit_flashcards_db;UID=mosarvit_1;PASSWORD=Fahrenheit;";

            //try
            //{
            //    conn = new MySqlConnection();
            //    conn.ConnectionString = connString;
            //    conn.Open();
            //    view.printInMainTextLabel("Connection opened");



            //    conn.Close();
            //    view.printInMainTextLabel("Connection closed");
            //}
            //catch (MySql.Data.MySqlClient.MySqlException ex)
            //{
            //    view.printInMainTextLabel(ex.Message);
            //}
            //finally
            //{
            //    if (conn != null)
            //    {
            //        conn.Dispose();
            //        view.printInMainTextLabel("Disposed connection");
            //    }
            //}
            
            

            //view.printInMainTextLabel("Finished");
        }

        internal void AddExcelDataToDatabase<fieldClass>(string fileName)
        {
            ReadInFromExcelToDb<fieldClass>(fileName);
            checkForDuplicates<fieldClass>();
        }

        private void ReadInFromExcelToDb<fieldClass>(string fileName)
        {
            MyDbContext db = new MyDbContext();

            view.printInMainTextLabel("Reading in from " + fileName + "\n");

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
                view.printInStatusLabel("\nReading from Excel. Row: " + i.ToString() + "\nTime elapsed: " + stopWatch.Elapsed);

                if (typeof(fieldClass) == typeof(ChineseCharacter))
                {
                    if (DEBUGGING == true && ++totalCount > MAX_CHINESE_CHARACTERS_TO_LOAD) break;

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
                    if (DEBUGGING == true && ++totalCount > MAX_CHINESE_WORDS_TO_LOAD) break;

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

            view.printInMainTextLabel(totalCount - 1 + " items Added\n");
        }

        internal void DeleteAllFlashcards()
        {
            view.printInMainTextLabel("Deleting all flashcards");

            using (MyDbContext db = new MyDbContext())
            {
                db.Flashcards.RemoveRange(db.Flashcards);
                db.SaveChanges();
            }

            view.printInMainTextLabel("All flashcards deleted");
        }

        internal void CreateFlashcards()
        {
            view.printInMainTextLabel("Creating flashCards process begins");

            var db = new MyDbContext();

            UpdateAll(db);
            CreateSubtitlePacks();
            List<SubtitleLinePack> tempSortSubtitleLinePackList = SortedSubtitleLinePackList(db);

            view.printInMainTextLabel("Number of stlps: " + tempSortSubtitleLinePackList.Count());

            CreateFlashcards(tempSortSubtitleLinePackList, db);

            view.printInMainTextLabel("Done with creating Flashcards process!\n");
        }

        private void CreateFlashcards(List<SubtitleLinePack> tempSortSubtitleLinePackList, MyDbContext db)
        {
            view.printInMainTextLabel("Creating Flashcards");

            foreach (var stlp in tempSortSubtitleLinePackList)
            {
                Flashcard fc = new Flashcard { };
                //fc.SubtitleLinePack = stlp;
                db.Flashcards.Add(fc);
            }

            db.SaveChanges();

            view.printInMainTextLabel("Done creating Flashcards");
        }

        private List<SubtitleLinePack> SortedSubtitleLinePackList(MyDbContext db)
        {
            view.printInMainTextLabel("Sorting SubtitlelinePacks");

            var tempStlpList = new List<SubtitleLinePack>();

            UpdateStlps(db);

            SubtitleLinePack mostDenseStlp = db.SubtitleLinePacks.OrderByDescending(c => c.DensityOfToLearnWords).ToList()[0];
            
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

            view.printInMainTextLabel("Finished sorting SubtitlelinePacks");

            return tempStlpList;
        }

        private void CreateSubtitlePacks()
        {
            MyDbContext db = new MyDbContext();

            view.printInMainTextLabel("Creating SubtitlelinePacks");
            view.printInMainTextLabel("Number of SubtitlelinePacks: " + db.SubtitleLinePacks.Count());

            foreach (SubtitleLinePack stlp in db.SubtitleLinePacks.ToList())
            {
                //view.printInMainTextLabel("Clearing, Id: " + stlp.Id + " " + stlp.NumberOfCharacters + " " + stlp.NumberOfToLearnWords);
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

            int totalCount = 0;

            IList<Movie> Movies = db.Movies.ToList();

            foreach (Movie movie in Movies)
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

                    view.printInStatusLabel("SubtitleLines analyzed: " + totalCount++);

                    if (DEBUGGING == true && totalCount > MAX_SUBTITLES_TO_UPDATE)
                    {
                        db.SaveChanges();
                        return;
                    }
                }
            }

            db.SaveChanges();

            view.printInMainTextLabel("Done Creating SubtitlelinePacks");
        }

        private void UpdateAll(MyDbContext db)
        {
            UpdateCanReadInSubtitleLines(db);
            UpdateStlps(db);
        }

        private void UpdateCanReadInSubtitleLines(MyDbContext db)
        {
            view.printInMainTextLabel("Updating canRead in SubtitleLines");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            int totalCount = 0;
            int findingsCount = 0;

            var wordsToLearn = from t in db.ChineseWords
                               where t.knowLevel == 0
                               select t;

            IList<SubtitleLine> allSubtitleLineList = db.SubtitleLines.ToList();

            foreach (SubtitleLine stl in allSubtitleLineList)
            {
                string subtitleLineChineseCopy = String.Copy(stl.Chinese);

                foreach (NoneChineseCharacter ncc in db.NoneChineseCharacters)
                {
                    subtitleLineChineseCopy = subtitleLineChineseCopy.Replace(ncc.Character, "");
                }

                stl.NumberOfCharacters = subtitleLineChineseCopy.Length;
                stl.NumberOfToLearnWords = 0;

                foreach (ChineseWord cw in wordsToLearn.ToList())
                {
                    if (subtitleLineChineseCopy.Contains(cw.Chinese))
                    {
                        stl.ToLearnWords.Add(cw);
                        stl.NumberOfToLearnWords++;
                    }

                    subtitleLineChineseCopy = subtitleLineChineseCopy.Replace(cw.Chinese, "");
                }

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
                        string answer = view.askIfChineseCharacter(cString);

                        if (answer == "AddToChineseCharacters")
                        {
                            db.ChineseCharacters.AddOrUpdate(p => p.Chinese, new ChineseCharacter { Chinese = cString, KnowLevel = 0, Rank = db.ChineseCharacters.Max(p => p.Rank) + 1 });
                            db.SaveChanges();
                            view.printInMainTextLabel("Added to Chinese Characters: \"" + cString + "\"");
                        }
                        else if (answer == "AddToNoneChineseCharacters")
                        {
                            db.NoneChineseCharacters.AddOrUpdate(p => p.Character, new NoneChineseCharacter { Character = cString });
                            db.SaveChanges();
                            view.printInMainTextLabel("Will add do none-Chinese Characters: " + cString);

                        }
                        else if (answer == "Cancel")
                        {
                            view.printInMainTextLabel("Canceld by user");
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
                //    view.printInMainTextLabel("cant Read : " + stl.Chinese + " iDontKnowCCNumber: " + iDontKnowCCNumber);
                //}                    

                if (totalCount % 10 == 0)
                    view.printInStatusLabel("Updating canRead in SubtitleLines: " + totalCount + "\nTime elapsed: " + stopWatch.Elapsed);

                if (totalCount > MAX_SUBTITLES_TO_UPDATE || findingsCount > 10)
                    break;
            }

            db.SaveChanges();

            stopWatch.Stop();

            view.printInMainTextLabel("Done updating canRead in SubtitleLines" + "\nTime elapsed: " + stopWatch.Elapsed);
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

        //private void printLineInMainView(StringBuilder stringBuilder, string sdf)
        //{
        //    stringBuilder.Append("\n" + sdf);
        //    view.printInMainTextLabel(stringBuilder.ToString());
        //}

        //private static void printInLineInMainView(StringBuilder stringBuilder, string sdf)
        //{
        //    stringBuilder.Append(sdf);
        //    view.printInMainTextLabel(stringBuilder.ToString());
        //}

        internal void clearAll()
        {
            MyDbContext db = new MyDbContext();

            view.printInMainTextLabel("\n");

            clearSubtitles();

            view.printInMainTextLabel("Clearing Table ChineseCharacters\n");
            db.Database.ExecuteSqlCommand("DELETE FROM [ChineseCharacters]");

            view.printInMainTextLabel("Clearing Table ChineseWords\n");
            db.Database.ExecuteSqlCommand("DELETE FROM [ChineseWords]");

            db.SaveChanges();

            view.printInMainTextLabel("\nDone!\n");
        }

        internal void clearSubtitles()
        {
            MyDbContext db = new MyDbContext();

            foreach (Movie movie in db.Movies)
            {
                movie.SubtitleLines.Clear();
            }
            db.SaveChanges();
            db.Database.ExecuteSqlCommand("DELETE FROM [Movies]");
            view.printInMainTextLabel("Cleared Table Movies");

            db.Database.ExecuteSqlCommand("DELETE FROM [SubtitleLines]");
            view.printInMainTextLabel("Cleared Table SubtitleLines");

            db.SaveChanges();
            view.refresh();
        }

        internal void refresh()
        {
            refreshChineseCharactersToChineseWordsReferences();
            refreshKnowLevels();
        }

        private void refreshKnowLevels()
        {
            MyDbContext db = new MyDbContext();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            int totalCount = 0;

            view.printInMainTextLabel("Refreshing knowLevels\n");

            foreach (ChineseCharacter cw in db.ChineseCharacters)
            {
                cw.KnowLevel = 0;
            }

            foreach (ChineseWord cw in db.ChineseWords.ToList())
            {
                view.printInStatusLabel("\nUpdating ChineseWords: " + totalCount++.ToString() + "\nTime elapsed: " + stopWatch.Elapsed);

                foreach (ChineseCharacter cc in cw.ChineseCharacters.ToList())
                {
                    cc.KnowLevel++;
                }
            }

            db.SaveChanges();

            view.printInMainTextLabel("\nDone refreshing knowLevels!\n");

            stopWatch.Stop();
            view.printInStatusLabel("Done refreshing knowLevels" + "\nTime elapsed: " + stopWatch.Elapsed);
        }

        private void refreshChineseCharactersToChineseWordsReferences()
        {
            MyDbContext db = new MyDbContext();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            int totalCount = 0;

            view.printInMainTextLabel("Referrencing ChineseCharacters to ChineseWords\n");

            foreach (ChineseWord cw in db.ChineseWords.ToList())
            {

                view.printInStatusLabel("\nUpdating ChineseWords: " + totalCount++.ToString() + "\nTime elapsed: " + stopWatch.Elapsed);

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

            view.printInMainTextLabel("\nDone Referrencing ChineseCharacters to ChineseWords!\n");

            stopWatch.Stop();
            view.printInStatusLabel("Done" + "\nTime elapsed: " + stopWatch.Elapsed);
        }

        private void checkForDuplicates<fieldClass>()
        {
            MyDbContext db = new MyDbContext();

            StringBuilder stringBuilder = new StringBuilder();
            view.printInMainTextLabel("\nChecing for duplicates:\n");

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
                view.printInMainTextLabel("\n variable duplicates is null for some reason");
            }
            else if (duplicates.Count() == 0)
            {
                view.printInMainTextLabel("\nno duplicates found");
            }
            else
            {
                foreach (var v in duplicates)
                {
                    view.printInMainTextLabel("\n" + v.ToString());
                }
            }
        }
    }
}


