package com.mosarvit.laguna;

import android.content.Context;
import android.media.MediaPlayer;
import android.net.Uri;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.Display;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.WindowManager;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;
import android.widget.VideoView;
import android.widget.MediaController;


import com.activeandroid.query.Select;

import java.util.List;

public class QuizActivity extends AppCompatActivity {

//    private static TextView questionTextView, txtTest;
    private static WebView webView;
    private static android.widget.VideoView videoView;
    private static Button btnTest, btnShortest, btnMiddle, btnLongest, btnShowAnswer;
    private static MediaController mediaController;
    List<Flashcard> allFcs;
    Flashcard currentFc;
    private String videoPath;
    private int stage = 0;




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
        btnShowAnswer = (Button)findViewById(R.id.btnShowAnswer);

        mediaController = new MediaController(this);

        videoView = (VideoView) findViewById(R.id.videoView);

        mediaController = new MediaController(QuizActivity.this);;
        mediaController.setPadding(0, 0, 0, 90);
        videoView.setMediaController(mediaController);
                                          /*
                                           * and set its position on screen
                                           */
        mediaController.setAnchorView(videoView);

        videoView.setOnPreparedListener(new MediaPlayer.OnPreparedListener() {
            @Override
            public void onPrepared(MediaPlayer mp) {
                mp.setOnVideoSizeChangedListener(new MediaPlayer.OnVideoSizeChangedListener() {
                    @Override
                    public void onVideoSizeChanged(MediaPlayer mp, int width, int height) {
                                          /*
                                           *  add media controller
                                           */
                        mediaController = new MediaController(QuizActivity.this);;
                        videoView.setMediaController(mediaController);
                        mediaController.setPadding(0, 0, 0, 90);
                                          /*
                                           * and set its position on screen
                                           */
                        mediaController.setAnchorView(videoView);
                    }
                });
            }
        });
//        mediaController.setAnchorView(videoView);

        webView = (WebView) findViewById(R.id.wbvQuestion);
        webView.getSettings().setJavaScriptEnabled(true);
        webView.setWebViewClient(new WebViewClient());
        webView.getSettings().setDefaultTextEncodingName("utf-8");


        allFcs = new Select().from(Flashcard.class).execute();

        if (allFcs.size() == 0){
            printLineToQuestionTextView("ERROR001 : No Flashcards in the Database, please sync first");
            return;
        }

        nextCard();

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

        btnShowAnswer.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View v) {

              showAnswer();
            }
        });
    }

    private void increaseDueTimeAndRefresh(long millis) {

        long now = System.currentTimeMillis();
        currentFc.setDuetime(now + millis);
        currentFc.save();

        nextCard();
    }

    private void nextCard() {

        stage=0;
        refreshButtons();
        refreshCurrentFc();
        refreshMediaFile();
        refreshQuestion();
    }

    private void showAnswer() {

        stage = 1;
        refreshButtons();
        refreshQuestion();
    }

    private void refreshButtons() {

        if(stage==0){

            btnShowAnswer.setVisibility(View.VISIBLE);
            btnShortest.setVisibility(View.GONE);
            btnMiddle.setVisibility(View.GONE);
            btnLongest.setVisibility(View.GONE);
        }else if(stage==1){

            btnShowAnswer.setVisibility(View.GONE);
            btnShortest.setVisibility(View.VISIBLE);
            btnMiddle.setVisibility(View.VISIBLE);
            btnLongest.setVisibility(View.VISIBLE);
        }
    }

    private void refreshMediaFile() {

        if (currentFc.mfsremoteid != 0){
            MediaFileSegment mfs = new Select().from(MediaFileSegment.class).where("remote_id = " + currentFc.mfsremoteid).executeSingle();
            videoPath = SharedClass.LOCAL_MEDIA_FOLDER + "/" + mfs.mediaFileName + "/" + mfs.fileName;
            Uri uri = Uri.parse(videoPath);
            videoView.setVideoURI(uri);
            videoView.start();
        }

    }

    private void refreshQuestion() {

        String cssHTMLtop = "";

        if(stage==0){
            cssHTMLtop = "<!DOCTYPE html><html><head><style> " +
                    "body " +
                    "{background-color: white; word-wrap: break-word;}" +
                    "#maindiv{display: inline;  } " +
                    "#toLearnWord{color: blue;}" +
                    "#chinese{line-height: 120%;  font-size: 160%;}" +
                    "#translit{font-size: 110%; line-height: 100%; visibility: hidden;}" +
                    "#english{font-size: 110%; line-height: 100%;  visibility: hidden;margin-bottom: 12px; }" +
                    "</style><body><div id=maindiv>";

        }else if(stage==1){
            cssHTMLtop = "<!DOCTYPE html><html><head><style> " +
                    "body " +
                    "{background-color: white; word-wrap: break-word;}" +
                    "#maindiv{display: inline;  } " +
                    "#toLearnWord{color: blue;}" +
                    "#chinese{line-height: 120%;  font-size: 160%;}" +
                    "#translit{font-size: 110%; line-height: 100%; visibility: visible;}" +
                    "#english{font-size: 110%; line-height: 100%;  visibility: visible;margin-bottom: 12px; }" +
                    "</style><body><div id=maindiv>";
        }


        String cssHTMLbottom = "</div ></body>\n" +
                "</html>";

        String questionString = cssHTMLtop + currentFc.getQuestion();

        webView.loadDataWithBaseURL (null, questionString, "text/html", "UTF-8", null);

    }

    private void refreshCurrentFc() {

        allFcs = new Select().from(Flashcard.class).where("ready = 1").orderBy("DueTime").execute();
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
