package com.mosarvit.laguna;

import java.io.File;

/**
 * Created by Mosarvit on 11/9/2017.
 */

public class FileHelper {

    public static String getName(String str) {

        int index = str.lastIndexOf('/');
        return str.substring(index + 1);
    }

    public static String getName(File f) {

        int index = f.getName().lastIndexOf('/');
        return f.getName().substring(index + 1);
    }

}
