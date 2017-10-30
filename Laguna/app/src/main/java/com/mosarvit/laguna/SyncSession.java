package com.mosarvit.laguna;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.text.method.ScrollingMovementMethod;
import android.util.DisplayMetrics;
import android.widget.TextView;
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


public class SyncSession extends Activity {

    private RequestQueue requestQueue;


    private int requestCount = 0;

    public static TextView syncTextView;


    private ArrayList<Flashcard> newFcsHere = new ArrayList<>();
    private HashMap<Integer, Flashcard> notNewFcsHere = new HashMap<>();

    private HashMap<Integer, Flashcard> allFcsHere = Flashcard.getAllAsHM();
    private HashMap<Integer, Flashcard> fcsServer = new HashMap<>();

    private ArrayList<Flashcard> toInsertHere = new ArrayList<>();
    private ArrayList<Flashcard> toInsertOnServer = new ArrayList<>();

    private ArrayList<Flashcard> toDeleteHere = new ArrayList<>();
    private ArrayList<Flashcard> toDeleteOnServer = new ArrayList<>();

    private ArrayList<Flashcard> moreRecentHereVersion = new ArrayList<>();
    private ArrayList<Flashcard> moreRecentServerVersion = new ArrayList<>();

    private ArrayList<Flashcard> contradictingHereVersion = new ArrayList<>();
    private ArrayList<Flashcard> contradictingServerVersion = new ArrayList<>();

    private int largestRemoteIdOnServer;


    private boolean syncSuccessful = true;
    private boolean syncOver;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setLastSyncSuccessful(false);

        setContentView(R.layout.sync_seesion_pop_up_layout);

        DisplayMetrics dm = new DisplayMetrics();
        getWindowManager().getDefaultDisplay().getMetrics(dm);

        int width = dm.widthPixels;
        int height  = dm.heightPixels;

        getWindow().setLayout((int)(width*.8),(int)(height*.6));

        syncTextView = findViewById(R.id.txtSyncMessage);
        syncTextView.setMovementMethod(new ScrollingMovementMethod());

        printLine("Starting the sync process");

        syncOver = false;

        requestQueue = Volley.newRequestQueue(getApplicationContext());

