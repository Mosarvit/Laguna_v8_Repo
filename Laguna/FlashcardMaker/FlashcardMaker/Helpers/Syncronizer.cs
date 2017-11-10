using FlashcardMaker.Controllers;
using FlashcardMaker.Helpers;
using FlashcardMaker.Models;
using FlashcardMaker.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Collections;
using System.IO;

namespace FlashcardMaker.Helpers
{
    public class Syncronizer<T> where T : OnServerModel, new()
    {
        private ISessionView view;

        //private List<OnServerModel> newFcsHere = new List<OnServerModel>();
        private Dictionary<int, OnServerModel> allFcsHere = new Dictionary<int, OnServerModel>();
        //private Dictionary<int, OnServerModel> notNewFcsHere = new Dictionary<int, OnServerModel>();
        private Dictionary<int, OnServerModel> fcsServer = new Dictionary<int, OnServerModel>();

        private List<OnServerModel> toRequestFromServer = new List<OnServerModel>();
        private List<OnServerModel> toInsertHere = new List<OnServerModel>();
        private List<OnServerModel> toInsertOnServer = new List<OnServerModel>();
        private List<OnServerModel> toUpdateOnServer = new List<OnServerModel>();

        private List<OnServerModel> toDeleteHere = new List<OnServerModel>();

        private List<OnServerModel> toDeleteOnServer = new List<OnServerModel>();

        private List<OnServerModel> updatedOnlyHereAfterLoading = new List<OnServerModel>();
        private List<OnServerModel> updatedOnlyOnServerAfterLoading = new List<OnServerModel>();

        private List<OnServerModel> contradictingHereVersion = new List<OnServerModel>();
        private List<OnServerModel> contradictingServerVersion = new List<OnServerModel>();

        private string userName = Properties.Settings.Default.UserName;
        private string password = Properties.Settings.Default.Password;
        private string ftpMediaFolder = Properties.Settings.Default.FtpMediaFolder;
        private string usersMediaFolder = Properties.Settings.Default.UsersMediaFolder;
        private string applicationsMediaFolder = Properties.Settings.Default.ApplicationsMediaFolder;

        private MyDbContext db;        

        public bool dbSyncSuccessful = true;
        public bool mediaSyncSuccessful = true;
        public bool requestSuccessful = true;
        public bool fillUpSuccessful = true;

        public bool success = true;

        public Syncronizer(ISessionView view, MyDbContext db)
        {
            this.view = view;
            this.db = db;
        }

        internal void syncronize()
        {        
            using (OurWebClient client = new OurWebClient())
            {
                if (typeof(T) == typeof(MediaFileSegment))
                {
                    Updater.updateDbMediaFiles(db, view);
                    printLine("Retrying Up- and Downloads");
                    //SyncronizeMediaFiles();
                }

                FillUpCollections();

                if (typeof(T) == typeof(MediaFileSegment))
                {
                    UpdateUpDownloadFlags(db);
                }

                if (success)
                {
                    InsertDeleteSelectRequest(client);

                    if (success)
                    {
                        if (typeof(T) == typeof(MediaFileSegment))
                            SyncronizeMediaFiles();

                        if (success)
                        {
                            MakeLocalChangesOnSeccessfulSession();
                        }
                    }
                }
            }


            if (typeof(T) == typeof(Flashcard))
            {
                printLine("Number of Flashcards in db: " + db.Flashcards.Count());
            }
            else if (typeof(T) == typeof(MediaFileSegment))
            {
                printLine("Number of MediaFileSegments in db: " + db.MediaFileSegments.Count());
            }

            printLine("done");
        }

