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

    @Column(name = "question")
    public String question;

    @Column(name = "duetime")
    public long duetime;

    @Column(name = "loadedtoapptime")
    public long loadedtoapptime;

    @Column(name = "updatetime")
    public long updatetime;

    public Flashcard(){}

    public Flashcard(int id, String question, long duetime, long updatetime){
        super();
        this.duetime = duetime;
        this.loadedtoapptime = System.currentTimeMillis();
        this.updatetime = updatetime;
        this.id = id;
        this.question = question;
    }

    public Flashcard(int id, long updatetime){
        super();
        this.updatetime = updatetime;
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
                "\nupdatetime: " + this.updatetime +
                "\nduetime: " + this.duetime +
                "\n\n";
    }
}

