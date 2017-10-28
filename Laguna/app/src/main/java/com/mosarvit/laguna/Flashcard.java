package com.mosarvit.laguna;

import com.activeandroid.Model;
import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;
import com.activeandroid.query.Select;

import java.util.List;

@Table(name = "Flashcards")
public class Flashcard extends Model {

    @Column(name = "remote_id", unique = true, onUniqueConflict = Column.ConflictAction.REPLACE)
    public int id;

    @Column(name = "status")
    private short status;        // 0 - default, 9 - to be deleted on the server on the next sync

    @Column(name = "question")
    private String question;

    @Column(name = "duetime")
    private long duetime;

    @Column(name = "loadedtoapptime")
    private long loadedtoapptime;

    @Column(name = "updatetimelocal")
    public long updatetimelocal;

    @Column(name = "updatetimewhenloaded")
    public long updatetimewhenloaded;

    public void setDuetime(long duetime){

        this.updatetimelocal = System.currentTimeMillis();
        this.duetime = duetime;
    }

    public void setQuestion(String question){

        this.updatetimelocal = System.currentTimeMillis();
        this.question = question;
    }

    public String getQuestion(){

        return this.question;
    }

    public long getDuetime(){

        return this.duetime;
    }

    public long getUpdateTimeLocal(){

        return this.updatetimelocal;
    }

    public long getUpdatetimeWhenLoaded(){

        return this.updatetimewhenloaded;
    }

    public int getStatus(){

        return this.status;
    }


    public void setToDeleteOnServer(){

        this.loadedtoapptime = System.currentTimeMillis();
        this.status = 9;
    }

    public Flashcard(){}

    public Flashcard(int id, String question, long duetime, long updatetime){
        super();
        this.duetime = duetime;
        this.updatetimewhenloaded = updatetime;
        this.updatetimelocal = updatetime;
        this.id = id;
        this.question = question;
    }

    public Flashcard(int id, long updatetime){
        super();
        this.updatetimewhenloaded = updatetime;
        this.updatetimelocal = updatetime;
        this.id = id;
    }

    public Flashcard(int id){
        super();
        this.id = id;
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

        return "Id: " + this.id +
                "\nquestion: " + this.question +
                "\nloadedtoapptime: " + this.loadedtoapptime +
                "\nupdatetimelocal: " + this.updatetimelocal +
                "\nduetime: " + this.duetime +
                "\n\n";
    }
}