        private void FillUpCollections()
        {
            using (OurWebClient client = new OurWebClient())
            {
                string response = "";

                ServicePointManager.Expect100Continue = false;

                byte[] serverResponse = new byte[0]; ;

                printLine("Starting");


                NameValueCollection parameters = new NameValueCollection();

                if (typeof(T) == typeof(Flashcard))
                {
                    parameters.Add("flashcard[0]", "1");
                }
                else if (typeof(T) == typeof(MediaFileSegment))
                {
                    parameters.Add("mediafilesegment[0]", "1");
                }                

                int tries = 5;
                while (true)
                {
                    try
                    {
                        serverResponse = client.UploadValues("http://mosar.heliohost.org/get_to_check_for_sync.php", parameters);
                        printLine("Response received");
                        break;
                    }
                    catch (WebException)
                    {
                        printLine("Failure 11, Time out");
                        if (--tries == 0)
                        {
                            success = false;
                            printLine("Achieved number of retries");
                            break;
                        }
                        else
                        {
                            printLine("retrying");
                        }
                    }
                }

                response = Encoding.UTF8.GetString(serverResponse);

                //printLine("response3: " + response);

                fillUpFcsServer(response);
                
                printLine("fcsServer.Count() : " + fcsServer.Count());

                FillAllFcsHere();

                //AssignRemoteIdsOfTheNewFcs();
                

                foreach (T tHere in allFcsHere.Values)
                {
                    OnServerModel fcServer;

                    if (!fcsServer.Keys.Contains(tHere.remote_id))
                    {
                        if (tHere.isNew)
                        {
                            toInsertOnServer.Add(tHere);
                        }
                        else
                        {
                            toDeleteHere.Add(tHere);
                        }                        
                    }
                    else
                    {
                        fcServer = fcsServer[tHere.remote_id];

                        ////debug
                        printLine("\r\nremote_id : " + tHere.remote_id +
                                    "\r\nfcHere.updatetimelocal : " + tHere.utlocal +
                                    "\r\nfcHere.toDelete : " + tHere.toDelete +
                                    "\r\nfcHere.updatetimewhenloaded : " + tHere.utserverwhenloaded +
                                    "\r\nfcServer.updatetimewhenloaded : " + fcServer.utserverwhenloaded
                                 );
                        if (tHere.utlocal > tHere.utserverwhenloaded
                            && fcServer.utserverwhenloaded > tHere.utserverwhenloaded)
                        {
                            contradictingHereVersion.Add(tHere);
                            contradictingServerVersion.Add(fcServer);
                        }
                        else if (tHere.utlocal > tHere.utserverwhenloaded)
                        {
                            updatedOnlyHereAfterLoading.Add(tHere);
                        }
                        else if (fcServer.utserverwhenloaded > tHere.utserverwhenloaded)
                        {
                            updatedOnlyOnServerAfterLoading.Add(fcServer);
                        }

                    }
                }

                foreach (T fcServer in fcsServer.Values)
                {
                    if (!allFcsHere.Keys.Contains(fcServer.remote_id))
                    {
                        toRequestFromServer.Add(fcServer);
                    }
                }

 

                

                if (contradictingHereVersion.Count() > 0)
                {
                    MakeADecisionsAboutTheContradictions();
                }

                SplitToInsertAndToDeleteOnServer();

                toRequestFromServer.AddRange(updatedOnlyOnServerAfterLoading);

                //DEBUG
                printLine("\r\nallFcsHere : " + allFcsHere.Count() +
                            "\r\ntoDeleteHere : " + toDeleteHere.Count() +
                            "\r\ntoInsertHere : " + toInsertHere.Count() +
                            "\r\ntoInsertOnServer: " + toInsertOnServer.Count() +
                            "\r\ntoUpdateOnserver: " + toUpdateOnServer.Count() +
                            "\r\ntoDeleteOnServer: " + toDeleteOnServer.Count());



            }
        }

        private void UpdateUpDownloadFlags(MyDbContext db)
        {
            foreach (MediaFileSegment mfs in toInsertHere)
            {
                mfs.ToDownload = true;
            }

            foreach (MediaFileSegment mfs in toInsertOnServer)
            {
                mfs.ToUpload = true;
            }

            foreach (MediaFileSegment mfs in toUpdateOnServer)
            {
                mfs.ToUpload = true;
            }

            db.SaveChanges();
        }

        private void SplitToInsertAndToDeleteOnServer()
        {
            foreach (T fcHere in updatedOnlyHereAfterLoading)
            {
                printLine(fcHere.toDelete.ToString());
                if (fcHere.toDelete)
                {
                    toDeleteOnServer.Add(fcHere);
                }
                else
                {
                    toUpdateOnServer.Add(fcHere);
                }
            }
        }

