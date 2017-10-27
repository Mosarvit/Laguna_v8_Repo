package com.mosarvit.laguna;

import android.os.AsyncTask;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

/**
 * Created by Mosarvit on 10/27/2017.
 */

public class syncDb extends AsyncTask<Void, Void, Void> {

    String data = "";
    String dataParsed = "";
    String str = "";

    @Override
    protected Void doInBackground(Void... params) {

        String line = "";
        String singleParsed = "";
        StringBuilder sb = new StringBuilder();

        try {

            URL url = new URL("http://mosar.heliohost.org/get_updated_after.php");
            HttpURLConnection httpURLConnection = (HttpURLConnection) url.openConnection();
            InputStream inputStream = httpURLConnection.getInputStream();
            BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(inputStream));

            while(line != null){

                line = bufferedReader.readLine();
                sb.append(line);
            }

            JSONArray JA = new JSONArray(sb.toString());
            for(int i = 0; i<JA.length(); i++) {

                JSONObject JO = (JSONObject) JA.get(i);
                Flashcard fc = new Flashcard(JO.getInt("id"), JO.getString("question"), JO.getLong("duetime"), JO.getLong("updatetime"));
//                Flashcard fc = new Flashcard();
                fc.save();
            }

        } catch (IOException e) {
            e.printStackTrace();
        } catch (JSONException e) {
            e.printStackTrace();
        }

        return null;
    }

    protected void onPostExecute(Void aVoid){

        super.onPostExecute(aVoid);
        MainActivity.syncFinished();


    }
}
