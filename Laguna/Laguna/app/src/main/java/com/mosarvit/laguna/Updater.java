package com.mosarvit.laguna;

import com.activeandroid.Model;
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

        File f = new File(SharedClass.LOCAL_MEDIA_FOLDER);
        if (!f.exists()) {
            f.mkdir();
        }
        File[] files = f.listFiles();


        List<MediaFileSegment> mediaFileNames = new Select().from(MediaFileSegment.class).groupBy("mediafileName").execute();

        for (File file : files) {
            if (file.isDirectory()) {

//                view.printLine("Deleting extra files started");

                File[] files2 = file.listFiles();
                for (File file2 : files2) {

                    if (file2.isFile() && new Select().from(MediaFileSegment.class).where("mediafileName = '" + getName(file) + "' AND fileName = '" + getName(file2) + "'").count() == 0) {

                        file2.delete();
//                        view.printLine("Deleting file: " + getName(file2));
                    }
                }

                if (file.listFiles().length == 0) {
                    file.delete();
                }

//                view.printLine("Deleting extra entries started");

                for (Model m :  new Select().from(MediaFileSegment.class).where("mediafileName = '" + getName(file)+ "'").execute()) {

                    MediaFileSegment mfs = (MediaFileSegment) m;

                    boolean contains = false;

                    for (File file2 : files2) {

//                        view.printLine("\ngetName(file): " + getName(file));
//                        view.printLine("getName(file2): " + getName(file2));

                        if (getName(file2).equals(mfs.fileName) && getName(file).equals(mfs.mediaFileName))
                            contains = true;
                    }

                    if (!contains) {
                        view.printLine("Deleting db entry: " + mfs.fileName);
                        Factory.deleteMediaFileSegment(mfs);
                    }

                }


            } else {

                file.delete();
            }


        }

        for (MediaFileSegment mfs : MediaFileSegment.getAll()) {

            boolean contains = false;

            for (File file : files) {

                if (getName(file).equals(mfs.mediaFileName))
                    contains = true;
            }

            if (!contains) {
                Factory.deleteMediaFileSegment(mfs);
            }

        }

        view.printLine("Local MediaFiles Inventory finished");
    }

    private static String getName(File f){

        return FileHelper.getName(f);
    }


}

