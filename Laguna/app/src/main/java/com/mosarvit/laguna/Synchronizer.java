package com.mosarvit.laguna;

import android.content.Context;
import android.content.SharedPreferences;
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

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Created by Mosarvit on 11/3/2017.
 */

public class Synchronizer<T extends OnServerModel> {

    SyncSession syncSession;
    private RequestQueue requestQueue;

    private int requestCount = 0;

    private int largestRemoteIdOnServer;

    private boolean success = true;
    private boolean syncOver;

    private ArrayList<T> newFcsHere = new ArrayList<>();
    private HashMap<Integer, T> notNewFcsHere = new HashMap<>();

    private HashMap<Integer, T> allFcsHere;
    private HashMap<Integer, T> fcsServer = new HashMap<>();

    private ArrayList<T> toInsertHere = new ArrayList<>();
    private ArrayList<T> toInsertOnServer = new ArrayList<>();

    private ArrayList<T> toDeleteHere = new ArrayList<>();
    private ArrayList<T> toDeleteOnServer = new ArrayList<>();

    private ArrayList<T> updatedOnlyHereAfterLoading = new ArrayList<>();
    private ArrayList<T> updatedOnlyOnServerAfterLoading = new ArrayList<>();

    private ArrayList<T> contradictingHereVersion = new ArrayList<>();
    private ArrayList<T> contradictingServerVersion = new ArrayList<>();

    private Class<T> modelType;

    public T create(int remote_id, long updatetime, long utwhenloaded, boolean isNew) throws InstantiationException, IllegalAccessException {
        T mt = modelType.newInstance();
        mt.remote_id = remote_id;
        mt.updatetime = updatetime;
        mt.utwhenloaded = utwhenloaded;
        mt.isNew = isNew;
        mt.toDelete = false;
        return mt;
    }


    public Synchronizer(SyncSession syncSession, Class<T> modelType) {

        this.syncSession = syncSession;
        this.modelType = modelType;

        setLastSyncSuccessful(false);
        syncSession.syncOver = false;

        requestQueue = Volley.newRequestQueue(syncSession.getApplicationContext());
    }

    public void synchronize() {

        nextTask();
    }

