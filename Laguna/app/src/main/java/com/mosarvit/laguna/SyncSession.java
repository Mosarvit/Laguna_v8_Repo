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
import java.util.HashMap;
import java.util.List;
import java.util.Map;


public class SyncSession extends Activity {

    private RequestQueue requestQueue;
    private ArrayList<Flashcard> toInsertInApp = new ArrayList<>();
    private ArrayList<Flashcard> toInsertInServer = new ArrayList<>();
    private ArrayList<Flashcard> toDeleteOnServer = new ArrayList<>();

    private int requestCount = 0;

    public static TextView syncTextView;
    public static List<Flashcard> fcsAppBeforeSync;
    HashMap<Integer, Flashcard> fcsServerHashMap;
    HashMap<Integer, Flashcard> fcsAppHashMap;

    ArrayList<Flashcard> toDeleteInApp = new ArrayList<>();
    ArrayList<Flashcard> serverHasAppDoesnt = new ArrayList<>();

    ArrayList<Flashcard> contradictingAppVersion = new ArrayList<>();
    ArrayList<Flashcard> contradictingServerVersion = new ArrayList<>();

    ArrayList<Flashcard> moreRecentAppVersion = new ArrayList<>();
    ArrayList<Flashcard> moreRecentServerVersion = new ArrayList<>();

    private boolean syncSuccessful = true;
    private boolean syncFinished;

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

        printLineToSyncTextView("Starting the sync process");

        syncFinished = false;

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
        printLineToSyncTextView("Requesting " + toInsertInApp.size() + " Flashcards");

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/get_flashcards_by_id.php", new Response.Listener<String>()
        {
            @Override
            public void onResponse(String response) {

//                printLineToSyncTextView("\n insertInApp(), response: \n\n");

                JSONArray JA = null;
                try {
                    JA = new JSONArray(response);
                    printLineToSyncTextView("Saving or updating " + JA.length() + " Flashcards");
                    if (JA.length()!=toInsertInApp.size()){
                        resposeUnequalOne("To little Flashcards arrived");
                    }

                    for(int i = 0; i<JA.length(); i++) {

                        JSONObject JO = (JSONObject) JA.get(i);

                        int id = JO.getInt("id");
                        String question = JO.getString("question");
                        long duetime = JO.getLong("duetime");
                        long updatetime = JO.getLong("updatetime");

                        if (fcsAppHashMap.keySet().contains(id)){


                            new Update(Flashcard.class).set(
                                    "question = '" + question +
                                    "', duetime = " + duetime +
                                    ", updatetimelocal = " + updatetime +
                                    ", updatetimewhenloaded = " + updatetime +
                                    ", status = 0 ")
                                    .where("remote_id = " + id).execute();
                        } else{
                            Flashcard fc = new Flashcard(id, question, duetime, updatetime);
                            fc.save();
                        }

                    }

                } catch (JSONException e) {
                    e.printStackTrace();
                }

                printLineToSyncTextView("Number of Flashcards in db: " + new Select().from(Flashcard.class).count());

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

                for (Flashcard fc : toInsertInApp){
                    parameters.put("ids[" + i++ + "]",Integer.toString(fc.id));
                }

                return parameters;
            }
        };