        private void MakeADecisionsAboutTheContradictions()
        {
            string decision = ProgramController.decideAboutConradictions();

            if (decision.Equals("DownloadFromServer"))
            {
                updatedOnlyOnServerAfterLoading.AddRange(contradictingServerVersion);
            }
            else if (decision.Equals("UploadToServer"))
            {
                updatedOnlyHereAfterLoading.AddRange(contradictingHereVersion);
            }
            else if (decision.Equals("Cancel"))
            {
                cancelSync();
            }
        }

        private void FillAllFcsHere()
        {
            // TRYY
            if (typeof(T) == typeof(Flashcard))
            {
                foreach (Flashcard fc in db.Flashcards.ToList())
                {
                    allFcsHere[fc.remote_id] = fc;
                }
            }
            else if (typeof(T) == typeof(MediaFileSegment))
            {
                foreach (MediaFileSegment fc in db.MediaFileSegments.ToList())
                {
                    allFcsHere[fc.remote_id] = fc;
                }
            }
        }

        private void fillUpFcsServer(string response)
        {
            printLine("fillUpFcsServer response: " + response);

            if (response.Equals("null")){
                return;
            }

            try
            {
                dynamic dynJson = JsonConvert.DeserializeObject(response);
                foreach (var item in dynJson)
                {
                    int remote_id = item.remote_id;

                    T fc = new T
                    {
                        remote_id = remote_id,
                        utserverwhenloaded = item.updatetime,
                        utlocal = item.updatetime
                    };

                    fcsServer[remote_id] = fc;

                    printLine("adding " + remote_id);
                }
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                printLine("Failure 3, response: " + response);
                success = false;
            }
            catch (System.NullReferenceException)
            {
                printLine("Failure 4, response: " + response);
                success = false;
            }
        }

        private int GetLargestRemoteId()
        {
            int largest = 0;

            if (fcsServer.Keys.Count() > 0)
            {
                largest = fcsServer.Keys.Max() + 1;
            }

            return largest;
        }

        private void DeleteToDeleteHere()
        {


            if (typeof(T) == typeof(Flashcard))
            {
                foreach (Flashcard fc in toDeleteHere)
                {
                    db.Flashcards.Remove(fc);
                }
            }
            else if (typeof(T) == typeof(MediaFileSegment))
            {
                foreach (MediaFileSegment fc in toDeleteHere)
                {
                    db.MediaFileSegments.Remove(fc);
                }
            }

            db.SaveChanges();
        }

        private void finishSync()
        {

        }

        private void InsertDeleteSelectRequest(WebClient client)
        {
            printLine("Starting to Make an Insert Delete Select request on server");

            client.Encoding = System.Text.Encoding.UTF8;

            if (toDeleteOnServer.Count()==0 && toInsertOnServer.Count() == 0 && toRequestFromServer.Count()==0)
            {
                return;
            }

            byte[] serverResponse = new byte[0]; ;

            NameValueCollection parameters = SetParameters();

            try
            {
                serverResponse = client.UploadValues("http://mosar.heliohost.org/insert_delete_request_flashcards.php", parameters /*new NameValueCollection() { }*/);
                printLine("Response received");
            }
            catch (WebException)
            {
                printLine("Failure 5");
                success = false;
                printLine("Time out");
                return;
            }

            String response = System.Text.Encoding.UTF8.GetString(serverResponse);
            printLine("response1: " + response);

            if (response[0] == ' ')
                response = response.Substring(1);

            dynamic dynJson = null;

            try
            {
                dynJson = JsonConvert.DeserializeObject(response);
                foreach (var item in dynJson)
                {
                    if (typeof(T) == typeof(Flashcard))
                    {
                        int remote_id = item.remote_id;
                        string question = item.question;
                        long duetime = item.duetime;
                        long utserverwhenloaded = item.updatetime;
                        long updatetime = item.updatetime;
                        int MediaFileSegment_remote_id = item.media_file_segment_remote_id;

                        Flashcard fc = Factory.InsertOrUpdateFlashcard(db, view, remote_id, updatetime, duetime, question, MediaFileSegment_remote_id, false);

                        toInsertHere.Add(fc);
                    }
                    else if (typeof(T) == typeof(MediaFileSegment))
                    {
                        int remote_id = item.remote_id;
                        long utserverwhenloaded = item.updatetime;
                        long utlocal = item.updatetime;
                        string FileName = item.filename;
                        string MediaFileName = item.mediafilename;

                        MediaFileSegment mfs = Factory.InsertOrUpdateMediaFileSegment(db, view, remote_id, utlocal, FileName, MediaFileName, false);

                        toInsertHere.Add(mfs);

                        printLine("creating mfs");

                        //db.MediaFileSegments.AddOrUpdate(p => p.remote_id, fc);
                    }
                }
            }
            catch (Newtonsoft.Json.JsonReaderException) { }

            if (dynJson != null || response.Equals("success"))
            {
                requestSuccessful = true;
            }
            else
            {
                badResponse(response);
                success = false;
            }




        }

