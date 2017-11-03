package com.mosarvit.laguna;

import com.activeandroid.annotation.Column;
import com.activeandroid.query.Select;

import java.util.HashMap;
import java.util.List;

/**
 * Created by Mosarvit on 11/3/2017.
 */

public class MediaFileSegment extends OnServerModel
{
    @Column(name = "fileName")
    public String fileName;

    @Column(name = "mediafileName")
    public String mediaFileName;

    public static List<MediaFileSegment> getAll(){

        return new Select().from(MediaFileSegment.class).execute();
    }

    public static HashMap<Integer, OnServerModel> getAllAsHM(){

        HashMap<Integer, OnServerModel> hm = new HashMap<Integer, OnServerModel>();

        for (MediaFileSegment fc : getAll()){
            hm.put(fc.remote_id, fc);
        }

        return hm;
    }
}
