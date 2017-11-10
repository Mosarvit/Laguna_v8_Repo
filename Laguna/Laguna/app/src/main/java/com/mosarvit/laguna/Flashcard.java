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

    @Column(name = "mfsremoteid")
    public int mfsremoteid;

    @Column(name = "ready")
    public boolean ready = true;

//    @Column(name = "mediaFileSegment", onDelete = Column.ForeignKeyAction.CASCADE)
//    public MediaFileSegment mediaFileSegment;




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

    public Flashcard(){}


    public Flashcard(int remote_id, String question, long duetime, long updatetime){
        super();
        this.duetime = duetime;
        this.utwhenloaded = updatetime;
        this.updatetime = updatetime;
        this.remote_id = remote_id;
        this.question = question;
    }

    public Flashcard(int remote_id, String question, long duetime, long updatetime, int mfsremoteid, boolean b){
        super();
        this.duetime = duetime;
        this.utwhenloaded = updatetime;
        this.updatetime = updatetime;
        this.isNew = b;
        this.remote_id = remote_id;
        this.question = question;
        this.mfsremoteid = mfsremoteid;

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

    @Override
    public String toString(){

        return "\nremote_id: " + this.remote_id +
                "\nupdatetime: " + this.updatetime +
                "\nmfsremoteid: " + this.mfsremoteid +
                "\nduetime: " + this.duetime +
                "\nquestion:\n " + this.question;
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

    public static String allToString(){

        StringBuilder sb = new StringBuilder();

        for (Flashcard fs : getAll()){
            sb.append(fs.toString()+"\n\n");
        }

        return sb.toString();
    }
}

