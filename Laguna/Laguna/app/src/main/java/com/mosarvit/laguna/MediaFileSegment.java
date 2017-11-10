package com.mosarvit.laguna;

import com.activeandroid.annotation.Column;
import com.activeandroid.query.Select;

import java.util.HashMap;
import java.util.List;

/**
 * Created by Mosarvit on 11/3/2017.
 */

public class MediaFileSegment extends OnServerModel {
    public MediaFileSegment() {
    }

    public MediaFileSegment(int remote_id, long updatetime, String filename, String mediaFileName, boolean b) {
        super();
        this.utwhenloaded = updatetime;
        this.updatetime = updatetime;
        this.remote_id = remote_id;
        this.fileName = filename;
        this.mediaFileName = mediaFileName;
        this.isNew = b;
    }

    @Column(name = "fileName")
    public String fileName;

    @Column(name = "mediafileName")
    public String mediaFileName;

    @Column(name = "toUpload")
    private boolean toUpload = false;

    @Column(name = "toDownload")
    private boolean toDownload = false;

//    @Column(name = "flashcard", onDelete = Column.ForeignKeyAction.CASCADE)
//    public Flashcard flashcard;


    public static List<MediaFileSegment> getAll() {

        return new Select().from(MediaFileSegment.class).execute();
    }

    public static HashMap<Integer, OnServerModel> getAllAsHM() {

        HashMap<Integer, OnServerModel> hm = new HashMap<Integer, OnServerModel>();

        for (MediaFileSegment fc : getAll()) {
            hm.put(fc.remote_id, fc);
        }

        return hm;
    }

    @Override
    public String toString() {

        return "\nremote_id: " + this.remote_id +
                "\nfileName: " + this.fileName +
                "\nmediaFileName: " + this.mediaFileName +
                "\nupdatetime: " + this.updatetime +
                "\nutwhenloaded: " + this.utwhenloaded +
                "\ntoDownload: " + this.toDownload +
                "\ntoUpload: " + this.toUpload
                ;
    }


    public static String allToString() {

        StringBuilder sb = new StringBuilder();

        for (MediaFileSegment mfs : getAll()) {
            sb.append(mfs.toString() + "\n\n");
        }

        return sb.toString();
    }

    public void setToDownload(boolean toDownload) {
        this.toDownload = toDownload;
        this.save();

        updateFlashcardsReady();
    }



    public void setToUpload(boolean toUpload) {
        this.toUpload = toUpload;
        this.save();
        updateFlashcardsReady();

    }

    private void updateFlashcardsReady() {
        Flashcard fc = new Select().from(Flashcard.class).where("mfsremoteid = " + this.remote_id).executeSingle();
        if (fc != null) {

            if (!this.toDownload && !this.toUpload) {
                fc.ready = true;
                fc.save();

            } else {
                fc.ready = false;
                fc.save();

            }
        }
    }

    public boolean getToDownload() {
        return toDownload;
    }
}