        private void badResponse(string response)
        {
            printLine("Failure 12, response: " + response);
            dbSyncSuccessful = false;
        }

        private void MakeLocalChangesOnSeccessfulSession()
        {
            printLine("MakeLocalChangesOnSeccessfulSession() started");

            foreach (T fc in toInsertOnServer)
            {
                fc.utserverwhenloaded = fc.utlocal;
            }

            foreach (T fc in toUpdateOnServer)
            {
                fc.utserverwhenloaded = fc.utlocal;
            }


            if (typeof(T) == typeof(Flashcard))
            {
                foreach (Flashcard fc in toDeleteOnServer)
                {
                    db.Flashcards.Remove(fc);
                }
                foreach (Flashcard fc in toDeleteHere)
                {
                    db.Flashcards.Remove(fc);
                }
                foreach (Flashcard mfs in db.Flashcards.Where(c => c.isNew).ToList())
                {
                    mfs.isNew = false;
                }
            }
            else if (typeof(T) == typeof(MediaFileSegment))
            {
                foreach (MediaFileSegment mfs in toDeleteOnServer)
                {
                    SubtitleLinePack stlp = db.SubtitleLinePacks.Where(p => p.MediaFileSegments_remote_id == mfs.remote_id).SingleOrDefault();
                    if (stlp != null)
                    {
                        stlp.MediaFileSegments_remote_id = 0;
                    }
                    db.MediaFileSegments.Remove(entity: (MediaFileSegment)mfs);
                }
                foreach (MediaFileSegment mfs in toDeleteHere)
                {
                    SubtitleLinePack stlp = db.SubtitleLinePacks.Where(p => p.MediaFileSegments_remote_id == mfs.remote_id).SingleOrDefault();
                    if (stlp != null)
                    {
                        stlp.MediaFileSegments_remote_id = 0;
                    }
                    db.MediaFileSegments.Remove(entity: (MediaFileSegment)mfs);
                }
                foreach(MediaFileSegment mfs in db.MediaFileSegments.Where(c => c.isNew).ToList())
                {
                    mfs.isNew = false;
                }
            }

            db.SaveChanges();
        }

