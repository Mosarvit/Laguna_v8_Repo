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
using System.Text.RegularExpressions;

namespace FlashcardMaker.Controllers
{
    public class DataIOController : IController
    {
        private ISessionView view;
        private ProgramController programController;

        private string[] tableNamesToBackUp = {

            "chinesecharacters",
            "pinyins",
            "pinyinchinesecharacters",
            "chinesewords",
            "chinesewordchinesecharacters",
            "pinyinwords",
            "pinyinwordchinesewords",
            "subtitlelines",
            "subtitlelinechinesewords",
            "movies"
        };

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
            printLine("Adding Subtitles ");

            while (true)
            {
                using (MyDbContext db = new MyDbContext())
                {
                    var movies = db.Movies.Where(b => !b.added).ToList();

                    if (movies.Count() == 0)
                    {
                        break;
                    }

                    var movie = movies[0];

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

                    db.SaveChanges();
                }
            }

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
                //string result = "";

                //string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", "你放屁了吗", "de#zh-CN");
                //WebClient webClient = new WebClient();
                //webClient.Encoding = System.Text.Encoding.UTF8;
                //string answer = webClient.DownloadString(url);

                //HttpDownloader hd = new HttpDownloader(url, "", "");
                //answer = hd.GetPage();


                //printLine(answer);
                //result = answer.Substring(answer.IndexOf("<span title=\"") + "<span title=\"".Length);
                //result = result.Substring(result.IndexOf(">") + 1);
                //result = result.Substring(0, result.IndexOf("</span>"));





                //printLine(result.Trim());

                //result = answer.Substring(answer.IndexOf("src-translit") + "< div id=src-translit".Length);
                //result = result.Substring(result.IndexOf(">") + 1);
                //result = result.Substring(0, result.IndexOf("</div>"));

                //printLine(HttpUtility.HtmlDecode(result.Trim()));

                //Flashcard fc = Factory.InsertFlashcard(db, view, HttpUtility.HtmlDecode(result.Trim()), 354, true);
                //int tone;

                //string toneString = Regex.Match("zhao4", @"\d+").Value;
                //printLine(toneString);

                //if (!Int32.TryParse(toneString, out tone))
                //{
                //    tone = 5;
                //}

                //printLine(""+tone);

                string query = "load data local infile 'C:/Users/Mosarvit/FilesToWorkWith/SharedFolder/dbBACKUPS/cc.csv' into table chinesecharacters fields terminated by ';' enclosed by '\"' lines terminated by '\\n' IGNORE 1 LINES";

                printLine(query);

                db.Database.ExecuteSqlCommand(query);

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

                if (Properties.Settings.Default.DEBUG_MAX_CCS_TO_LOAD > 0 && totalCount > Properties.Settings.Default.DEBUG_MAX_CCS_TO_LOAD)
                {
                    printLine("Achieved MAX_CCS_TO_LOAD");
                    break;
                }

                if (typeof(fieldClass) == typeof(ChineseCharacter))
                {
                    if (ProgramController.DEBUGGING == true && ++totalCount > ProgramController.MAX_CHINESE_CHARACTERS_TO_LOAD) break;

                    var cc = new ChineseCharacter { };

                    cc.Rank = Int32.Parse(xlRange.Cells[i, 1].Value2.ToString());
                    cc.Chinese = xlRange.Cells[i, 2].Value2.ToString();

                    if (xlRange.Cells[i, 5].Value2 != null)
                    {
                        string pinYinFromCell = xlRange.Cells[i, 5].Value2.ToString();

                        string[] pinYins = pinYinFromCell.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                        PinYin py;

                        foreach (string pinYin in pinYins)
                        {
                            string plain = Regex.Replace(pinYin, @"[\d-]", string.Empty);
                            int tone;
                            string toneString = Regex.Match(pinYin, @"\d+").Value;

                            if (!Int32.TryParse(toneString, out tone))
                            {
                                tone = 5;
                            }

                            printLine(pinYin + "," + toneString + "," + tone);

                            py = db.PinYins.Where(p => p.Plain == plain && p.Tone == tone).SingleOrDefault();

                            if (py == null)
                            {
                                py = new PinYin();
                                py.Tone = tone;
                                py.Plain = plain;
                                db.PinYins.Add(py);
                            }

                            cc.PinYins.Add(py);
                            db.SaveChanges();
                        }
                    }

                    if (xlRange.Cells[i, 6].Value2 != null)
                    {
                        cc.English = xlRange.Cells[i, 6].Value2.ToString();
                    }

                    db.ChineseCharacters.AddOrUpdate(p => p.Chinese, cc);
                }
                else if (typeof(fieldClass) == typeof(ChineseWord))
                {
                    if (ProgramController.DEBUGGING == true && ++totalCount > ProgramController.MAX_CHINESE_WORDS_TO_LOAD) break;

                    string chinese = xlRange.Cells[i, 2].Value2.ToString();
                    string pinyin = xlRange.Cells[i, 4].Value2.ToString();

                    var cw = new ChineseWord { Chinese = xlRange.Cells[i, 2].Value2.ToString() };

                    //var cw = db.ChineseWords.Where(c => c.Chinese.Equals(chinese)).SingleOrDefault();

                    //if (cw == null)
                    //{
                    //    cw = new ChineseWord { Chinese = xlRange.Cells[i, 2].Value2.ToString() };
                    //}
                    //else
                    //{
                    //    cw = db.ChineseWords.Where(c => c.Chinese.Equals(chinese)).SingleOrDefault();
                    //    printLine(chinese);
                    //}

                    //cw = new ChineseWord { Chinese = xlRange.Cells[i, 2].Value2.ToString() };


                    cw.position = Int32.Parse(xlRange.Cells[i, 1].Value2.ToString());
                    cw.knowLevel = Int32.Parse(xlRange.Cells[i, 3].Value2.ToString());
                    cw.PinYin = pinyin;

                    PinYinWord pyw = db.PinYinWords.Where(p => p.PinYin.Equals(pinyin)).SingleOrDefault();

                    if (pyw == null)
                    {
                        pyw = new PinYinWord { PinYin = pinyin };
                    }

                    cw.possiblePYWs.Add(pyw);

                    cw.English = xlRange.Cells[i, 5].Value2.ToString();

                    db.ChineseWords.AddOrUpdate(p => p.Chinese, cw);

                    //if (cw == null)
                    //{                        
                    //    db.ChineseWords.AddOrUpdate(p => p.Chinese, cw);
                    //}                    
                }

