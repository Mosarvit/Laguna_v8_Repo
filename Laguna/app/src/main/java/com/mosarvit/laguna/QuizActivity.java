package com.mosarvit.laguna;

import android.content.Context;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.Display;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.WindowManager;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;
import android.widget.VideoView;

import com.activeandroid.query.Select;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

public class QuizActivity extends AppCompatActivity {

//    private static TextView questionTextView, txtTest;
    private static WebView webView;
    private static android.widget.VideoView videoView;
    private static Button btnTest, btnShortest, btnMiddle, btnLongest;
    List<Flashcard> allFcs;
    Flashcard currentFc;
    private String videoPath;




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

//        txtTest = (TextView) findViewById(R.id.txtTest);


        btnTest = (Button)findViewById(R.id.btnTest);
        btnShortest = (Button)findViewById(R.id.btnShortest);
        btnMiddle = (Button)findViewById(R.id.btnMiddle);
        btnLongest = (Button)findViewById(R.id.btnLongest);
        webView = (WebView) findViewById(R.id.wbvQuestion);
        videoView = (VideoView) findViewById(R.id.videoView);

        webView.getSettings().setJavaScriptEnabled(true);
        webView.setWebViewClient(new WebViewClient());
        webView.getSettings().setDefaultTextEncodingName("utf-8");
//        webView.setOnTouchListener(new View.OnTouchListener() {
//
//
//            public boolean onTouch(View v, MotionEvent event) {
//                return (event.getAction() == MotionEvent.ACTION_MOVE);
//            }
//        });

//        webView.getSettings().setUseWideViewPort(true);
//        webView.getSettings().setLoadWithOverviewMode(true);

//        webView.setHorizontalScrollBarEnabled(false);
//        webView.setVerticalScrollBarEnabled(true);
//        webView.getSettings().setLoadWithOverviewMode(true);
//        webView.getSettings().setUseWideViewPort(true);

//        webView.setInitialScale(1);
//        webView.getSettings().setJavaScriptEnabled(true);
//        webView.getSettings().setLoadWithOverviewMode(true);
//        webView.getSettings().setUseWideViewPort(true);
//        webView.setScrollBarStyle(WebView.SCROLLBARS_OUTSIDE_OVERLAY);
//        webView.setScrollbarFadingEnabled(false);

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
        refreshMediaFile();
        refreshQuestion();
    }

    private void refreshMediaFile() {

        if (currentFc.mfsremoteid != 0){
            MediaFileSegment mfs = new Select().from(MediaFileSegment.class).where("remote_id = " + currentFc.mfsremoteid).executeSingle();
            videoPath = SharedData.LOCAL_MEDIA_FOLDER + "/" + mfs.mediaFileName + "/" + mfs.fileName;
            Uri uri = Uri.parse(videoPath);
            videoView.setVideoURI(uri);
            videoView.start();
        }

    }

    private void refreshQuestion() {

        String cssHTMLtop = "<!DOCTYPE html>\n" +
                "<html>\n" +
                "<head>\n" +
                "<style>\n" +
                "body {" +
                "background-color: white;" +
                "word-wrap:break-word;" +
                "" +
                "}" +
                "#maindiv{" +
                "" +
                "}" +
                "h1   {color: blue;}\n" +
                "p    {color: red;}\n" +
                "</style>\n" +
                "</head>" +
                "" +
                "<body>" +
                "<div id=maindiv>" +
                "";

        String cssHTMLbottom = "</div ></body>\n" +
                "</html>";

        String questionString = cssHTMLtop + currentFc.getQuestion() + cssHTMLbottom;

        webView.loadDataWithBaseURL (null, questionString  + videoPath, "text/html", "UTF-8", null);
//
//        String str = "android.resource://com.android.AndroidVideoPlayer/" + R.layout.activity_main ;
//
//        StringBuilder sb = new StringBuilder();
//
//        File parentDir = new File(str);
//        ArrayList<File> inFiles = new ArrayList<File>();
//        File[] files = parentDir.listFiles();
//        for (File file : files) {
//
//            sb.append(file.getName());
//        }
//
//
//        webView.loadDataWithBaseURL (null, sb.toString(), "text/html", "UTF-8", null);

//        String path = Environment.getRootDirectory().toString();
//        File f = new File(path);
//        for (File file : f.listFiles()){
//            sb.append(file.getName()+ "</br>" );
//        }
//        webView.loadDataWithBaseURL (null, sb.toString(), "text/html", "UTF-8", null);
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

        webView.loadData(str, "text/html; charset=utf-8", "UTF-8");

//        questionTextView.setText(output+str+"\n");
    }

    private static void printToQuestionTextView(String str){

//        questionTextView.setText(str);
        webView.loadData(str, "text/html; charset=utf-8", "UTF-8");
    }


    private int getScale(){
        Display display = ((WindowManager) getSystemService(Context.WINDOW_SERVICE)).getDefaultDisplay();
        int width = display.getWidth();
        Double val = new Double(width) ;
//        val = val * 100d;
        return val.intValue();
    }
}
