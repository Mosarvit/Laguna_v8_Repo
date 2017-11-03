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

        private List<OnServerModel> newFcsHere = new List<OnServerModel>();
        private Dictionary<int, OnServerModel> notNewFcsHere = new Dictionary<int, OnServerModel>();
        private Dictionary<int, OnServerModel> fcsServer = new Dictionary<int, OnServerModel>();

        private List<OnServerModel> toRequestFromServer = new List<OnServerModel>();
        private List<OnServerModel> toInserHere = new List<OnServerModel>();
        private List<OnServerModel> toInsertOnServer = new List<OnServerModel>();

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

        private int largestRemoteIdOnServer;

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
                }

                FillUpCollections();

                if (success)
                {
                    InsertDeleteSelectRequest(client);

                    if (success)
                    {
                        syncronizeMediaFiles();

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
                try
                {
                    serverResponse = client.UploadValues("http://mosar.heliohost.org/get_to_check_for_sync.php", parameters);
                    printLine("Response received");

                }
                catch (WebException)
                {
                    //throw;
                    success = false;
                    printLine("Time out");
                    return;
                }

                response = Encoding.UTF8.GetString(serverResponse);

                //printLine("response3: " + response);

                fillUpFcsServer(response);

                largestRemoteIdOnServer = GetLargestRemoteId();

                printLine("largestRemoteIdOnServer : " + largestRemoteIdOnServer);
                printLine("fcsServer.Count() : " + fcsServer.Count());

                SplitNewAndNotNew();

                AssignRemoteIdsOfTheNewFcs();

                toInsertOnServer.AddRange(newFcsHere);

                foreach (T notNewFcHere in notNewFcsHere.Values)
                {
                    OnServerModel fcServer;

                    if (!fcsServer.Keys.Contains(notNewFcHere.remote_id))
                    {
                        printLine("notNewFcHere.remote_id : " + notNewFcHere.remote_id);
                        printLine("have to delete");
                        
                        toDeleteHere.Add(notNewFcHere);
                    }
                    else
                    {
                        fcServer = fcsServer[notNewFcHere.remote_id];

                        ////debug
                        printLine("\r\nremote_id : " + notNewFcHere.remote_id +
                                    "\r\nfcHere.updatetimelocal : " + notNewFcHere.utlocal +
                                    "\r\nfcHere.toDelete : " + notNewFcHere.toDelete +
                                    "\r\nfcHere.updatetimewhenloaded : " + notNewFcHere.utserverwhenloaded +
                                    "\r\nfcServer.updatetimewhenloaded : " + fcServer.utserverwhenloaded
                                 );
                        if (notNewFcHere.utlocal > notNewFcHere.utserverwhenloaded
                            && fcServer.utserverwhenloaded > notNewFcHere.utserverwhenloaded)
                        {
                            contradictingHereVersion.Add(notNewFcHere);
                            contradictingServerVersion.Add(fcServer);
                        }
                        else if (notNewFcHere.utlocal > notNewFcHere.utserverwhenloaded)
                        {
                            updatedOnlyHereAfterLoading.Add(notNewFcHere);
                        }
                        else if (fcServer.utserverwhenloaded > notNewFcHere.utserverwhenloaded)
                        {
                            updatedOnlyOnServerAfterLoading.Add(fcServer);
                        }

                    }
                }

                foreach (T fcServer in fcsServer.Values)
                {
                    if (!notNewFcsHere.Keys.Contains(fcServer.remote_id))
                    {
                        toRequestFromServer.Add(fcServer);
                    }
                }

                ////DEBUG
                printLine("\r\nnewFcsHere : " + newFcsHere.Count() +
                            "\r\ntoDeleteHere : " + toDeleteHere.Count() +
                            "\r\ntoInsertHere : " + toRequestFromServer.Count() +
                            "\r\ncontradictingHereVersion: " + contradictingHereVersion.Count() +
                            "\r\ncontradictingServerVersion: " + contradictingServerVersion.Count() +
                            "\r\nmoreRecentHereVersion: " + updatedOnlyHereAfterLoading.Count() +
                            "\r\nmoreRecentServerVersion: " + updatedOnlyOnServerAfterLoading.Count());

                if (contradictingHereVersion.Count() > 0)
                {
                    MakeADecisionsAboutTheContradictions();
                }

                SplitToInsertAndToDeleteOnServer();

                toRequestFromServer.AddRange(updatedOnlyOnServerAfterLoading);

                ////DEBUG
                printLine("\r\ntoDeleteOnServer: " + toDeleteOnServer.Count() +
                            "\r\ntoDeleteHere: " + toDeleteHere.Count() +
                            "\r\ntoInsertOnServer: " + toInsertOnServer.Count() +
                            "\r\ntoInsertHere: " + toRequestFromServer.Count());



            }
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
                    toInsertOnServer.Add(fcHere);
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

        private void AssignRemoteIdsOfTheNewFcs()
        {
            int i = largestRemoteIdOnServer;

            foreach (OnServerModel fc in newFcsHere)
            {
                fc.remote_id = i;
                i++;
            }
        }

        private void SplitNewAndNotNew()
        {
            // TRYY
            //if (typeof(T) == typeof(Flashcard))
            //{
                foreach (OnServerModel fc in db.Flashcards.ToList())
                {
                    if (fc.isNew)
                    {
                        newFcsHere.Add(fc);
                    }
                    else
                    {
                        notNewFcsHere[fc.remote_id] = fc;
                    }
                }
            //}
            //else if (typeof(T) == typeof(MediaFileSegment))
            //{
            //    foreach (MediaFileSegment fc in db.MediaFileSegments.ToList())
            //    {
            //        if (fc.isNew)
            //        {
            //            newFcsHere.Add(fc);
            //        }
            //        else
            //        {
            //            notNewFcsHere[fc.remote_id] = fc;
            //        }
            //    }
            //}

        }

        private void fillUpFcsServer(string response)
        {
            printLine("fillUpFcsServer response: " + response);

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
                printLine("response3: " + response);
                success = false;
            }
            catch (System.NullReferenceException)
            {
                printLine("response3: " + response);
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

            byte[] serverResponse = new byte[0]; ;

            NameValueCollection parameters = SetParameters();

            try
            {
                serverResponse = client.UploadValues("http://mosar.heliohost.org/insert_delete_request_flashcards.php", parameters /*new NameValueCollection() { }*/);
                printLine("Response received");
            }
            catch (WebException)
            {
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
                    int remote_id = item.remote_id;



                    if (typeof(T) == typeof(Flashcard))
                    {
                        Flashcard fc = new Flashcard
                        {
                            remote_id = item.remote_id,
                            question = item.question,
                            duetime = item.duetime,
                            utserverwhenloaded = item.updatetime,
                            utlocal = item.updatetime,
                            isNew = false
                        };
                        toInserHere.Add(fc);
                    }
                    else if (typeof(T) == typeof(MediaFileSegment))
                    {
                        MediaFileSegment mfs = new MediaFileSegment
                        {
                            remote_id = item.remote_id,
                            utserverwhenloaded = item.updatetime,
                            utlocal = item.updatetime,
                            FileName = item.filename,
                            MediaFileName = item.mediafilename,
                            isNew = false
                        };

                        toInserHere.Add(mfs);

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
                requestSuccessful = false;
            }




        }

        private void badResponse(string response)
        {
            printLine("Bad response: \r\n" + response);
            dbSyncSuccessful = false;
        }

        private void MakeLocalChangesOnSeccessfulSession()
        {
            foreach (T fc in toInsertOnServer)
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
                foreach (Flashcard fc in toInserHere)
                {
                    db.Flashcards.Add(fc);
                }
            }
            else if (typeof(T) == typeof(MediaFileSegment))
            {
                foreach (MediaFileSegment mfs in toDeleteOnServer)
                {
                    db.MediaFileSegments.Remove(entity: (MediaFileSegment)mfs);
                }
                foreach (MediaFileSegment mfs in toDeleteHere)
                {
                    db.MediaFileSegments.Remove(entity: (MediaFileSegment)mfs);
                }
                foreach (MediaFileSegment mfs in toInserHere)
                {
                    Factory.InsertOrUpdateMediaFileSegment(db, view, mfs);
                }
            }


            foreach (T fc in newFcsHere)
            {
                fc.isNew = false;
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
                    parameters.Add("questions[" + i + "]", fc.question);
                    parameters.Add("duetimes[" + i + "]", fc.utlocal.ToString());
                    parameters.Add("updatetimes[" + i + "]", fc.utlocal.ToString());

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
                foreach (MediaFileSegment fc in toInsertOnServer)
                {
                    parameters.Add("remote_mediafilesegment_ids_insert[" + i + "]", fc.remote_id.ToString());
                    parameters.Add("updatetimes[" + i + "]", fc.utlocal.ToString());
                    parameters.Add("filenames[" + i + "]", fc.FileName);
                    parameters.Add("mediafilenames[" + i + "]", fc.MediaFileName);
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

        internal void syncronizeMediaFiles()
        {
            using (OurWebClient client = new OurWebClient())
            {
                printLine("Starting");

                client.Credentials = new NetworkCredential(userName, password);

                DeleteHere();
                UpLoadToInsertOnServer(client);
                DeleteOnserver(client);
                DownloadFromServer(client);
            }
        }

        private void DownloadFromServer(OurWebClient client)
        {
            foreach (MediaFileSegment mfs in toInserHere)
            {
                //string remoteUri = "ftp://mosar.heliohost.org/";
                //string fileName = "xczx.php", myStringWebResource = null;

                string ftpFilePath = ftpMediaFolder + "/" + mfs.MediaFile.FileName + "/" + mfs.FileName;
                string localFilePath = Path.Combine(Properties.Settings.Default.ApplicationsMediaFolder, mfs.MediaFile.FileName, mfs.FileName);

                printLine("Downloading :" + ftpFilePath);

                try
                {
                    client.DownloadFile(ftpFilePath, localFilePath);
                    printLine("done");

                }
                catch (WebException)
                {
                    //throw;

                    success = false;
                    printLine("Time out");
                    return;
                }
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

                    success = false;
                    printLine("Time out. MediaSync not Successful");
                    return;
                }
            }
        }

        private void DeleteHere()
        {
            foreach (MediaFileSegment mfs in toDeleteHere)
            {
                // TO-DO : this is not a nice solution at all

                string path = Path.Combine(applicationsMediaFolder, Path.Combine(mfs.MediaFile.FileName, mfs.FileName));
                File.Delete(path);
                printLine("Deleted : " + mfs.FileName);
            }
        }

        

        private void UpLoadToInsertOnServer(OurWebClient client)
        {
            foreach (MediaFileSegment mfs in toInsertOnServer)
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
            try
            {
                client.Credentials = new NetworkCredential(userName, password);
                serverResponse = client.UploadFile(filePathFtp, "STOR", fileToUpload);

            }
            catch (WebException)
            {
                success = false;
                printLine("Time out");
            }

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
