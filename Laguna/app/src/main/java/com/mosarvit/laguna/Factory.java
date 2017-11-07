package com.mosarvit.laguna;

/**
 * Created by Mosarvit on 11/7/2017.
 */

public class Factory {

    public static void deleteMediaFileSegment(MediaFileSegment mfs){
        if (mfs.flashcard!=null){
            mfs.flashcard.mediaFileSegment = null;
        }
        mfs.delete();
    }
}
