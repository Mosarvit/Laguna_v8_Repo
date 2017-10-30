package com.mosarvit.laguna;

import android.content.Context;

import com.activeandroid.query.Select;
import com.android.volley.AuthFailureError;
import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
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

/**
 * Created by Mosarvit on 10/27/2017.
 */

public class SyncSession1 {

    private RequestQueue requestQueue;
    private ArrayList<Flashcard> toRequestFcs;

    public SyncSession1(Context applicationContext){

        requestQueue = Volley.newRequestQueue(applicationContext);
    };

    public void sync() {

        fillUpToRequestFcs();

    }

    private void getToRequestFcs() {
        MainActivity.printLineToMainTextView("Starting Request");

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/get_flashcards_by_id.php", new Response.Listener<String>()
        {
            @Override
            public void onResponse(String response) {

//                MainActivity.printLineToMainTextView("\n getToRequestFcs(), response: \n\n");

                MainActivity.printLineToMainTextView(response);

                JSONArray JA = null;
                try {
                    JA = new JSONArray(response);
                    for(int i = 0; i<JA.length(); i++) {

                        JSONObject JO = (JSONObject) JA.get(i);
                        Flashcard fc = new Flashcard(JO.getInt("remote_id"), JO.getString("question"), JO.getLong("duetime"), JO.getLong("updatetime"));
                        fc.save();
                    }

                } catch (JSONException e) {
                    e.printStackTrace();
                }

                MainActivity.printLineToMainTextView("Number of Flashcards in db: " + new Select().from(Flashcard.class).count());
                MainActivity.printLineToMainTextView(Flashcard.allToString());
//
//                ArrayList<Flashcard> newFcs = new ArrayList<Flashcard>();
//
//                // find the new entries
//
//                ArrayList<Model> sdf = (ArrayList<Model>) new Select(new String[]{"Id"}).from(Flashcard.class).execute();
//                ArrayList<Integer> ids = new ArrayList<Integer>();
//
//                for (Integer remote_id : ids)
//                    ids.add(remote_id);
//
//                for (Flashcard fc : fcsFromServer){
//
//                    if (!ids.contains(fc.remote_id)){
//
//                        MainActivity.printLineToMainTextView("New Flashcards detected: \n" + fc.toString());
//                        newFcs.add(fc);
//                    }
//                }
//
//                fcsToRequest = new ArrayList<Flashcard>(newFcs);
            }
        },
                new Response.ErrorListener()
                {
                    @Override
                    public void onErrorResponse(VolleyError error)
                    {

                    }
                }) {

                @Override
                protected Map<String, String> getParams() throws AuthFailureError {

                    Map<String,String> parameters  = new HashMap<String, String>();

                    int i=0;

                    for (Flashcard fc : toRequestFcs){
                        parameters.put("ids[" + i++ + "]",Integer.toString(fc.remote_id));
                    }

                    return parameters;
                }
        };

        MainActivity.printLineToMainTextView("sending Request");

        requestQueue.add(request);

        MainActivity.printLineToMainTextView("done sending Request");
    }

    private void fillUpToRequestFcs() {
        MainActivity.printLineToMainTextView("Starting Request");

        StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/get_to_check_for_sync.php", new Response.Listener<String>()
        {
            @Override
            public void onResponse(String response) {

                ArrayList<Flashcard> fcsFromServer = new ArrayList<Flashcard>();

                JSONArray JA = null;
                try {
                    JA = new JSONArray(response);
                    for(int i = 0; i<JA.length(); i++) {

                        JSONObject JO = (JSONObject) JA.get(i);
                        Flashcard fc = new Flashcard(JO.getInt("remote_id"), JO.getLong("updatetime"));
                        fcsFromServer.add(fc);
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }

                ArrayList<Flashcard> newFcs = new ArrayList<Flashcard>();

                // find the new entries

                List<Flashcard> fcs = new Select(new String[]{"remote_id, remote_id"}).from(Flashcard.class).execute();
                ArrayList<Integer> ids = new ArrayList<>();

                for (Flashcard fc : fcs){

                    ids.add(fc.remote_id);
                }

                for (Flashcard fc : fcsFromServer){

                    if (!ids.contains(fc.remote_id)){

                        MainActivity.printLineToMainTextView("New Flashcards detected: \n"  + fc.remote_id);
                        newFcs.add(fc);
                    }
                }

                MainActivity.printLineToMainTextView("newFcs.size(): "  + newFcs.size());

                toRequestFcs = new ArrayList<Flashcard>(newFcs);

                if (toRequestFcs.size()>0)
                    getToRequestFcs();
            }
        },
                new Response.ErrorListener()
                {
                    @Override
                    public void onErrorResponse(VolleyError error)
                    {

                    }
                }) {

//                    @Override
//                    protected Map<String, String> getParams() throws AuthFailureError {
//
//                        Map<String,String> parameters  = new HashMap<String, String>();
//                        parameters.put("remote_id",Integer.toString(16));
//
//                        return parameters;
//                    }
        };

        MainActivity.printLineToMainTextView("sending Request");

        requestQueue.add(request);

        MainActivity.printLineToMainTextView("done sending Request");
    }
}
