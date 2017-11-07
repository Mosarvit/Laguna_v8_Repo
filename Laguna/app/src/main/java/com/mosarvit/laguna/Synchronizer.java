package com.mosarvit.laguna;

import android.content.Context;
import android.content.SharedPreferences;
import android.os.Handler;
import android.widget.Toast;

import com.activeandroid.query.Select;
import com.activeandroid.query.Update;
import com.android.volley.AuthFailureError;
import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.TimeoutError;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Created by Mosarvit on 11/3/2017.
 */

public class Synchronizer<T extends OnServerModel> {

    SyncSession view;
    private RequestQueue requestQueue;

    private int taskCount = 0;

    private int largestRemoteIdOnServer;

    private boolean success = true;
    private boolean syncOver;

//    private ArrayList<T> newFcsHere = new ArrayList<>();
//    private HashMap<Integer, T> notNewFcsHere = new HashMap<>();

    private HashMap<Integer, T> allFcsHere = new HashMap<>();
    private HashMap<Integer, T> newFcsHere = new HashMap<>();
    private HashMap<Integer, T> notNewFcsHere = new HashMap<>();
    private HashMap<Integer, T> fcsServer = new HashMap<>();

    private ArrayList<T> toInsertHere = new ArrayList<>();

    private ArrayList<T> toRequestFromServer = new ArrayList<>();
    private ArrayList<T> toInsertOnServer = new ArrayList<>();

    private ArrayList<T> toDeleteHere = new ArrayList<>();
    private ArrayList<T> toDeleteOnServer = new ArrayList<>();

    private ArrayList<T> updatedOnlyHereAfterLoading = new ArrayList<>();
    private ArrayList<T> updatedOnlyOnServerAfterLoading = new ArrayList<>();

    private ArrayList<T> contradictingHereVersion = new ArrayList<>();
    private ArrayList<T> contradictingServerVersion = new ArrayList<>();

    private int timeOutsSoFar = 0;

    private boolean finished = false;

    private Class<T> modelType;

    private String userName = SharedData.FTP_USERNAME;
    private String password = SharedData.FTP_PASSWORD;
    private String ftpMediaFolder = SharedData.FTP_MEDIA_FOLDER;
    private int portNumber = SharedData.FTP_PORT_NUMBER;

    public T create(int remote_id, long updatetime, long utwhenloaded, boolean isNew) throws InstantiationException, IllegalAccessException {
        T mt = modelType.newInstance();
        mt.remote_id = remote_id;
        mt.updatetime = updatetime;
        mt.utwhenloaded = utwhenloaded;
        mt.toDelete = false;
        return mt;
    }


    public Synchronizer(SyncSession view, Class<T> modelType) {

        this.view = view;
        this.modelType = modelType;

        setLastSyncSuccessful(false);
        view.syncOver = false;

        requestQueue = Volley.newRequestQueue(view.getApplicationContext());
    }

    public void synchronize() {

        if (modelType == Flashcard.class) {
            printLine("\nSynchronizing Flashcards");
        } else if (modelType == MediaFileSegment.class) {
            printLine("\nSynchronizing MediaFileSegments");
            Updater.updateDbMediaFiles(view);
        }


        taskCount = 0;
        nextTask();
    }

