package com.mosarvit.laguna;

import com.activeandroid.Model;
import com.activeandroid.annotation.Column;
import com.activeandroid.annotation.Table;

@Table(name = "Flashcards")
public class Flashcard extends Model {

    @Column(name = "remote_id", unique = true, onUniqueConflict = Column.ConflictAction.REPLACE)
    public int id;

    @Column(name = "Question")
    public String question;

    public Flashcard(){}

    public Flashcard(int id, String question){
        super();
        this.id = id;
        this.question = question;
    }
}

