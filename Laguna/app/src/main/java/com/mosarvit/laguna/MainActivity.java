package com.mosarvit.laguna;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.text.method.ScrollingMovementMethod;
import android.view.View;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.Button;
import android.widget.TextView;

import com.activeandroid.ActiveAndroid;
import com.activeandroid.Configuration;
import com.activeandroid.query.Select;

public class MainActivity extends AppCompatActivity {


    private static final int DB_VERSION = 1;
    public static TextView textView;
    public static Button btnStartQuiz, btnSync, btnTest;



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
        btnTest = (Button)findViewById(R.id.btnTest);

        textView.setMovementMethod(new ScrollingMovementMethod());



        btnStartQuiz.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(MainActivity.this, QuizActivity.class));

            }
        });

        btnSync.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View view) {

                startActivity(new Intent(MainActivity.this, SyncSession.class));

            }
        });

        btnTest.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View view) {

                Flashcard fc = Flashcard.getAll().get(0);
                fc.setQuestion("asdfasdfasdfsad");
                fc.save();

                fc = Flashcard.getAll().get(1);
                fc.setQuestion("asdfasdfasdfsad");
                fc.save();
            }
        });




        SharedPreferences sharedPreferecnes = getSharedPreferences("SyncStatus", Context.MODE_PRIVATE);

        if (!sharedPreferecnes.getBoolean("lastSyncSuccessful", true)){

            printLineToMainTextView("Some recent sync did not finish properly. Please sync again soon.");
        }

        // DEBUG start

//        fetchData process = new fetchData();
//        process.execute();
//        startActivity(new Intent(MainActivity.this, QuizActivity.class));

//

//        printLineToMainTextView(fc.remote_id + " " + fc.updatetime);

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

        String output = textView.getText().toString();

        if (output.equals("No Output")){
            output = "";
        }

        textView.setText(output+str+"\n");
    }

    public static void syncFinished() {

        MainActivity.printLineToMainTextView(Flashcard.allToString() + "\nDone");
    }
}
