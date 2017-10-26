package com.mosarvit.laguna;

import android.os.AsyncTask;

import com.activeandroid.query.Select;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.List;

//import com.activeandroid.query.Select;


public class fetchData extends AsyncTask<Void, Void, Void> {

    String data = "";
    String dataParsed = "";
    String str = "";

    @Override
    protected Void doInBackground(Void... params) {

        String line = "";
        String singleParsed = "";
        StringBuilder sb = new StringBuilder();

        try {

            URL url = new URL("http://mosar.heliohost.org/get_data.php");
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
//                Flashcard fc = new Flashcard(JO.getInt("id"), JO.getString("question"));
                Flashcard fc = new Flashcard();
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

        List<Flashcard> allFcs = new Select().from(Flashcard.class).execute();

        for (Flashcard fc : allFcs){

            str += "id: " + fc.id + "\nquestion: " + fc.question + "\n\n";
        }

        MainActivity.printLineToMainTextView(str);
        MainActivity.printLineToMainTextView("\nDone");
    }
}