package com.mosarvit.laguna;

import com.activeandroid.query.Select;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

/**
 * Created by Mosarvit on 11/6/2017.
 */

public class Updater {

    public static void updateDbMediaFiles(ViewActivity view) {

        view.printLine("Local MediaFiles Inventory started");

        ArrayList<MediaFileSegment> mf_s = new ArrayList<MediaFileSegment>();

        File f = new File(SharedData.LOCAL_MEDIA_FOLDER);
        if(!f.exists()){
            f.mkdir();
        }
        File[] files = f.listFiles();


        List<MediaFileSegment> mediaFileNames = new Select().from(MediaFileSegment.class).groupBy("mediafileName").execute();

        for (File file : files) {
            if (file.isDirectory()) {

                File[] files2 = file.listFiles();
                for (File file2 : files2) {

                    if ( file2.isFile() &&  new Select().from(MediaFileSegment.class).where("mediafileName = '" + file.getName() + "' AND fileName = '" + file2.getName() + "'").count() == 0) {

                        file2.delete();
                    }
                }

                if (file.listFiles().length == 0) {
                    file.delete();
                }

                for (MediaFileSegment mfs : MediaFileSegment.getAll()) {

                    boolean contains = false;

                    for (File file2 : files2) {

                        if (file2.getName().equals(mfs.fileName) && file.getName().equals(mfs.mediaFileName))
                            contains = true;
                    }

                    if (!contains)
                        Factory.deleteMediaFileSegment(mfs);
                }


            } else {

                file.delete();
            }


        }

        for (MediaFileSegment mfs : MediaFileSegment.getAll()) {

            boolean contains = false;

            for (File file : files) {

                if (file.getName().equals(mfs.mediaFileName))
                    contains = true;
            }

            if (!contains)
                Factory.deleteMediaFileSegment(mfs);
        }

        view.printLine("Local MediaFiles Inventory finished");
    }
}
