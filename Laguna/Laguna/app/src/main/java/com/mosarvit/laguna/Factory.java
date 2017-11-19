package com.mosarvit.laguna;

import com.activeandroid.query.Select;

/**
 * Created by Mosarvit on 11/7/2017.
 */

public class Factory {

    public static void deleteMediaFileSegment(MediaFileSegment mfs){

        Flashcard fc = new Select().from(Flashcard.class).where("mfsremoteid = " + mfs.remote_id).executeSingle();

        if (fc!=null){
            fc.mfsremoteid = 0;
        }
        mfs.delete();
    }
}
