package com.mosarvit.laguna;

import android.content.Context;
import android.content.SharedPreferences;

import java.io.File;

/**
 * Created by Mosarvit on 11/5/2017.
 */

public class SharedClass {

    public static String FTP_USERNAME;
    public static String FTP_PASSWORD;
    public static String FTP_MEDIA_FOLDER;
    public static String HOST_NAME = "ricky.heliohost.org";
    public static int FTP_PORT_NUMBER;
    public static String ROOT_FOLDER;
    public static String LOCAL_MEDIA_FOLDER =  ROOT_FOLDER+"/mediafiles";
    public static int CLIENT_ID = 2;
    public static int MAX_RETRIES = 5;

    public static int MAX_RETRIES_ON_TIMEOUT = 20;


    public static void setLastSyncSuccessful(boolean synci, ViewActivity view) {
        SharedPreferences sharedPreferecnes = view.getSharedPreferences("SyncStatus", Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPreferecnes.edit();
        editor.putBoolean("lastSyncSuccessful", synci);
        editor.apply();

    }

}