    private void setLastSyncSuccessful(boolean synci) {
        SharedPreferences sharedPreferecnes = view.getSharedPreferences("SyncStatus", Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPreferecnes.edit();
        editor.putBoolean("lastSyncSuccessful", synci);
        editor.apply();

    }

    private void onVolleyError(VolleyError volleyError) {
        printLine(volleyError.toString());

        if (volleyError.networkResponse == null) {
            if (volleyError.getClass().equals(TimeoutError.class)) {
                // Show timeout error message

                timeOutsSoFar++;

                Toast.makeText(view.getApplication(),
                        "Oops. Timeout error!",
                        Toast.LENGTH_LONG).show();
            }
        }
        if (timeOutsSoFar < SharedData.MAX_RETRIES_ON_TIMEOUT) {
            taskCount--;
            nextTask();
        } else {
            success = false;
            finishSync();
        }

    }

    private void badResponse(String response) {
        System.out.println(response);
        printLine(response);
        success = false;
    }

    private void insertDeleteSelectRequest() {

        printLine("Starting insertDeleteSelectRequest()");
        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/insert_delete_request_flashcards.php", new Response.Listener<String>() {
            @Override
            public void onResponse(String response) {

//                printLine("response received: " + response);

                if (response.charAt(0) == ' ')
                    response = response.substring(1);

                JSONArray JA = null;
                try {
                    JA = new JSONArray(response);
                    printLine("Saving or updating " + JA.length() + " Flashcards");
                    if (JA.length() != toRequestFromServer.size()) {
                        badResponse("To little Flashcards arrived");
                    }

                    for (int i = 0; i < JA.length(); i++) {

                        JSONObject JO = (JSONObject) JA.get(i);

                        int remote_id = JO.getInt("remote_id");
                        long updatetime = JO.getLong("updatetime");

                        if (modelType == Flashcard.class) {

                            String question = JO.getString("question");
                            long duetime = JO.getLong("duetime");
                            int mediaFileSegmentRemoteId = JO.getInt("media_file_segment_remote_id");

                            if (allFcsHere.keySet().contains(remote_id)) {

                                new Update(Flashcard.class).set(
                                        "question = '" + question +
                                                "', duetime = " + duetime +
                                                ", updatetime = " + updatetime +
                                                ", utwhenloaded = " + updatetime +
                                                ", mediaFileSegmentRemoteId = " + mediaFileSegmentRemoteId)
                                        .where("remote_id = " + remote_id).execute();
                                Flashcard fc = new Select().from(Flashcard.class).where("remote_id = " + remote_id).executeSingle();

                                if (mediaFileSegmentRemoteId > 0) {
                                    MediaFileSegment mfs = new Select().from(MediaFileSegment.class).where("remote_id = " + mediaFileSegmentRemoteId).executeSingle();
                                    if (mfs != null) {
                                        fc.mediaFileSegment = mfs;
                                    }
                                }
                                fc.save();
                            } else {
                                Flashcard fc = new Flashcard(remote_id, question, duetime, updatetime, mediaFileSegmentRemoteId, false);
                                if (mediaFileSegmentRemoteId > 0) {
                                    MediaFileSegment mfs = new Select().from(MediaFileSegment.class).where("remote_id = " + mediaFileSegmentRemoteId).executeSingle();
                                    if (mfs != null) {
                                        fc.mediaFileSegment = mfs;
                                    }
                                }
                                printLine(fc.toString());

                                fc.save();

                                toInsertHere.add((T) fc);
                            }
                        } else if (modelType == MediaFileSegment.class) {

                            String mediafilename = JO.getString("mediafilename");
                            String filename = JO.getString("filename");

                            if (allFcsHere.keySet().contains(remote_id)) {
                                printLine("updating");
                                new Update(MediaFileSegment.class).set(
                                        "filename = '" + filename +
                                                "', mediafilename = '" + mediafilename +
                                                "', updatetime = " + updatetime +
                                                ", utwhenloaded = " + updatetime
                                )
                                        .where("remote_id = " + remote_id).execute();
                            } else {
                                MediaFileSegment fc = new MediaFileSegment(remote_id, updatetime, filename, mediafilename, false);
                                fc.save();
                                toInsertHere.add((T) fc);
                            }
                        }
                    }
                } catch (JSONException e) {

                }

                if (JA == null && !response.equals("success")) {

                    badResponse(response);
                }

                nextTask();
            }
        },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError volleyError) {
                        onVolleyError(volleyError);
                    }
                }) {

            @Override
            protected Map<String, String> getParams() throws AuthFailureError {

                Map<String, String> parameters = new HashMap<String, String>();


                int i = 0;

                if (modelType == Flashcard.class) {

                    for (Flashcard fc : (ArrayList<Flashcard>) toInsertOnServer) {

                        parameters.put("remote_flashcard_ids_insert[" + i + "]", Integer.toString(fc.remote_id));
                        parameters.put("questions[" + i + "]", fc.getQuestion());
                        parameters.put("duetimes[" + i + "]", Long.toString(fc.getDuetime()));
                        parameters.put("updatetimes[" + i + "]", Long.toString(fc.updatetime));
                        parameters.put("media_file_segment_remote_ids[" + i + "]", Integer.toString(fc.mediaFileSegmentRemoteId));

                        i++;
                    }

                    i = 0;

                    for (Flashcard fc : (ArrayList<Flashcard>) toDeleteOnServer) {

                        parameters.put("remote_flashcard_ids_delete[" + i + "]", Integer.toString(fc.remote_id));

                        i++;
                    }

                    i = 0;

                    for (OnServerModel fc : toRequestFromServer) {

                        parameters.put("remote_flashcard_ids_select[" + i + "]", Integer.toString(fc.remote_id));

                        i++;
                    }
                } else if (modelType == MediaFileSegment.class) {


                    for (MediaFileSegment fc : (ArrayList<MediaFileSegment>) toInsertOnServer) {

                        parameters.put("remote_mediafilesegment_ids_insert[" + i + "]", Integer.toString(fc.remote_id));
                        parameters.put("updatetimes[" + i + "]", Long.toString(fc.updatetime));
                        parameters.put("filenames[" + i + "]", fc.fileName);
                        parameters.put("mediafilenames[" + i + "]", fc.mediaFileName);

                        i++;
                    }

                    i = 0;

                    for (OnServerModel fc : toDeleteOnServer) {


                        parameters.put("remote_mediafilesegment_ids_delete[" + i + "]", Integer.toString(fc.remote_id));

                        i++;
                    }

                    i = 0;

                    for (OnServerModel fc : toRequestFromServer) {

                        parameters.put("remote_mediafilesegment_ids_select[" + i + "]", Integer.toString(fc.remote_id));

                        i++;
                    }
                }


                return parameters;
            }
        };

        requestQueue.add(request);

    }

    private void makeLocalChangesOnSeccessfulSession() {

        printLine("making local changes");

//        for(T fc:)

        printLine("updating updatetime of toInsertOnServer");

        for (T fc : toInsertOnServer) {
            fc.utwhenloaded = fc.updatetime;
            fc.save();
        }

        printLine("deleting toDeleteOnServer");

        for (T fc : toDeleteOnServer) {
            fc.delete();
        }

        printLine("deleting toDeleteHere");

        for (T fc : toDeleteHere) {
            fc.delete();
        }

        for (T t : notNewFcsHere.values()){
            t.isNew = false;
        }
        printLine("deleting toDeleteHere finished");
        nextTask();
    }

    private void fillUpCollections() {

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/get_to_check_for_sync.php", new Response.Listener<String>() {
            @Override
            public void onResponse(String response) {

                try {
                    fillUpFcsServer(response);
                } catch (IllegalAccessException e) {
                    printLine("fillUpFcsServer(response) failed");
                    printLine("response : " + response);
                    success = false;
                } catch (InstantiationException e) {
                    printLine("fillUpFcsServer(response) failed");
                    printLine("response : " + response);
                    success = false;
                }

                fillUpAllHMs();

                splitNewAndNotNew();

                for (T fcHere : allFcsHere.values()) {

                    T fcServer;

                    if (!fcsServer.keySet().contains(fcHere.remote_id)) {

                        if(fcHere.isNew){
                            toInsertOnServer.add(fcHere);
                        }else{
                            toDeleteHere.add(fcHere);
                        }

                    } else {

                        fcServer = fcsServer.get(fcHere.remote_id);


                        ////debug
//                        printLine("\nremote_id : " + fcHere.remote_id +
//                                "\nnotNewFcHere.updatetime : " + fcHere.updatetime +
//                                "\nnotNewFcHere.utwhenloaded : " + fcHere.utwhenloaded +
//                                "\nfcServer.utwhenloaded : " + fcServer.utwhenloaded
//                        );


                        if (fcHere.updatetime > fcHere.utwhenloaded
                                && fcServer.utwhenloaded > fcHere.utwhenloaded) {

                            contradictingHereVersion.add(fcHere);
                            contradictingServerVersion.add(fcServer);

                        } else if (fcHere.updatetime > fcHere.utwhenloaded) {

                            updatedOnlyHereAfterLoading.add(fcHere);

                        } else if (fcServer.utwhenloaded > fcHere.utwhenloaded) {

                            updatedOnlyOnServerAfterLoading.add(fcServer);
                        }
                    }
                }

                for (T fcServer : fcsServer.values()) {

                    if (!allFcsHere.keySet().contains(fcServer.remote_id)) {
                        toRequestFromServer.add(fcServer);
                    }
                }

                ////DEBUG
                printLine("fcsServer.size(): " + fcsServer.size() +
                        "\nallFcsHere.size(): " + allFcsHere.size() +
                        "\ntoDeleteHere.size(): " + toDeleteHere.size() +
                        "\ntoRequestFromServer.size(): " + toRequestFromServer.size() +
                        "\ncontradictingHereVersion: " + contradictingHereVersion.size() +
                        "\ncontradictingServerVersion: " + contradictingServerVersion.size() +
                        "\nupdatedOnlyHereAfterLoading: " + updatedOnlyHereAfterLoading.size() +
                        "\nupdatedOnlyOnServerAfterLoading: " + updatedOnlyOnServerAfterLoading.size());

                if (contradictingHereVersion.size() > 0) {
                    view.makeADecisionsAboutTheContradictions();
                } else {
                    continueFillingUp();
                }
            }


        },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError volleyError) {
                        onVolleyError(volleyError);
                    }
                }) {

            @Override
            protected Map<String, String> getParams() throws AuthFailureError {

                Map<String, String> parameters = new HashMap<String, String>();

                if (modelType == Flashcard.class) {
                    parameters.put("flashcard[0]", "1");
                } else if (modelType == MediaFileSegment.class) {
                    parameters.put("mediafilesegment[0]", "1");
                }


                return parameters;
            }
        };

        requestQueue.add(request);
    }

    private void splitNewAndNotNew() {

        for (T t : allFcsHere.values()){
            if(t.isNew){
                newFcsHere.put(t.remote_id, t);
            }else{
                notNewFcsHere.put(t.remote_id, t);
            }
        }
    }

    private void fillUpAllHMs() {

        if (modelType == Flashcard.class) {
            allFcsHere = (HashMap<Integer, T>) Flashcard.getAllAsHM();
        } else if (modelType == MediaFileSegment.class) {
            allFcsHere = (HashMap<Integer, T>) MediaFileSegment.getAllAsHM();
        }

    }

    private void continueFillingUp() {

        splitToInsertAndToDeleteOnServer();
        toRequestFromServer.addAll(updatedOnlyOnServerAfterLoading);

        ////DEBUG
        printLine("\ntoDeleteOnServer: " + toDeleteOnServer.size() +
                "\ntoDeleteHere: " + toDeleteHere.size() +
                "\ntoInsertOnServer: " + toInsertOnServer.size() +
                "\ntoRequestFromServer: " + toRequestFromServer.size());


        nextTask();
    }

    public void serverWins() {
        updatedOnlyOnServerAfterLoading.addAll(contradictingServerVersion);
        continueFillingUp();
    }

    public void hereWins() {
        updatedOnlyHereAfterLoading.addAll(contradictingHereVersion);
        continueFillingUp();
    }

    public void cancel() {
        cancelSync();
    }

    private void splitToInsertAndToDeleteOnServer() {
        for (T fc : updatedOnlyHereAfterLoading) {

            if (fc.toDelete) {

                toDeleteOnServer.add(fc);
            } else {

                toInsertOnServer.add(fc);
            }
        }
    }

    private int getLargestRemoteId() {

        int largest = 0;

        if (fcsServer.keySet().size() != 0) {
            largest = Collections.max(fcsServer.keySet()) + 1;
        }

        return largest;
    }

    private void fillUpFcsServer(String response) throws IllegalAccessException, InstantiationException {

        //debug
//        printLine("response:" + response);

        if (response.equals("") || response.equals(" ")) {
            return;
        }


        JSONArray JA = null;
        try {
            JA = new JSONArray(response);
            for (int i = 0; i < JA.length(); i++) {

                JSONObject JO = (JSONObject) JA.get(i);

                int remote_id = JO.getInt("remote_id");

                T fc = create(JO.getInt("remote_id"), JO.getLong("updatetime"), JO.getLong("updatetime"), false);
                fcsServer.put(remote_id, fc);
            }
        } catch (JSONException e) {

            printLine("Fail 1 : reading JSONArray failed");
            printLine("response : " + response);
            success = false;
        }
    }

    private void cancelSync() {

        syncOver = true;
        view.setFinishOnTouchOutside(true);
        printLine("Sync cancelled. Please perform a sync again soon.");
    }

    private ArrayList<Integer> getIds(List<Flashcard> fcsApp) {
        ArrayList<Integer> ids = new ArrayList<>();
        for (Flashcard fc : fcsApp) ids.add(fc.remote_id);
        return ids;
    }

    public void printLine(String str) {

        this.view.printLine(str);
    }

    public void syncronizeMediaFiles() throws IOException {

        printLine("DownloadFromServer() starting");

        try {
            DownloadFromServer();
        } catch (IOException e) {
            e.printStackTrace();
        }

        printLine("UploadToServer() starting");

        try {
            UploadToServer();
        } catch (IOException e) {
            e.printStackTrace();
        }

        nextTask();
    }

    private void UploadToServer() throws IOException {

        for (MediaFileSegment mfs : (List<MediaFileSegment>) this.toInsertHere) {
            //string remoteUri = "ftp://mosar.heliohost.org/";
            //string fileName = "xczx.php", myStringWebResource = null;

            String ftpFilePath = ftpMediaFolder + "/" + mfs.mediaFileName + "/" + mfs.fileName;
            File localFilePath = new File(SharedData.LOCAL_MEDIA_FOLDER + "/" + mfs.mediaFileName + "/" + mfs.fileName);

            new DownloadFromFtpTask(ftpFilePath.toString(), localFilePath.toString(), view).execute();

        }
    }

    private void DownloadFromServer() throws IOException {

        List<MediaFileSegment> mediaFileNames = new Select().from(MediaFileSegment.class).groupBy("mediafileName").execute();

        for (MediaFileSegment mfs : mediaFileNames) {


        }

        for (MediaFileSegment mfs : (List<MediaFileSegment>) this.toInsertHere) {
            //string remoteUri = "ftp://mosar.heliohost.org/";
            //string fileName = "xczx.php", myStringWebResource = null;

            String ftpFilePath = ftpMediaFolder + "/" + mfs.mediaFileName + "/" + mfs.fileName;
            File localFolderPath = new File(SharedData.LOCAL_MEDIA_FOLDER + "/" + mfs.mediaFileName);
            if (!localFolderPath.exists()) {
                localFolderPath.mkdirs();
            }
            File localFilePath = new File(localFolderPath + "/" + mfs.fileName);

            new DownloadFromFtpTask(ftpFilePath.toString(), localFilePath.toString(), view).execute();

        }

    }

    private void finishSync() {

        if (modelType == Flashcard.class) {
            printLine("Number of Flashcards in Db: " + new Select().from(Flashcard.class).count());
            printLine(Flashcard.allToString());
        } else if (modelType == MediaFileSegment.class) {

            printLine("Number of MediaFileSegments in Db: " + new Select().from(MediaFileSegment.class).count());
            printLine(MediaFileSegment.allToString());
        }

        syncOver = true;
        view.setFinishOnTouchOutside(true);
        setLastSyncSuccessful(success);
        if (success) {
            printLine("Syncing finished, Everything is up to date!");
        } else {
            printLine("Something went wrong in sync, please try again later");
        }
    }

    public void nextTask() {

        taskCount++;

        if (taskCount == 1) {
            view.printLine("fillUpCollections starting");
            fillUpCollections();

        } else if (taskCount == 2) {
            view.printLine("fillUpCollections finished");

            if ((toRequestFromServer.size() > 0 || toDeleteOnServer.size() > 0 || toInsertOnServer.size() > 0) && success) {
                view.printLine("insertDeleteSelectRequest starting");
                insertDeleteSelectRequest();
            } else
                nextTask();

        } else if (taskCount == 3) {

            if (success)
                makeLocalChangesOnSeccessfulSession();
            else
                nextTask();

        } else if (taskCount == 4) {

            if (success && modelType == MediaFileSegment.class) {
                try {
                    syncronizeMediaFiles();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            } else {
                nextTask();

            }

        } else if (taskCount == 5) {

            if (success && modelType == MediaFileSegment.class) {
                Synchronizer<Flashcard> sF = new Synchronizer<>(view, Flashcard.class);
                sF.synchronize();
            } else {
                finishSync();
            }
        }
    }
}
