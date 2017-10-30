package com.mosarvit.laguna;

import com.activeandroid.Model;
import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;
import com.activeandroid.query.Select;

import java.util.HashMap;
import java.util.List;

@Table(name = "Flashcards")
public class Flashcard extends Model {

    @Column(name = "remote_id", unique = true, onUniqueConflict = Column.ConflictAction.REPLACE)
    public int remote_id;

    @Column(name = "question")
    private String question;

    @Column(name = "duetime")
    private long duetime;

    @Column(name = "utlocal")
    public long utlocal;

    @Column(name = "utwhenloaded")
    public long utwhenloaded;

    @Column(name = "newfc")
    public boolean newfc;

    @Column(name = "toDelete")
    public boolean toDelete;

    public void setDuetime(long duetime){

        this.utlocal = System.currentTimeMillis();
        this.duetime = duetime;
    }

    public void setQuestion(String question){

        this.utlocal = System.currentTimeMillis();
        this.question = question;
    }

    public String getQuestion(){

        return this.question;
    }

    public long getDuetime(){

        return this.duetime;
    }

    public long getUpdateTimeLocal(){

        return this.utlocal;
    }

    public long getUpdatetimeWhenLoaded(){

        return this.utwhenloaded;
    }



    public void setToDeleteOnServer(){

        this.utlocal = System.currentTimeMillis();
    }

    public Flashcard(){
        super();
    }


    public Flashcard(int remote_id, String question, long duetime, long updatetime, boolean newfc){
        super();
        this.duetime = duetime;
        this.utwhenloaded = updatetime;
        this.utlocal = updatetime;
        this.remote_id = remote_id;
        this.question = question;
        this.newfc = newfc;
    }

    public Flashcard(int remote_id, String question, long duetime, long updatetime){
        super();
        this.duetime = duetime;
        this.utwhenloaded = updatetime;
        this.utlocal = updatetime;
        this.remote_id = remote_id;
        this.question = question;
    }

    public Flashcard(int remote_id, long updatetime){
        super();
        this.utwhenloaded = updatetime;
        this.utlocal = updatetime;
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
                "\nutlocal: " + this.utlocal +
                "\nduetime: " + this.duetime +
                "\n\n";
    }

    public static Flashcard getByRemoteId(int id){

        return new Select().from(Flashcard.class).where("remote_id = " + id).executeSingle();
    }

    public static List<Flashcard> getAll(){

        return new Select().from(Flashcard.class).execute();
    }

    public static HashMap<Integer, Flashcard> getAllAsHM(){

        HashMap<Integer, Flashcard> hm = new HashMap<Integer, Flashcard>();

        for (Flashcard fc : getAll()){
            hm.put(fc.remote_id, fc);
        }

        return hm;
    }
}