                db.SaveChanges();
            }

            xlWorkbook.Close();

            db.SaveChanges();

            stopWatch.Stop();
            //view.printInStatusLabel("Done" + "\nTime elapsed: " + stopWatch.Elapsed);

            printLine(totalCount - 1 + " items Added\n");
        }

        internal void ExportWordsAndCharacters()
        {
            int hightsId = highestIdOfFolders("backUp_", Properties.Settings.Default.dbBackUpFolder);

            string dbDirPath = Properties.Settings.Default.dbBackUpFolder + "/backUp_" + (hightsId + 1);

            Directory.CreateDirectory(dbDirPath);

            using (MyDbContext db = new MyDbContext())
            {
                foreach (string tableName in tableNamesToBackUp)
                {
                    string query = " SELECT * FROM " + tableName + " INTO OUTFILE '" + dbDirPath + "/" + tableName + ".csv'  fields terminated by ';' enclosed by '\"' lines terminated by '\\n'";
                    printLine("Exporting to table " + tableName + ": " + db.Database.ExecuteSqlCommand(query));
                }
            }
        }

        internal void ImportWordsAndCharacters()
        {
            ClearAll();

            int hightsId = highestIdOfFolders("backUp_", Properties.Settings.Default.dbBackUpFolder);

            string dbDirPath = Properties.Settings.Default.dbBackUpFolder + "/backUp_" + hightsId;

            using (MyDbContext db = new MyDbContext())
            {
                foreach (string tableName in tableNamesToBackUp)
                {
                    string query = "load data local infile '" + dbDirPath + "/" + tableName + ".csv' into table " + tableName + " fields terminated by ';' enclosed by '\"' lines terminated by '\\n'";
                    printLine("Importing to table " + tableName + ": " + db.Database.ExecuteSqlCommand(query));
                }
            }
        }

        private int highestIdOfFolders(string prefix, string folderName1)
        {
            string[] subDirectories = Directory.GetDirectories(folderName1);
            List<string> directoryNames = new List<string>();
            int hightsId = 0;

            foreach (string directory in subDirectories)
            {
                string folderName = Path.GetFileName(directory);

                if (folderName.StartsWith(prefix))
                {
                    string digitsOnly = new String(folderName.Where(Char.IsDigit).ToArray());
                    printLine(digitsOnly);
                    int folderId = Int32.Parse(digitsOnly);
                    if (folderId > hightsId)
                        hightsId = folderId;
                }
            }

            return hightsId;
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
            RelateAllSubtitlesToWords();
        }

        private void RelateAllSubtitlesToWords()
        {
            using (MyDbContext db = new MyDbContext())
            {

            }
        }

        private void UpdateTranslationAndTranslit()
        {
            using (OurWebClient webClient = new OurWebClient())
            using (MyDbContext db = new MyDbContext())
            {
                int counter = 0;

                foreach (SubtitleLine stl in db.SubtitleLines.Where(p => p.English == null && p.Translit == null).ToList())
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

                    result = answer.Substring(answer.IndexOf("<span title=\"") + "<span title=\"".Length);
                    result = result.Substring(result.IndexOf(">") + 1);
                    result = result.Substring(0, result.IndexOf("</span>"));

                    if (result.Length > 200)
                    {
                        printLine(result);
                    }

                    if (!result.StartsWith("<html><head><meta content=")){

                        

                        English = HttpUtility.HtmlDecode(result.Trim());

                        result = answer.Substring(answer.IndexOf("src-translit") + "< div id=src-translit".Length);
                        result = result.Substring(result.IndexOf(">") + 1);
                        result = result.Substring(0, result.IndexOf("</div>"));

                        Translit = HttpUtility.HtmlDecode(result.Trim());
                    }


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

                ClearSubtitles();

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
                db.Database.ExecuteSqlCommand("DELETE FROM pinyinwords");
                printLine("Finished clearing Table ChineseWords");
            }
        }

        internal void ClearCharacters()
        {
            using (MyDbContext db = new MyDbContext())
            {
                printLine("Clearing Table ChineseCharacters");

                string[] tableNames = { "pinyinchinesecharacters", "chinesecharacters", "pinyins" };

                foreach (string tableName in tableNames)
                {
                    string query = "DELETE FROM " + tableName;
                    printLine("Deleting from " + tableName + ": " + db.Database.ExecuteSqlCommand(query));
                }
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