        private NameValueCollection SetParameters()
        {
            NameValueCollection parameters = new NameValueCollection();
            int i = 0;

            if (typeof(T) == typeof(Flashcard))
            {
                i = 0;
                foreach (Flashcard fc in toInsertOnServer)
                {
                    parameters.Add("remote_flashcard_ids_insert[" + i + "]", fc.remote_id.ToString());
                    parameters.Add("questions_insert[" + i + "]", fc.question);
                    parameters.Add("duetimes_insert[" + i + "]", fc.utlocal.ToString());
                    parameters.Add("updatetimes_insert[" + i + "]", fc.utlocal.ToString());
                    parameters.Add("media_file_segment_remote_ids_insert[" + i + "]", fc.MediaFileSegment_remote_id.ToString());

                    i++;
                }

                i = 0;
                foreach (Flashcard fc in toUpdateOnServer)
                {
                    parameters.Add("remote_flashcard_ids_update[" + i + "]", fc.remote_id.ToString());
                    parameters.Add("questions_update[" + i + "]", fc.question);
                    parameters.Add("duetimes_update[" + i + "]", fc.utlocal.ToString());
                    parameters.Add("updatetimes_update[" + i + "]", fc.utlocal.ToString());
                    parameters.Add("media_file_segment_remote_ids_update[" + i + "]", fc.MediaFileSegment_remote_id.ToString());

                    i++;
                }

                i = 0;

                foreach (T fc in toDeleteOnServer)
                {
                    parameters.Add("remote_flashcard_ids_delete[" + i + "]", fc.remote_id.ToString());
                    i++;
                }

                i = 0;

                foreach (T fc in toRequestFromServer)
                {
                    parameters.Add("remote_flashcard_ids_select[" + i + "]", fc.remote_id.ToString());
                    i++;
                }

            }
            else if (typeof(T) == typeof(MediaFileSegment))
            {
                i = 0;
                foreach (MediaFileSegment fc in toInsertOnServer)
                {
                    parameters.Add("remote_mediafilesegment_ids_insert[" + i + "]", fc.remote_id.ToString());
                    parameters.Add("updatetimes_insert[" + i + "]", fc.utlocal.ToString());
                    parameters.Add("filenames_insert[" + i + "]", fc.FileName);
                    parameters.Add("mediafilenames_insert[" + i + "]", fc.MediaFileName);
                    i++;
                }

                i = 0;
                foreach (MediaFileSegment fc in toUpdateOnServer)
                {
                    parameters.Add("remote_mediafilesegment_ids_update[" + i + "]", fc.remote_id.ToString());
                    parameters.Add("updatetimes_update[" + i + "]", fc.utlocal.ToString());
                    parameters.Add("filenames_update[" + i + "]", fc.FileName);
                    parameters.Add("mediafilenames_update[" + i + "]", fc.MediaFileName);
                    i++;
                }

                i = 0;

                foreach (T fc in toDeleteOnServer)
                {
                    parameters.Add("remote_mediafilesegment_ids_delete[" + i + "]", fc.remote_id.ToString());
                    i++;
                }

                i = 0;

                foreach (T fc in toRequestFromServer)
                {
                    parameters.Add("remote_mediafilesegment_ids_select[" + i + "]", fc.remote_id.ToString());
                    i++;
                }
            }

            return parameters;
        }

        internal void SyncronizeMediaFiles()
        {
            using (OurWebClient client = new OurWebClient())
            {
                printLine("SyncronizeMediaFiles starting");

                client.Credentials = new NetworkCredential(userName, password);

                DeleteMediaFilesHere();
                UploadToServer(client);
                DownloadFromServer(client);

                //DeleteOnserver(client);

                printLine("SyncronizeMediaFiles finished");
            }
        }

        private void DownloadFromServer(OurWebClient client)
        {
            foreach (MediaFileSegment mfs in db.MediaFileSegments.Where(p=>p.ToDownload).ToList())
            {
                string ftpFilePath =    ftpMediaFolder + "/" 
                                    + mfs.MediaFile.FileName + "/" 
                                    + mfs.FileName;
                string localFilePath = Path.Combine(Properties.Settings.Default.ApplicationsMediaFolder, mfs.MediaFile.FileName, mfs.FileName);

                printLine("Downloading :" + ftpFilePath);

                int tries = 5;
                while (true)
                {
                    try
                    {
                        client.DownloadFile(ftpFilePath, localFilePath);
                        printLine("done");
                        mfs.ToDownload = false;
                        break;
                    }
                    catch (WebException we)
                    {
                        if (--tries == 0)
                        {
                            success = false;
                            printLine("Failure 6: Achieved number of retries");
                            printLine("Exception: " + we.ToString());
                            break;
                        }
                        else
                        {
                            printLine("retrying");
                        }
                    }
                }

                db.SaveChanges();
            }
        }

        private void DeleteOnserver(OurWebClient client)
        {
            foreach (MediaFileSegment mfs in toDeleteOnServer)
            {
                printLine("Deleting : " + mfs.FileName);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpMediaFolder + "/" + mfs.MediaFile.FileName + "/" + mfs.FileName);

                request.Credentials = new NetworkCredential(userName, password);
                request.Method = WebRequestMethods.Ftp.DeleteFile;


                try
                {
                    using (var response = (FtpWebResponse)request.GetResponse())
                    {
                        printLine("Delete status: " + response.StatusDescription);
                    }

                }
                catch (WebException)
                {
                    //throw;
                    printLine("Failure 7");
                    success = false;
                    printLine("Time out. MediaSync not Successful");
                    return;
                }
            }
        }