        nextRequest();
    }

    private void setLastSyncSuccessful(boolean synci) {
        SharedPreferences sharedPreferecnes = getSharedPreferences("SyncStatus", Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPreferecnes.edit();
        editor.putBoolean("lastSyncSuccessful", synci);
        editor.apply();
    }

    private void insertInApp() {
        printLine("Requesting " + toInsertHere.size() + " Flashcards");

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/get_flashcards_by_id.php", new Response.Listener<String>()
        {
            @Override
            public void onResponse(String response) {

//                printLine("\n insertInApp(), response: \n\n");

                JSONArray JA = null;
                try {
                    JA = new JSONArray(response);
                    printLine("Saving or updating " + JA.length() + " Flashcards");
                    if (JA.length()!= toInsertHere.size()){
                        badResponse("To little Flashcards arrived");
                    }

                    for(int i = 0; i<JA.length(); i++) {

                        JSONObject JO = (JSONObject) JA.get(i);

                        int remote_id = JO.getInt("remote_id");
                        String question = JO.getString("question");
                        long duetime = JO.getLong("duetime");
                        long updatetime = JO.getLong("updatetime");
                        boolean newfc = false;

                        if (allFcsHere.keySet().contains(remote_id)){

                            new Update(Flashcard.class).set(
                                            "question = '" + question +
                                            "', duetime = " + duetime +
                                            ", utlocal = " + updatetime +
                                            ", utwhenloaded = " + updatetime +
                                            ", newfc = " + newfc )
                                    .where("remote_id = " + remote_id).execute();
                        } else{
                            Flashcard fc = new Flashcard(remote_id, question, duetime, updatetime, newfc);
                            fc.save();
                        }
                    }

                } catch (JSONException e) {
                    e.printStackTrace();
                }

                nextRequest();
            }
        },
                new Response.ErrorListener()
                {
                    @Override
                    public void onErrorResponse(VolleyError volleyError)
                    {
                        onVolleyError(volleyError);
                    }
                }) {

            @Override
            protected Map<String, String> getParams() throws AuthFailureError {

                Map<String,String> parameters  = new HashMap<String, String>();

                int i=0;

                for (Flashcard fc : toInsertHere){
                    parameters.put("remote_ids[" + i++ + "]",Integer.toString(fc.remote_id));
                }

                return parameters;
            }
        };

        requestQueue.add(request);

    }

    private void onVolleyError(VolleyError volleyError) {
        printLine(volleyError.toString());

        if (volleyError.networkResponse == null) {
            if (volleyError.getClass().equals(TimeoutError.class)) {
                // Show timeout error message
                Toast.makeText(getApplication(),
                        "Oops. Timeout error!",
                        Toast.LENGTH_LONG).show();
            }
        }

        syncSuccessful = false;
        finishSync();
    }

    private void deleteOnServer() {

        printLine("Deleting " + toDeleteOnServer.size() + " Flashcards from server");

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/delete_flashcards_by_id.php", new Response.Listener<String>()
        {
            @Override
            public void onResponse(String response) {

                if (response.equals("1")){
                    printLine("Successfully deleted " + toDeleteOnServer.size() + " Flashcards from server");
                }else{
                    badResponse(response);
                }

                nextRequest();
            }
        },
                new Response.ErrorListener()
                {
                    @Override
                    public void onErrorResponse(VolleyError volleyError)
                    {
                        onVolleyError(volleyError);
                    }
                }) {

            @Override
            protected Map<String, String> getParams() throws AuthFailureError {

                Map<String,String> parameters  = new HashMap<String, String>();

                int i=0;

                for (Flashcard fc : toDeleteOnServer){
                    parameters.put("remote_ids[" + i++ + "]",Integer.toString(fc.remote_id));
                }

                return parameters;
            }
        };

        requestQueue.add(request);

    }

    private void badResponse(String response) {
        System.out.println(response);
        printLine(response);
        syncSuccessful = false;
    }

    private void insertToServer() {

        printLine("Inserting " + toInsertOnServer.size() + " Flashcards to server");

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/insert_or_update_flashcards_by_id.php", new Response.Listener<String>()
        {
            @Override
            public void onResponse(String response) {

                if (response.equals("1")){

                    makeLocalChangesOnSeccessfulSession();

                }else{
                    badResponse(response);
                }

                nextRequest();
            }
        },
                new Response.ErrorListener()
                {
                    @Override
                    public void onErrorResponse(VolleyError volleyError)
                    {
                        onVolleyError(volleyError);
                    }
                }) {

            @Override
            protected Map<String, String> getParams() throws AuthFailureError {

                Map<String,String> parameters  = new HashMap<String, String>();

                int i=0;

                for (Flashcard fc : toInsertOnServer){

                    parameters.put("ids[" + i + "]",Integer.toString(fc.remote_id));
                    parameters.put("questions[" + i + "]",fc.getQuestion());
                    parameters.put("duetimes[" + i + "]",Long.toString(fc.getUpdateTimeLocal()));
                    parameters.put("updatetimes[" + i + "]",Long.toString(fc.getUpdateTimeLocal()));

                    i++;
                }

                return parameters;
            }
        };

        requestQueue.add(request);

    }

    private void insertDeleteSelectRequest() {

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/insert_delete_request_flashcards.php", new Response.Listener<String>()
        {
            @Override
            public void onResponse(String response) {

                if (response.charAt(0)==' ')
                    response = response.substring(1);

                JSONArray JA = null;
                try {
                    JA = new JSONArray(response);
                    printLine("Saving or updating " + JA.length() + " Flashcards");
                    if (JA.length()!= toInsertHere.size()){
                        badResponse("To little Flashcards arrived");
                    }

                    for(int i = 0; i<JA.length(); i++) {

                        JSONObject JO = (JSONObject) JA.get(i);

                        int remote_id = JO.getInt("remote_id");
                        String question = JO.getString("question");
                        long duetime = JO.getLong("duetime");
                        long updatetime = JO.getLong("updatetime");
                        boolean newfc = false;

                        if (allFcsHere.keySet().contains(remote_id)){

                            new Update(Flashcard.class).set(
                                    "question = '" + question +
                                            "', duetime = " + duetime +
                                            ", utlocal = " + updatetime +
                                            ", utwhenloaded = " + updatetime +
                                            ", newfc = " + newfc )
                                    .where("remote_id = " + remote_id).execute();
                        } else{
                            Flashcard fc = new Flashcard(remote_id, question, duetime, updatetime, newfc);
                            fc.save();
                        }
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }

                if (JA!=null || response.equals("success")){

                    makeLocalChangesOnSeccessfulSession();

                }else{

                    badResponse(response);
                }

                nextRequest();
            }
        },
                new Response.ErrorListener()
                {
                    @Override
                    public void onErrorResponse(VolleyError volleyError)
                    {
                        onVolleyError(volleyError);
                    }
                }) {

            @Override
            protected Map<String, String> getParams() throws AuthFailureError {

                Map<String,String> parameters  = new HashMap<String, String>();

                int i=0;

                for (Flashcard fc : toInsertOnServer){

                    parameters.put("remote_ids_insert[" + i + "]",Integer.toString(fc.remote_id));
                    parameters.put("questions[" + i + "]",fc.getQuestion());
                    parameters.put("duetimes[" + i + "]",Long.toString(fc.getDuetime()));
                    parameters.put("updatetimes[" + i + "]",Long.toString(fc.utlocal));

                    i++;
                }

                i=0;

                for (Flashcard fc : toDeleteOnServer){

                    parameters.put("remote_ids_delete[" + i + "]",Integer.toString(fc.remote_id));

                    i++;
                }

                i=0;

                for (Flashcard fc : toInsertHere){

                    parameters.put("remote_ids_select[" + i + "]",Integer.toString(fc.remote_id));

                    i++;
                }

                return parameters;
            }
        };

        requestQueue.add(request);

    }

    private void makeLocalChangesOnSeccessfulSession() {

        for (Flashcard fc : toInsertOnServer) {
            fc.utwhenloaded = fc.utlocal;
            fc.save();
        }

        for (Flashcard fc : toDeleteOnServer) {
            fc.delete();
        }

        for (Flashcard fc : newFcsHere)
        {
            fc.newfc = false;
            fc.save();
        }
    }

    private void fillUpCollections() {

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/get_to_check_for_sync.php", new Response.Listener<String>()
        {
            @Override
            public void onResponse(String response) {

                fcsServer = fillUpFcsServer(response);

                largestRemoteIdOnServer =  getLargestRemoteId();

                splitNewAndNotNew();

                assignRemoteIdsOfTheNewFcs();

                toInsertOnServer.addAll(newFcsHere);

                for (Flashcard notNewFcHere : notNewFcsHere.values()){
                    
                    Flashcard fcServer; 
                    
                    if (!fcsServer.keySet().contains(notNewFcHere.remote_id)){

                        toDeleteHere.add(notNewFcHere);

                    } else {

                        fcServer = fcsServer.get(notNewFcHere.remote_id);


                         ////debug
                        printLine("\nremote_id : " + notNewFcHere.remote_id +
                                                "\nnotNewFcHere.utlocal : " + notNewFcHere.utlocal +
                                                "\nnotNewFcHere.utwhenloaded : " + notNewFcHere.utwhenloaded +
                                                "\nfcServer.utwhenloaded : " + fcServer.utwhenloaded
                                                );


                        if (notNewFcHere.utlocal > notNewFcHere.utwhenloaded
                                &&  fcServer.utwhenloaded > notNewFcHere.utwhenloaded) {

                            contradictingHereVersion.add(notNewFcHere);
                            contradictingServerVersion.add(fcServer);

                        } else if (  notNewFcHere.getUpdateTimeLocal() > notNewFcHere.getUpdatetimeWhenLoaded()) {

                            moreRecentHereVersion.add(notNewFcHere);

                        } else if (  fcServer.getUpdatetimeWhenLoaded() > notNewFcHere.getUpdatetimeWhenLoaded()) {

                            moreRecentServerVersion.add(fcServer);
                        }
                    }
                }

                for (Flashcard fcServer : fcsServer.values()){

                    if (!notNewFcsHere.keySet().contains(fcServer.remote_id)) {
                        toInsertHere.add(fcServer);
                    }
                }

                ////DEBUG
                printLine("toDeleteHere.size(): " + toDeleteHere.size() +
                        "\ntoInsertHere.size(): " + toInsertHere.size() +
                        "\ncontradictingHereVersion: " + contradictingHereVersion.size() +
                        "\ncontradictingServerVersion: " + contradictingServerVersion.size() +
                        "\nmoreRecentHereVersion: " + moreRecentHereVersion.size() +
                        "\nmoreRecentServerVersion: " + moreRecentServerVersion.size());

                if (contradictingHereVersion.size()>0){
                    makeADecisionsAboutTheContradictions();
                }else {
                    continueFillingUp();
                }
            }

            private void makeADecisionsAboutTheContradictions() {
                AlertDialog.Builder alert_builder = new AlertDialog.Builder(SyncSession.this);
                alert_builder.setMessage("There were some conradictions, how should we act in contradictory cases?").setCancelable(true)
                        .setNegativeButton(
                        "Download from Server", new DialogInterface.OnClickListener(){
                        @Override
                        public void onClick(DialogInterface dialog, int which) {

                            moreRecentServerVersion.addAll(contradictingServerVersion);
                            continueFillingUp();
                        }
                    }).setNeutralButton("Upload to Server", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {

                            moreRecentHereVersion.addAll(contradictingHereVersion);
                            continueFillingUp();
                        }
                    }).setPositiveButton("Cancel", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {

                            cancelSync();
                        }
                    }).setOnCancelListener(new DialogInterface.OnCancelListener() {
                    @Override
                    public void onCancel(DialogInterface dialog) {

                            cancelSync();
                    }
                });

                AlertDialog alert = alert_builder.create();
                alert.setTitle("Alert!");
                alert.show();
            }

            private void continueFillingUp() {

                splitToInsertAndToDeleteOnServer();
                toInsertHere.addAll(moreRecentServerVersion);

                ////DEBUG
                printLine(  "\ntoDeleteOnServer: " + toDeleteOnServer.size() +
                            "\ntoDeleteHere: " + toDeleteHere.size() +
                            "\ntoInsertOnServer: " + toInsertOnServer.size() +
                            "\ntoInsertHere: " + toInsertHere.size());

                nextRequest();
            }
        },
                new Response.ErrorListener()
                {
                    @Override
                    public void onErrorResponse(VolleyError volleyError)
                    {
                        onVolleyError(volleyError);
                    }
                }) {
        };

        requestQueue.add(request);
    }

    private void splitToInsertAndToDeleteOnServer() {
        for (Flashcard fc : moreRecentHereVersion){

            if (fc.toDelete){

                toDeleteOnServer.add(fc);
            }else{

                toInsertOnServer.add(fc);
            }
        }
    }

    private void assignRemoteIdsOfTheNewFcs() {
        int i = largestRemoteIdOnServer;

        for (Flashcard fc : newFcsHere){

            fc.remote_id = i;
            fc.save();
            i++;
        }
    }

    private void splitNewAndNotNew() {
        for (Flashcard fc : allFcsHere.values())
        {
            if (fc.newfc)
            {
                newFcsHere.add(fc);
            }
            else
            {
                notNewFcsHere.put(fc.remote_id, fc);
            }
        }
    }

    private int getLargestRemoteId() {

        int largest=0;

        if (fcsServer.keySet().size() != 0)
        {
            largest =  Collections.max(fcsServer.keySet()) + 1;
        }

        return largest;
    }

    private HashMap<Integer, Flashcard> fillUpFcsServer(String response) {

        HashMap<Integer, Flashcard> fcsServer = new HashMap<>();

        JSONArray JA = null;
        try {
            JA = new JSONArray(response);
            for(int i = 0; i<JA.length(); i++) {

                JSONObject JO = (JSONObject) JA.get(i);

                int remote_id = JO.getInt("remote_id");

                Flashcard fc = new Flashcard(JO.getInt("remote_id"), JO.getLong("updatetime"));
                fcsServer.put(remote_id, fc);
            }
        } catch (JSONException e) {
            e.printStackTrace();
        }

        return fcsServer;
    }

    private void finishSync() {

        printLine("Number of Flashcards in Db: " + new Select().from(Flashcard.class).count());

        syncOver = true;
        this.setFinishOnTouchOutside(true);
        setLastSyncSuccessful(syncSuccessful);
        if (syncSuccessful){
            printLine("Syncing finished, Everything is up to date!");
        }else{
            printLine("Something went wrong in sync, please try again later");
        };
    }

    private void cancelSync() {

        syncOver = true;
        this.setFinishOnTouchOutside(true);
        printLine("Sync cancelled. Please perform a sync again soon.");
    }

    private void deleteToDeleteHere() {

        printLine("Deleting on App");

        for (Flashcard fc : toDeleteHere){

            fc.delete();
        }

        nextRequest();
    }

    private ArrayList<Integer> getIds(List<Flashcard> fcsApp) {
        ArrayList<Integer> ids = new ArrayList<>();
        for (Flashcard fc : fcsApp)     ids.add(fc.remote_id);
        return ids;
    }

    @Override
    public void onBackPressed() {

        printLine(Boolean.toString(syncOver));
        if (syncOver)
            finish();
    }

    public static void printLine(String str){

        String output = syncTextView.getText().toString();

        if (output.equals("No Output")){
            output = "";
        }

        syncTextView.setText(output+str+"\n");
    }

    public void nextRequest(){

        requestCount++;

        if (requestCount==1){

            fillUpCollections();
            return;

        }else if (requestCount==2){

            if (toInsertHere.size()>0 || toDeleteOnServer.size()>0 || toInsertOnServer.size()>0 )
                insertDeleteSelectRequest();
            else
                nextRequest();
            return;

        }else if (requestCount==3){

            if (toDeleteHere.size()>0)
                deleteToDeleteHere();
            else
                nextRequest();
            return;
        }else if (requestCount==4){

            finishSync();
            return;
        }

    }

//    public void nextRequest(){
//
//        requestCount++;
//
//        if (requestCount==1){
//
//            fillUpCollections();
//            return;
//
//        }else if (requestCount==2){
//
//            if (toDeleteHere.size()>0)
//                deleteToDeleteHere();
//            else
//                nextRequest();
//            return;
//
//        }else if (requestCount==3){
//
//            if (toInsertHere.size()>0)
//                insertInApp();
//            else
//                nextRequest();
//            return;
//
//        }else if (requestCount==4){
//
//            if (toInsertOnServer.size()>0)
//                insertToServer();
//            else
//                nextRequest();
//            return;
//
//        }else if (requestCount==5){
//
//            if (toDeleteOnServer.size()>0)
//                deleteOnServer();
//            else
//                nextRequest();
//            return;
//        }else if (requestCount==6){
//
//            finishSync();
//            return;
//        }
//
//    }
}
