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

namespace FlashcardMaker.Controllers
{
    public class SyncController : IController
    {
        private SyncView view;

        private List<Flashcard> newFcsHere = new List<Flashcard>();
        private Dictionary<int, Flashcard> notNewFcsHere = new Dictionary<int, Flashcard>();
        private Dictionary<int, Flashcard> fcsServer = new Dictionary<int, Flashcard>();

        private List<Flashcard> toInsertHere = new List<Flashcard>();
        private List<Flashcard> toInsertOnServer = new List<Flashcard>();

        private List<Flashcard> toDeleteHere = new List<Flashcard>();
        private List<Flashcard> toDeleteOnServer = new List<Flashcard>();

        private List<Flashcard> updatedOnlyHereAfterLoading = new List<Flashcard>();
        private List<Flashcard> updatedOnlyOnServerHereAfterLoading = new List<Flashcard>();

        private List<Flashcard> contradictingHereVersion = new List<Flashcard>();
        private List<Flashcard> contradictingServerVersion = new List<Flashcard>();

        private int largestRemoteIdOnServer;

        public bool syncSuccessful = true;

        public SyncController(SyncView syncView)
        {
            this.view = syncView;
        }

        internal void sync()
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


                try
                {
                    serverResponse = client.UploadValues("http://mosar.heliohost.org/get_to_check_for_sync.php", new NameValueCollection());
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

                Dictionary<int, Flashcard> fcsServer = fillUpFcsServer(response);

                largestRemoteIdOnServer = GetLargestRemoteId(fcsServer);

                SplitNewAndNotNew(db);

                AssignRemoteIdsOfTheNewFcs();

                toInsertOnServer.AddRange(newFcsHere);

                foreach (Flashcard notNewFcHere in notNewFcsHere.Values)
                {
                    Flashcard fcServer;

                    if (!fcsServer.Keys.Contains(notNewFcHere.remote_id))
                    {
                        toDeleteHere.Add(notNewFcHere);
                    }
                    else
                    {
                        fcServer = fcsServer[notNewFcHere.remote_id];

                        ////debug
                        printLine("\r\remote_id : " + notNewFcHere.remote_id +
                                    "\r\nfcHere.updatetimelocal : " + notNewFcHere.utlocal +
                                    "\r\nfcHere.toDelete : " + notNewFcHere.getToDelete() +
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

                foreach (Flashcard fcServer in fcsServer.Values)
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

                printLine("done");

            }
        }

        private void SplitToInsertAndToDeleteOnServer()
        {
            foreach (Flashcard fcHere in updatedOnlyHereAfterLoading)
            {
                if (fcHere.getToDelete())
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
            string decision = view.decideAboutConradictions();

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

            foreach (Flashcard fc in newFcsHere)
            {
                fc.remote_id = i;
                i++;
            }
        }

        private void SplitNewAndNotNew(MyDbContext db)
        {
            foreach (Flashcard fc in db.Flashcards.ToList())
            {
                if (fc.newFc)
                {
                    newFcsHere.Add(fc);
                }
                else
                {
                    notNewFcsHere[fc.remote_id] = fc;
                }
            }
        }

        private static Dictionary<int, Flashcard> fillUpFcsServer(string response)
        {
            Dictionary<int, Flashcard> fcsServer = new Dictionary<int, Flashcard>();
            try
            {


                dynamic dynJson = JsonConvert.DeserializeObject(response);
                foreach (var item in dynJson)
                {
                    int remote_id = item.remote_id;

                    Flashcard fc = new Flashcard
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

            return fcsServer;
        }

        private int GetLargestRemoteId(Dictionary<int, Flashcard> fcsServer)
        {
            int largest = 0;

            if (fcsServer.Keys.Count() != 0)
            {
                largestRemoteIdOnServer = fcsServer.Keys.Max() + 1;
            }

            return largest;
        }

        private void DeleteToDeleteHere(MyDbContext db)
        {

            foreach (Flashcard fc in toDeleteHere)
            {
                db.Flashcards.Remove(fc);
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

            List<Flashcard> fcsServer = new List<Flashcard>();

            dynamic dynJson = null;

            try
            {
                dynJson = JsonConvert.DeserializeObject(response);
                foreach (var item in dynJson)
                {
                    int remote_id = item.remote_id;

                    Flashcard fc = new Flashcard
                    {
                        remote_id = item.remote_id,
                        question = item.question,
                        duetime = item.duetime,
                        utserverwhenloaded = item.updatetime,
                        utlocal = item.updatetime,
                        newFc = false
                    };

                    db.Flashcards.AddOrUpdate(p => p.remote_id, fc);
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

            printLine("Number of Flashcards in db: " + db.Flashcards.Count());

        }

        private void badResponse(MyDbContext db, string response)
        {
            printLine("Bad response: \r\n" + response);
            syncSuccessful = false;
        }

        private void MakeLocalChangesOnSeccessfulSession(MyDbContext db)
        {
            foreach (Flashcard fc in toInsertOnServer)
            {
                fc.utserverwhenloaded = fc.utlocal;
            }

            foreach (Flashcard fc in toDeleteOnServer)
            {
                db.Flashcards.Remove(fc);
            }

            foreach (Flashcard fc in newFcsHere)
            {
                fc.newFc = false;
            }

            db.SaveChanges();

        }

        private NameValueCollection SetParameters()
        {
            NameValueCollection parameters = new NameValueCollection();

            int i = 0;

            foreach (Flashcard fc in toInsertOnServer)
            {
                parameters.Add("remote_ids_insert[" + i + "]", fc.remote_id.ToString());
                parameters.Add("questions[" + i + "]", fc.question);
                parameters.Add("duetimes[" + i + "]", fc.utlocal.ToString());
                parameters.Add("updatetimes[" + i + "]", fc.utlocal.ToString());

                i++;
            }

            i = 0;

            foreach (Flashcard fc in toDeleteOnServer)
            {
                parameters.Add("remote_ids_delete[" + i + "]", fc.remote_id.ToString());
                i++;
            }

            i = 0;

            foreach (Flashcard fc in toInsertHere)
            {
                parameters.Add("remote_ids_select[" + i + "]", fc.remote_id.ToString());
                i++;
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
