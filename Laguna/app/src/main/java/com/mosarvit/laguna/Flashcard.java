package com.mosarvit.laguna;

import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;
import com.activeandroid.query.Select;

import java.util.HashMap;
import java.util.List;

@Table(name = "Flashcards")
public class Flashcard extends OnServerModel {

    @Column(name = "question")
    private String question;

    @Column(name = "duetime")
    private long duetime;

    public void setDuetime(long duetime){

        this.updatetime = System.currentTimeMillis();
        this.duetime = duetime;
    }

    public void setQuestion(String question){

        this.updatetime = System.currentTimeMillis();
        this.question = question;
    }

    public String getQuestion(){

        return this.question;
    }

    public long getDuetime(){

        return this.duetime;
    }

    public long getUpdateTimeLocal(){

        return this.updatetime;
    }

    public long getUpdatetimeWhenLoaded(){

        return this.utwhenloaded;
    }



    public void setToDeleteOnServer(){

        this.updatetime = System.currentTimeMillis();
    }

    public Flashcard(){
        super();
    }


    public Flashcard(int remote_id, String question, long duetime, long updatetime, boolean newfc){
        super();
        this.duetime = duetime;
        this.utwhenloaded = updatetime;
        this.updatetime = updatetime;
        this.remote_id = remote_id;
        this.question = question;
        this.isNew = newfc;
    }

    public Flashcard(int remote_id, String question, long duetime, long updatetime){
        super();
        this.duetime = duetime;
        this.utwhenloaded = updatetime;
        this.updatetime = updatetime;
        this.remote_id = remote_id;
        this.question = question;
    }

    public Flashcard(int remote_id, long updatetime){
        super();
        this.utwhenloaded = updatetime;
        this.updatetime = updatetime;
        this.remote_id = remote_id;
    }

    public Flashcard(int remote_id){
        super();
        this.remote_id = remote_id;
    }

    public static String allToString(){

        String str = "";

        List<Flashcard> allFcs = new Select().from(Flashcard.class).execute();

        for (Flashcard fc : allFcs){

            str +=  fc.toString();
        }

        return str;
    }

    @Override
    public String toString(){

        return "Id: " + this.remote_id +
                "\nquestion: " + this.question +
                "\nupdatetime: " + this.updatetime +
                "\nduetime: " + this.duetime +
                "\n\n";
    }

    public static Flashcard getByRemoteId(int id){

        return new Select().from(Flashcard.class).where("remote_id = " + id).executeSingle();
    }

    public static List<Flashcard> getAll(){

        return new Select().from(Flashcard.class).execute();
    }

    public static HashMap<Integer, OnServerModel> getAllAsHM(){

        HashMap<Integer, OnServerModel> hm = new HashMap<Integer, OnServerModel>();

        for (Flashcard fc : getAll()){
            hm.put(fc.remote_id, fc);
        }

        return hm;
    }
}