        private void DeleteMediaFilesHere()
        {
            foreach (MediaFileSegment mfs in toDeleteHere)
            {
                string path = Path.Combine(applicationsMediaFolder, Path.Combine(mfs.MediaFile.FileName, mfs.FileName));
                File.Delete(path);
                printLine("Deleted : " + mfs.FileName);
            }
        }        

        private void UploadToServer(OurWebClient client)
        {
            foreach (MediaFileSegment mfs in db.MediaFileSegments.Where(p=>p.ToUpload).ToList())
            {
                List<string> directories = GetDirectoriesOnServersMediaFileFolder();

                if (mediaSyncSuccessful && !directories.Contains(mfs.MediaFile.FileName))
                {
                    printLine("need to Creat directory: " + mfs.MediaFile.FileName);
                    CreateDirectoryOnFtpServer(mfs.MediaFile.FileName);
                }

                UploadMfs(client, mfs);
            }
        }

        private void UploadMfs(OurWebClient client, MediaFileSegment mfs)
        {
            string filePathFtp = ftpMediaFolder + "/" + mfs.MediaFile.FileName + "/" + mfs.FileName;
            string fileToUpload = Path.Combine(Properties.Settings.Default.ApplicationsMediaFolder, mfs.MediaFile.FileName, mfs.FileName);

            printLine("fileToUpload : " + fileToUpload);
            printLine("filePathFtp : " + filePathFtp);

            byte[] serverResponse = new byte[0];

            int tries = 5;
            while (true)
            {
                try
                {                   
                    client.Credentials = new NetworkCredential(userName, password);
                    serverResponse = client.UploadFile(filePathFtp, "STOR", fileToUpload);
                    mfs.ToUpload = false;
                    break;
                }
                catch (WebException)
                {
                    printLine("Failure 8, Time out");
                    if (--tries == 0)
                    {
                        success = false;
                        printLine("Achieved number of retries");
                        break;
                    }
                    else
                    {
                        printLine("retrying");
                    }                   
                }
            }

            db.SaveChanges();

            var response = Encoding.UTF8.GetString(serverResponse);

            printLine("response2:" + response + "end");
        }

        private void CreateDirectoryOnFtpServer(string fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpMediaFolder + "/" + fileName);
            printLine("requesting to make dir: " + ftpMediaFolder + "/" + fileName);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(userName, password);
            request.KeepAlive = true;
            request.UseBinary = true;

            try
            {
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    printLine(response.StatusCode.ToString());
                }
            }
            catch (WebException)
            {
                //throw;
                printLine("Failure 9");
                success = false;
                printLine("Time out. MediaSync not Successful");
                return;
            }
        }

        private List<string> GetDirectoriesOnServersMediaFileFolder()
        {
            List<string> directories = new List<string>();


            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpMediaFolder);
            request.Credentials = new NetworkCredential(userName, password);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            List<string> dirInfos = new List<string>();

            try
            {
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    StreamReader streamReader = new StreamReader(response.GetResponseStream());
                    string line = streamReader.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        dirInfos.Add(line);
                        line = streamReader.ReadLine();
                    }
                    streamReader.Close();
                }
            }
            catch (WebException)
            {
                //throw;
                printLine("Failure 10");
                success = false;
                printLine("Time out. MediaSync not Successful");
                return directories;
            }

            printLine("directories on " + ftpMediaFolder + " : ");

            foreach (string dirInfo in dirInfos)
            {
                if (dirInfo.StartsWith("d"))
                {
                    string dir = dirInfo.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[8];
                    if (!dir.Equals(".") && !dir.Equals(".."))
                    {
                        directories.Add(dir);
                        printLine(dir);
                    }
                }
            }

            return directories;
        }        

        private void cancelSync()
        {
            throw new NotImplementedException();
        }

        public void printLine(string str)
        {
            view.printLine(str);
        }

        public void printStatusLabel(string str)
        {
            view.printStatusLabel(str);
        }


    }
}
