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

        private List<OnServerModel> toInsertHere = new List<OnServerModel>();
        private List<OnServerModel> toInsertOnServer = new List<OnServerModel>();

        private List<OnServerModel> toDeleteHere = new List<OnServerModel>();

        

        private List<OnServerModel> toDeleteOnServer = new List<OnServerModel>();

        private List<OnServerModel> updatedOnlyHereAfterLoading = new List<OnServerModel>();
        private List<OnServerModel> updatedOnlyOnServerHereAfterLoading = new List<OnServerModel>();

        private List<OnServerModel> contradictingHereVersion = new List<OnServerModel>();
        private List<OnServerModel> contradictingServerVersion = new List<OnServerModel>();

        private int largestRemoteIdOnServer;

        public bool syncSuccessful = true;

        public Syncronizer(ISessionView view)
        {
            this.view = view;
        }

        internal void syncronize()
        {
            FillUpCollections();
        }

        private void FillUpCollections()
        {
            using (MyDbContext db = new MyDbContext())
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

                    printLine("Time out");
                    return;
                }

                response = System.Text.Encoding.UTF8.GetString(serverResponse);


                printLine("response: " + response);

                fillUpFcsServer(response);

                largestRemoteIdOnServer = GetLargestRemoteId();

                printLine("largestRemoteIdOnServer : " + largestRemoteIdOnServer);
                printLine("fcsServer.Count() : " + fcsServer.Count());



                SplitNewAndNotNew(db);

                AssignRemoteIdsOfTheNewFcs();

                toInsertOnServer.AddRange(newFcsHere);

                foreach (OnServerModel notNewFcHere in notNewFcsHere.Values)
                {
                    OnServerModel fcServer;

                    if (!fcsServer.Keys.Contains(notNewFcHere.remote_id))
                    {
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
                            updatedOnlyOnServerHereAfterLoading.Add(fcServer);
                        }

                    }
                }

                foreach (T fcServer in fcsServer.Values)
                {
                    if (!notNewFcsHere.Keys.Contains(fcServer.remote_id))
                    {
                        toInsertHere.Add(fcServer);
                    }
                }

                ////DEBUG
                printLine("\r\nnewFcsHere : " + newFcsHere.Count() +
                            "\r\ntoDeleteHere : " + toDeleteHere.Count() +
                            "\r\ntoInsertHere : " + toInsertHere.Count() +
                            "\r\ncontradictingHereVersion: " + contradictingHereVersion.Count() +
                            "\r\ncontradictingServerVersion: " + contradictingServerVersion.Count() +
                            "\r\nmoreRecentHereVersion: " + updatedOnlyHereAfterLoading.Count() +
                            "\r\nmoreRecentServerVersion: " + updatedOnlyOnServerHereAfterLoading.Count());

                if (contradictingHereVersion.Count() > 0)
                {
                    MakeADecisionsAboutTheContradictions();
                }

                SplitToInsertAndToDeleteOnServer();

                toInsertHere.AddRange(updatedOnlyOnServerHereAfterLoading);

                ////DEBUG
                printLine("\r\ntoDeleteOnServer: " + toDeleteOnServer.Count() +
                            "\r\ntoDeleteHere: " + toDeleteHere.Count() +
                            "\r\ntoInsertOnServer: " + toInsertOnServer.Count() +
                            "\r\ntoInsertHere: " + toInsertHere.Count());

                InsertDeleteSelectRequest(client, db);

                DeleteToDeleteHere(db);


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
        }

        private void SplitToInsertAndToDeleteOnServer()
        {
            foreach (T fcHere in updatedOnlyHereAfterLoading)
            {
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
                updatedOnlyOnServerHereAfterLoading.AddRange(contradictingServerVersion);
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

        private void SplitNewAndNotNew(MyDbContext db)
        {
            if (typeof(T) == typeof(Flashcard))
            {
                foreach (Flashcard fc in db.Flashcards.ToList())
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
            }
            else if (typeof(T) == typeof(MediaFileSegment))
            {
                foreach (MediaFileSegment fc in db.MediaFileSegments.ToList())
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
            }

        }

        private void fillUpFcsServer(string response)
        {
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
                }
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {

            }
            catch (System.NullReferenceException)
            {

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

        private void DeleteToDeleteHere(MyDbContext db)
        {
            foreach (OnServerModel fc in toDeleteHere)
            {
                if (typeof(T) == typeof(Flashcard))
                {
                    db.Flashcards.Remove((Flashcard)fc);
                }
                else if (typeof(T) == typeof(MediaFileSegment))
                {
                    db.MediaFileSegments.Remove((MediaFileSegment)fc);
                }
            }

            db.SaveChanges();
        }

        private void finishSync()
        {

        }

        private void InsertDeleteSelectRequest(WebClient client, MyDbContext db)
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
                printLine("Time out");
                return;
            }

            String response = System.Text.Encoding.UTF8.GetString(serverResponse);
            printLine("response: " + response);

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

                        db.Flashcards.AddOrUpdate(p => p.remote_id, fc);
                    }
                    else if (typeof(T) == typeof(MediaFileSegment))
                    {
                        MediaFileSegment fc = new MediaFileSegment
                        {
                            remote_id = item.remote_id,
                            utserverwhenloaded = item.updatetime,
                            utlocal = item.updatetime,
                            FileName = item.filename,
                            MediaFileName = item.mediafilename,
                            isNew = false
                        };

                        db.MediaFileSegments.AddOrUpdate(p => p.remote_id, fc);
                    }
                }
            }
            catch (Newtonsoft.Json.JsonReaderException) { }

            if (dynJson != null || response.Equals("success"))
            {
                MakeLocalChangesOnSeccessfulSession(db);
            }
            else
            {
                badResponse(db, response);
            }




        }

        private void badResponse(MyDbContext db, string response)
        {
            printLine("Bad response: \r\n" + response);
            syncSuccessful = false;
        }

        private void MakeLocalChangesOnSeccessfulSession(MyDbContext db)
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
            }
            else if (typeof(T) == typeof(MediaFileSegment))
            {
                foreach (MediaFileSegment fc in toDeleteOnServer)
                {
                    db.MediaFileSegments.Remove(entity: (MediaFileSegment)fc);
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

                foreach (T fc in toInsertHere)
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
                    parameters.Add("filenames[" + i + "]", fc.MediaFileName);
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

                foreach (T fc in toInsertHere)
                {
                    parameters.Add("remote_mediafilesegment_ids_select[" + i + "]", fc.remote_id.ToString());
                    i++;
                }
            }





            return parameters;
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
