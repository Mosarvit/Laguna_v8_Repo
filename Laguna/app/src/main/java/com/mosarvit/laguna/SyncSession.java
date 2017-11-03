package com.mosarvit.laguna;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.text.method.ScrollingMovementMethod;
import android.util.DisplayMetrics;
import android.widget.TextView;


public class SyncSession extends Activity {


    public Synchronizer<MediaFileSegment> sM;
    public Synchronizer<Flashcard> sF;

    public static TextView syncTextView;

    public boolean syncOver = false;


    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);


        setContentView(R.layout.sync_seesion_pop_up_layout);

        DisplayMetrics dm = new DisplayMetrics();
        getWindowManager().getDefaultDisplay().getMetrics(dm);

        int width = dm.widthPixels;
        int height = dm.heightPixels;

        getWindow().setLayout((int) (width * .8), (int) (height * .6));

        syncTextView = findViewById(R.id.txtSyncMessage);
        syncTextView.setMovementMethod(new ScrollingMovementMethod());

        printLine("Starting the sync process");


        sM = new Synchronizer(this, MediaFileSegment.class);

        sM.synchronize();

//        nextTask();
    }

    @Override
    public void onBackPressed() {

        printLine(Boolean.toString(syncOver));
        if (syncOver)
            finish();
    }

    public void makeADecisionsAboutTheContradictions() {
        AlertDialog.Builder alert_builder = new AlertDialog.Builder(SyncSession.this);
        alert_builder.setMessage("There were some conradictions, how should we act in contradictory cases?").setCancelable(true)
                .setNegativeButton(
                        "Download from Server", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {
                                sM.serverWins();

                            }
                        }).setNeutralButton("Upload to Server", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {

                sM.hereWins();
            }
        }).setPositiveButton("Cancel", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {

                sM.cancel();
            }
        }).setOnCancelListener(new DialogInterface.OnCancelListener() {
            @Override
            public void onCancel(DialogInterface dialog) {

                sM.cancel();
            }
        });

        AlertDialog alert = alert_builder.create();
        alert.setTitle("Alert!");
        alert.show();
    }

    public void printLine(String str) {

        String output = syncTextView.getText().toString();

        if (output.equals("No Output")) {
            output = "";
        }

        syncTextView.setText(output + str + "\n");
    }

//    public void nextTask(){
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
//                nextTask();
//            return;
//
//        }else if (requestCount==3){
//
//            if (toInsertHere.size()>0)
//                insertInApp();
//            else
//                nextTask();
//            return;
//
//        }else if (requestCount==4){
//
//            if (toInsertOnServer.size()>0)
//                insertToServer();
//            else
//                nextTask();
//            return;
//
//        }else if (requestCount==5){
//
//            if (toDeleteOnServer.size()>0)
//                deleteOnServer();
//            else
//                nextTask();
//            return;
//        }else if (requestCount==6){
//
//            finishSync();
//            return;
//        }
//
//    }
}
