package com.mosarvit.laguna;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.text.method.ScrollingMovementMethod;
import android.util.DisplayMetrics;
import android.widget.TextView;


public class SyncSession extends  ViewActivity {


    public Synchronizer<MediaFileSegment> sM;
    public Synchronizer<Flashcard> sF;

    public static TextView syncTextView;

    public boolean syncOver = false;


    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        SharedPreferences prefs = getSharedPreferences("FTP_PREFS", MODE_PRIVATE);





        setContentView(R.layout.sync_seesion_pop_up_layout);

        DisplayMetrics dm = new DisplayMetrics();
        getWindowManager().getDefaultDisplay().getMetrics(dm);

        int width = dm.widthPixels;
        int height = dm.heightPixels;

        getWindow().setLayout((int) (width * .8), (int) (height * .6));

        syncTextView = findViewById(R.id.txtSyncMessage);
        syncTextView.setMovementMethod(new ScrollingMovementMethod());

        printLine("Starting the sync process");


        sF = new Synchronizer<>(this, Flashcard.class);

        sF.synchronize();


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
        alert_builder.setMessage("There were some contradictions, how should we act in contradictory cases?").setCancelable(true)
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

}
