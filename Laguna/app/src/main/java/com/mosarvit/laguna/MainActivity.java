package com.mosarvit.laguna;

import android.content.Intent;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.Button;
import android.widget.TextView;

import com.activeandroid.ActiveAndroid;
import com.activeandroid.Configuration;
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
import java.util.Map;

public class MainActivity extends AppCompatActivity {


    private static final int DB_VERSION = 1;
    public static TextView textView;
    public static Button btnStartQuiz, btnSync;

    private   RequestQueue requestQueue;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        Configuration dbConfig =
                new Configuration
                        .Builder(this)
                        .setDatabaseName("dbName")
                        .setDatabaseVersion(DB_VERSION)
                        .create();

        ActiveAndroid.initialize(this);

        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        FloatingActionButton fab = (FloatingActionButton) findViewById(R.id.fab);
        fab.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Snackbar.make(view, "Replace with your own action", Snackbar.LENGTH_LONG)
                        .setAction("Action", null).show();
            }
        });


        textView = (TextView) findViewById(R.id.txt_Output);
        btnStartQuiz = (Button)findViewById(R.id.btnStartQuiz);
        btnSync = (Button)findViewById(R.id.btnSync);

        requestQueue = Volley.newRequestQueue(getApplicationContext());

        btnStartQuiz.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(MainActivity.this, QuizActivity.class));

            }
        });

        btnSync.setOnClickListener(new View.OnClickListener() {



            @Override
            public void onClick(View view) {

                long earliestloadtime = 1509042243137l;

                StringRequest request = new StringRequest(Request.Method.POST, "http://mosar.heliohost.org/get_updated_after.php", new Response.Listener<String>()
                {
                    @Override
                    public void onResponse(String response) {

                        ArrayList<Flashcard> fcsFromServer = new ArrayList<Flashcard>();

                        JSONArray JA = null;
                        try {
                            JA = new JSONArray(response.toString().toString());
                            for(int i = 0; i<JA.length(); i++) {

                                JSONObject JO = (JSONObject) JA.get(i);
                                Flashcard fc = new Flashcard(JO.getInt("id"), JO.getLong("updatetime"));
                                fcsFromServer.add(fc);
                            }
                        } catch (JSONException e) {
                            e.printStackTrace();
                        }

                        for (Flashcard fc : fcsFromServer){

                            printLineToMainTextView(fc.toString());
                        }

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
                        parameters.put("afterthis",Long.toString(1509042243137l));

                        return parameters;
                    }
                };
                requestQueue.add(request);
            }

        });

        // DEBUG start

//        fetchData process = new fetchData();
//        process.execute();
//        startActivity(new Intent(MainActivity.this, QuizActivity.class));

        // DEBUG end
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    public static void printLineToMainTextView(String str){

        String output = (String) textView.getText();

        if (output.equals("No Output ")){
            output = "";
        }

        textView.setText(output+str+"\n");
    }

    public static void syncFinished() {

        MainActivity.printLineToMainTextView(Flashcard.allToString() + "\nDone");
    }
}
