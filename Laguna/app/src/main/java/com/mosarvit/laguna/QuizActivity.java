package com.mosarvit.laguna;

import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;
import android.widget.TextView;

import com.activeandroid.query.Select;

import java.util.List;

public class QuizActivity extends AppCompatActivity {

    private static TextView questionTextView;
    private static WebView webView;
    private static Button btnTest, btnShortest, btnMiddle, btnLongest;
    List<Flashcard> allFcs;
    Flashcard currentFc;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_quiz);
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

//        questionTextView = (TextView) findViewById(R.id.txt_Question);
        btnTest = (Button)findViewById(R.id.btnTest);
        btnShortest = (Button)findViewById(R.id.btnShortest);
        btnMiddle = (Button)findViewById(R.id.btnMiddle);
        btnLongest = (Button)findViewById(R.id.btnLongest);
        webView = (WebView) findViewById(R.id.wbvQuestion);

        webView.getSettings().setJavaScriptEnabled(true);
        webView.setWebViewClient(new WebViewClient());

//        questionTextView.set
//        setContentView(webView);

        allFcs = new Select().from(Flashcard.class).execute();

        if (allFcs.size() == 0){
            printLineToQuestionTextView("ERROR001 : No Flashcards in the Database, please sync first");
            return;
        }

        refreshSession();

        btnTest.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                for (Flashcard fc : allFcs){

                    printLineToQuestionTextView(Flashcard.allToString());
                }
            }
        });

        btnShortest.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View v) {

                increaseDueTimeAndRefresh(300000l);
            }
        });

        btnMiddle.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View v) {

                increaseDueTimeAndRefresh(3600000l);
            }
        });

        btnLongest.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View v) {

                increaseDueTimeAndRefresh(8640000000l);
            }
        });
    }

    private void increaseDueTimeAndRefresh(long millis) {

        long now = System.currentTimeMillis();
        currentFc.setDuetime(now + 300000l);
        currentFc.save();

        refreshSession();
    }

    private void refreshSession() {

        refreshCurrentFc();
        refreshQuestion();
    }

    private void refreshQuestion() {

        webView.loadData(currentFc.getQuestion(), "text/html", "UTF-8");
    }

    private void refreshCurrentFc() {

        allFcs = new Select().from(Flashcard.class).orderBy("DueTime").execute();
        currentFc = allFcs.get(0);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_quiz, menu);
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

    private static void printLineToQuestionTextView(String str){

//        String output = (String) questionTextView.getText();

//        if (output.equals("No Output ")){
//            output = "";
//        }

        webView.loadData(str, "text/html", "UTF-8");

//        questionTextView.setText(output+str+"\n");
    }

    private static void printToQuestionTextView(String str){

//        questionTextView.setText(str);
        webView.loadData(str, "text/html", "UTF-8");
    }
}