    private void setLastSyncSuccessful(boolean synci) {
        SharedPreferences sharedPreferecnes = syncSession.getSharedPreferences("SyncStatus", Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPreferecnes.edit();
        editor.putBoolean("lastSyncSuccessful", synci);
        editor.apply();

//
//        erverModel> g = new ArrayList<>();
//        g.get(1).
//        ArrayList<? extends OnS


    }
//
//    private void insertInApp() {
//        printLine("Requesting " + toInsertHere.size() + " Flashcards");
//
//        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/get_flashcards_by_id.php", new Response.Listener<String>() {
//            @Override
//            public void onResponse(String response) {
//
////                printLine("\n insertInApp(), response: \n\n");
//
//                JSONArray JA = null;
//                try {
//                    JA = new JSONArray(response);
//                    printLine("Saving or updating " + JA.length() + " Flashcards");
//                    if (JA.length() != toInsertHere.size()) {
//                        badResponse("To little Flashcards arrived");
//                    }
//
//                    for (int i = 0; i < JA.length(); i++) {
//
//                        JSONObject JO = (JSONObject) JA.get(i);
//
//                        int remote_id = JO.getInt("remote_id");
//                        String question = JO.getString("question");
//                        long duetime = JO.getLong("duetime");
//                        long updatetime = JO.getLong("updatetime");
//                        boolean newfc = false;
//
//                        if (allFcsHere.keySet().contains(remote_id)) {
//
//                            new Update(Flashcard.class).set(
//                                    "question = '" + question +
//                                            "', duetime = " + duetime +
//                                            ", updatetime = " + updatetime +
//                                            ", utwhenloaded = " + updatetime +
//                                            ", isNew = " + newfc)
//                                    .where("remote_id = " + remote_id).execute();
//                        } else {
//                            Flashcard fc = new Flashcard(remote_id, question, duetime, updatetime, newfc);
//                            fc.save();
//                        }
//                    }
//
//                } catch (JSONException e) {
//                    e.printStackTrace();
//                }
//
//                nextTask();
//            }
//        },
//                new Response.ErrorListener() {
//                    @Override
//                    public void onErrorResponse(VolleyError volleyError) {
//                        onVolleyError(volleyError);
//                    }
//                }) {
//
//            @Override
//            protected Map<String, String> getParams() throws AuthFailureError {
//
//                Map<String, String> parameters = new HashMap<String, String>();
//
//                int i = 0;
//
//                for (Flashcard fc : toInsertHere) {
//                    parameters.put("remote_ids[" + i++ + "]", Integer.toString(fc.remote_id));
//                }
//
//                return parameters;
//            }
//        };
//
//        requestQueue.add(request);
//
//    }

    private void onVolleyError(VolleyError volleyError) {
        printLine(volleyError.toString());

        if (volleyError.networkResponse == null) {
            if (volleyError.getClass().equals(TimeoutError.class)) {
                // Show timeout error message
                Toast.makeText(syncSession.getApplication(),
                        "Oops. Timeout error!",
                        Toast.LENGTH_LONG).show();
            }
        }

        success = false;
        finishSync();
    }

//    private void deleteOnServer() {
//
//        printLine("Deleting " + toDeleteOnServer.size() + " Flashcards from server");
//
//        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/delete_flashcards_by_id.php", new Response.Listener<String>() {
//            @Override
//            public void onResponse(String response) {
//
//                if (response.equals("1")) {
//                    printLine("Successfully deleted " + toDeleteOnServer.size() + " Flashcards from server");
//                } else {
//                    badResponse(response);
//                }
//
//                nextTask();
//            }
//        },
//                new Response.ErrorListener() {
//                    @Override
//                    public void onErrorResponse(VolleyError volleyError) {
//                        onVolleyError(volleyError);
//                    }
//                }) {
//
//            @Override
//            protected Map<String, String> getParams() throws AuthFailureError {
//
//                Map<String, String> parameters = new HashMap<String, String>();
//
//                int i = 0;
//
//                for (Flashcard fc : toDeleteOnServer) {
//                    parameters.put("remote_ids[" + i++ + "]", Integer.toString(fc.remote_id));
//                }
//
//                return parameters;
//            }
//        };
//
//        requestQueue.add(request);
//
//    }

    private void badResponse(String response) {
        System.out.println(response);
        printLine(response);
        success = false;
    }

//    private void insertToServer() {
//
//        printLine("Inserting " + toInsertOnServer.size() + " Flashcards to server");
//
//        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/insert_or_update_flashcards_by_id.php", new Response.Listener<String>() {
//            @Override
//            public void onResponse(String response) {
//
//                if (response.equals("1")) {
//
//                    makeLocalChangesOnSeccessfulSession();
//
//                } else {
//                    badResponse(response);
//                }
//
//                nextTask();
//            }
//        },
//                new Response.ErrorListener() {
//                    @Override
//                    public void onErrorResponse(VolleyError volleyError) {
//                        onVolleyError(volleyError);
//                    }
//                }) {
//
//            @Override
//            protected Map<String, String> getParams() throws AuthFailureError {
//
//                Map<String, String> parameters = new HashMap<String, String>();
//
//                int i = 0;
//
//                for (Flashcard fc : toInsertOnServer) {
//
//                    parameters.put("ids[" + i + "]", Integer.toString(fc.remote_id));
//                    parameters.put("questions[" + i + "]", fc.getQuestion());
//                    parameters.put("duetimes[" + i + "]", Long.toString(fc.getUpdateTimeLocal()));
//                    parameters.put("updatetimes[" + i + "]", Long.toString(fc.getUpdateTimeLocal()));
//
//                    i++;
//                }
//
//                return parameters;
//            }
//        };
//
//        requestQueue.add(request);
//
//    }

    private void insertDeleteSelectRequest() {

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/insert_delete_request_flashcards.php", new Response.Listener<String>() {
            @Override
            public void onResponse(String response) {

                if (response.charAt(0) == ' ')
                    response = response.substring(1);

                JSONArray JA = null;
                try {
                    JA = new JSONArray(response);
                    printLine("Saving or updating " + JA.length() + " Flashcards");
                    if (JA.length() != toInsertHere.size()) {
                        badResponse("To little Flashcards arrived");
                    }

                    for (int i = 0; i < JA.length(); i++) {

                        JSONObject JO = (JSONObject) JA.get(i);

                        int remote_id = JO.getInt("remote_id");
                        String question = JO.getString("question");
                        long duetime = JO.getLong("duetime");
                        long updatetime = JO.getLong("updatetime");
                        boolean newfc = false;

                        if (allFcsHere.keySet().contains(remote_id)) {

                            new Update(Flashcard.class).set(
                                    "question = '" + question +
                                            "', duetime = " + duetime +
                                            ", updatetime = " + updatetime +
                                            ", utwhenloaded = " + updatetime +
                                            ", isNew = " + newfc)
                                    .where("remote_id = " + remote_id).execute();
                        } else {
                            Flashcard fc = new Flashcard(remote_id, question, duetime, updatetime, newfc);
                            fc.save();
                        }
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }

                if (JA != null || response.equals("success")) {

                    makeLocalChangesOnSeccessfulSession();

                } else {

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

                if (modelType == Flashcard.class){

                    for (Flashcard fc : (ArrayList<Flashcard>)toInsertOnServer) {

                        parameters.put("remote_ids_insert[" + i + "]", Integer.toString(fc.remote_id));
                        parameters.put("questions[" + i + "]", fc.getQuestion());
                        parameters.put("duetimes[" + i + "]", Long.toString(fc.getDuetime()));
                        parameters.put("updatetimes[" + i + "]", Long.toString(fc.updatetime));

                        i++;
                    }

                    i = 0;

                    for (Flashcard fc : (ArrayList<Flashcard>)toDeleteOnServer) {


                        parameters.put("remote_ids_delete[" + i + "]", Integer.toString(fc.remote_id));

                        i++;
                    }

                    i = 0;

                    for (Flashcard fc : (ArrayList<Flashcard>)toInsertHere) {


                        parameters.put("remote_ids_select[" + i + "]", Integer.toString(fc.remote_id));

                        i++;
                    }
                }
                else if(modelType == MediaFileSegment.class){

                }


                return parameters;
            }
        };

        requestQueue.add(request);

    }

    private void makeLocalChangesOnSeccessfulSession() {

        for (T fc : toInsertOnServer) {
            fc.utwhenloaded = fc.updatetime;
            fc.save();
        }

        for (T fc : toDeleteOnServer) {
            fc.delete();
        }

        for (T fc : newFcsHere) {
            fc.isNew = false;
            fc.save();
        }
    }

    private void fillUpCollections() {

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/get_to_check_for_sync.php", new Response.Listener<String>() {
            @Override
            public void onResponse(String response) {

                try {
                    fcsServer = fillUpFcsServer(response);
                } catch (IllegalAccessException e) {
                    e.printStackTrace();
                } catch (InstantiationException e) {
                    e.printStackTrace();
                }

                largestRemoteIdOnServer = getLargestRemoteId();

                fillUpAllHMs();

                splitNewAndNotNew();

                assignRemoteIdsOfTheNewFcs();

                toInsertOnServer.addAll(newFcsHere);

                for (T notNewFcHere : notNewFcsHere.values()) {

                    T fcServer;

                    if (!fcsServer.keySet().contains(notNewFcHere.remote_id)) {

                        toDeleteHere.add(notNewFcHere);

                    } else {

                        fcServer = fcsServer.get(notNewFcHere.remote_id);


                        ////debug
                        printLine(  "\nremote_id : " + notNewFcHere.remote_id +
                                    "\nnotNewFcHere.updatetime : " + notNewFcHere.updatetime +
                                    "\nnotNewFcHere.utwhenloaded : " + notNewFcHere.utwhenloaded +
                                    "\nfcServer.utwhenloaded : " + fcServer.utwhenloaded
                        );


                        if (notNewFcHere.updatetime > notNewFcHere.utwhenloaded
                                && fcServer.utwhenloaded > notNewFcHere.utwhenloaded) {

                            contradictingHereVersion.add(notNewFcHere);
                            contradictingServerVersion.add(fcServer);

                        } else if (notNewFcHere.updatetime > notNewFcHere.utwhenloaded) {

                            updatedOnlyHereAfterLoading.add(notNewFcHere);

                        } else if (fcServer.utwhenloaded > notNewFcHere.utwhenloaded) {

                            updatedOnlyOnServerAfterLoading.add(fcServer);
                        }
                    }
                }

                for (T fcServer : fcsServer.values()) {

                    if (!notNewFcsHere.keySet().contains(fcServer.remote_id)) {
                        toInsertHere.add(fcServer);
                    }
                }

                ////DEBUG
                printLine("toDeleteHere.size(): " + toDeleteHere.size() +
                        "\ntoInsertHere.size(): " + toInsertHere.size() +
                        "\ncontradictingHereVersion: " + contradictingHereVersion.size() +
                        "\ncontradictingServerVersion: " + contradictingServerVersion.size() +
                        "\nupdatedOnlyHereAfterLoading: " + updatedOnlyHereAfterLoading.size() +
                        "\nupdatedOnlyOnServerAfterLoading: " + updatedOnlyOnServerAfterLoading.size());

                if (contradictingHereVersion.size() > 0) {
                    syncSession.makeADecisionsAboutTheContradictions();
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
                    parameters.put("remote_ids[0]", "1");
                } else if (modelType == MediaFileSegment.class) {
                    parameters.put("mediafilesegment[0]", "1");
                }


                return parameters;
            }
        };

        requestQueue.add(request);
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
        toInsertHere.addAll(updatedOnlyOnServerAfterLoading);

        ////DEBUG
        printLine("\ntoDeleteOnServer: " + toDeleteOnServer.size() +
                "\ntoDeleteHere: " + toDeleteHere.size() +
                "\ntoInsertOnServer: " + toInsertOnServer.size() +
                "\ntoInsertHere: " + toInsertHere.size());

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

    private void assignRemoteIdsOfTheNewFcs() {
        int i = largestRemoteIdOnServer;

        for (OnServerModel fc : newFcsHere) {

            fc.remote_id = i;
            fc.save();
            i++;
        }
    }

    private void splitNewAndNotNew() {

        for (T fc : allFcsHere.values()) {
            if (fc.isNew) {
                newFcsHere.add(fc);
            } else {
                notNewFcsHere.put(fc.remote_id, fc);
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

    private HashMap<Integer, T> fillUpFcsServer(String response) throws IllegalAccessException, InstantiationException {


        JSONArray JA = null;
        try {
            JA = new JSONArray(response);
            for (int i = 0; i < JA.length(); i++) {

                JSONObject JO = (JSONObject) JA.get(i);

                int remote_id = JO.getInt("remote_id");

                T fc = create(JO.getInt("remote_id"), JO.getLong("updatetime"), System.currentTimeMillis(), false);
                fcsServer.put(remote_id, fc);
            }
        } catch (JSONException e) {

            success = false;
            e.printStackTrace();
        }

        return fcsServer;
    }

    private void finishSync() {

        printLine("Number of Flashcards in Db: " + new Select().from(Flashcard.class).count());

        syncOver = true;
        syncSession.setFinishOnTouchOutside(true);
        setLastSyncSuccessful(success);
        if (success) {
            printLine("Syncing finished, Everything is up to date!");
        } else {
            printLine("Something went wrong in sync, please try again later");
        }
        ;
    }

    private void cancelSync() {

        syncOver = true;
        syncSession.setFinishOnTouchOutside(true);
        printLine("Sync cancelled. Please perform a sync again soon.");
    }

    private void deleteToDeleteHere() {

        printLine("Deleting on App");

        for (T fc : toDeleteHere) {

            fc.delete();
        }

        nextTask();
    }

    private ArrayList<Integer> getIds(List<Flashcard> fcsApp) {
        ArrayList<Integer> ids = new ArrayList<>();
        for (Flashcard fc : fcsApp) ids.add(fc.remote_id);
        return ids;
    }


    public void printLine(String str) {

        this.syncSession.printLine(str);
    }

    public void nextTask() {

        requestCount++;

        if (requestCount == 1) {

            fillUpCollections();
            requestCount +=7;
            return;

        } else if (requestCount == 2) {

            if ((toInsertHere.size() > 0 || toDeleteOnServer.size() > 0 || toInsertOnServer.size() > 0) && success)
                insertDeleteSelectRequest();
            else
                nextTask();
            return;

        } else if (requestCount == 3) {

            if (toDeleteHere.size() > 0)
                deleteToDeleteHere();
            else
                nextTask();
            return;
        } else if (requestCount == 4) {

            finishSync();
            return;
        }

    }
}