        requestQueue.add(request);

    }

    private void onVolleyError(VolleyError volleyError) {
        printLineToSyncTextView(volleyError.toString());

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

    private void deleteToDeleteFcs() {

        printLineToSyncTextView("Deleting " + toDeleteOnServer.size() + " Flashcards from server");

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/delete_flashcards_by_id.php", new Response.Listener<String>()
        {
            @Override
            public void onResponse(String response) {

                if (response.equals("1")){
                    printLineToSyncTextView("Successfully deleted " + toDeleteOnServer.size() + " Flashcards from server");
                }else{
                    resposeUnequalOne(response);
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
                    parameters.put("ids[" + i++ + "]",Integer.toString(fc.id));
                }

                return parameters;
            }
        };

        requestQueue.add(request);

    }

    private void resposeUnequalOne(String response) {
        printLineToSyncTextView(response);
        syncSuccessful = false;
    }

    private void insertToServer() {

        printLineToSyncTextView("Inserting " + toInsertInServer.size() + " Flashcards to server");

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/insert_or_update_flashcards_by_id.php", new Response.Listener<String>()
        {
            @Override
            public void onResponse(String response) {

                if (response.equals("1")){

                    for (Flashcard fc : toInsertInServer){
                        fc.updatetimewhenloaded = fc.updatetimelocal;
                        fc.save();
                    }

                }else{
                    resposeUnequalOne(response);
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

                for (Flashcard fc : toInsertInServer){

                    parameters.put("ids[" + i + "]",Integer.toString(fc.id));
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


    private void fillUpToRequestFcs() {

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/get_to_check_for_sync.php", new Response.Listener<String>()
        {
            @Override
            public void onResponse(String response) {

                final ArrayList<Flashcard> fcsServer = new ArrayList<Flashcard>();

                JSONArray JA = null;
                try {
                    JA = new JSONArray(response);
                    for(int i = 0; i<JA.length(); i++) {

                        JSONObject JO = (JSONObject) JA.get(i);
                        Flashcard fc = new Flashcard(JO.getInt("id"), JO.getLong("updatetime"));
                        fcsServer.add(fc);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }

                ArrayList<Flashcard> newFcs = new ArrayList<>();

                // find the new entries

                fcsAppBeforeSync = new Select().from(Flashcard.class).execute();
                
                fcsServerHashMap = new HashMap<>();
                fcsAppHashMap = new HashMap<>();
                
                for (Flashcard fc : fcsServer){

                    fcsServerHashMap.put(fc.id, fc);
                }

                for (Flashcard fc : fcsAppBeforeSync){

                    fcsAppHashMap.put(fc.id, fc);
                }

                toInsertInApp = new ArrayList<Flashcard>(newFcs);
                
                for (Flashcard fcApp : fcsAppHashMap.values()){
                    
                    Flashcard fcServer; 
                    
                    if (!fcsServerHashMap.keySet().contains(fcApp.id)){
                       toDeleteInApp.add(fcApp);
                    } else {

                        fcServer = fcsServerHashMap.get(fcApp.id);

                        if (fcApp.getUpdateTimeLocal() > fcApp.getUpdatetimeWhenLoaded()
                                &&  fcServer.getUpdatetimeWhenLoaded() > fcApp.getUpdatetimeWhenLoaded()) {

                            contradictingAppVersion.add(fcApp);
                            contradictingServerVersion.add(fcServer);

                        } else if (  fcApp.getUpdateTimeLocal() > fcApp.getUpdatetimeWhenLoaded()) {

                            moreRecentAppVersion.add(fcApp);

                        } else if (  fcServer.getUpdatetimeWhenLoaded() > fcApp.getUpdatetimeWhenLoaded()) {

                            moreRecentServerVersion.add(fcServer);
                        }
                    }
                }

                for (Flashcard fcServer : fcsServer){

                    if (!fcsAppHashMap.keySet().contains(fcServer.id)) {
                        serverHasAppDoesnt.add(fcServer);
                    }
                }
                
//                printLineToSyncTextView("toDeleteInApp.size(): " + toDeleteInApp.size() +
//                                        "\nserverHasAppDoesnt.size(): " + serverHasAppDoesnt.size() +
//                                        "\ncontradictingAppVersion: " + contradictingAppVersion.size() +
//                                        "\ncontradictingServerVersion: " + contradictingServerVersion.size() +
//                                        "\nmoreRecentAppVersion: " + moreRecentAppVersion.size() +
//                                        "\nmoreRecentServerVersion: " + moreRecentServerVersion.size());


                if (contradictingAppVersion.size()>0){
                    AlertDialog.Builder alert_builder = new AlertDialog.Builder(SyncSession.this);
                    alert_builder.setMessage("There were some conradictions, how should we act in contradictory cases?").setCancelable(true)
                            .setNegativeButton(
                            "Download from Server", new DialogInterface.OnClickListener(){
                            @Override
                            public void onClick(DialogInterface dialog, int which) {

                                toInsertInApp.addAll(contradictingServerVersion);
                                continueFillingUp();
                            }
                        }).setNeutralButton("Upload to Server", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {

                                toInsertInServer.addAll(contradictingAppVersion);
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
                }else {
                    continueFillingUp();
                }
//                else {
//
//                    finishSync();
//                }

            }

            private void continueFillingUp() {
                toInsertInServer.addAll(moreRecentAppVersion);

                for (Flashcard fc : toInsertInServer){

                    if (fc.getStatus() == 9){
                        toDeleteOnServer.add(fc);
                    }
                }
                toInsertInApp.addAll(moreRecentServerVersion);
                toInsertInApp.addAll(serverHasAppDoesnt);

                printLineToSyncTextView("\ntoDeleteOnServer: " + toDeleteOnServer.size() +
                                        "\ntoDeleteInApp: " + toDeleteInApp.size() +
                                        "\ntoInsertInServer: " + toInsertInServer.size() +
                                        "\ntoInsertInApp: " + toInsertInApp.size());

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

    private void finishSync() {

        syncFinished = true;
        this.setFinishOnTouchOutside(true);
        setLastSyncSuccessful(syncSuccessful);
        if (syncSuccessful){
            printLineToSyncTextView("Syncing finished, Everything is up to date!");
        }else{
            printLineToSyncTextView("Something went wrong in sync, please try again later");
        };
    }

    private void cancelSync() {
        printLineToSyncTextView("Sync cancelled. Please perform a sync again soon.");
    }

    private void deleteToDeleteInApp() {
        for (Flashcard fc : toDeleteInApp){

            fc.delete();
        }

        nextRequest();
    }

    private ArrayList<Integer> getIds(List<Flashcard> fcsApp) {
        ArrayList<Integer> ids = new ArrayList<>();
        for (Flashcard fc : fcsApp)     ids.add(fc.id);
        return ids;
    }

    @Override
    public void onBackPressed() {

        printLineToSyncTextView(Boolean.toString(syncFinished));
        if (syncFinished)
            finish();
    }

    public static void printLineToSyncTextView(String str){

        String output = syncTextView.getText().toString();

        if (output.equals("No Output")){
            output = "";
        }

        syncTextView.setText(output+str+"\n");
    }

    public void nextRequest(){

        requestCount++;

        if (requestCount==1){

            fillUpToRequestFcs();
            return;

        }else if (requestCount==2){

            if (toDeleteInApp.size()>0)
                deleteToDeleteInApp();
            else
                nextRequest();
            return;

        }else if (requestCount==3){

            if (toInsertInApp.size()>0)
                insertInApp();
            else
                nextRequest();
            return;

        }else if (requestCount==4){

            if (toInsertInServer.size()>0)
                insertToServer();
            else
                nextRequest();
            return;

        }else if (requestCount==5){

            if (toDeleteOnServer.size()>0)
                deleteToDeleteFcs();
            else
                nextRequest();
            return;
        }else if (requestCount==6){

            finishSync();
            return;
        }

    }
}
