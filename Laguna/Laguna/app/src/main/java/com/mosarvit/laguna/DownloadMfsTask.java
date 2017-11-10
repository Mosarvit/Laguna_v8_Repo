package com.mosarvit.laguna;

import android.os.AsyncTask;


import com.activeandroid.query.Select;

import java.io.File;

/**
 * Created by Mosarvit on 11/5/2017.
 */


public class DownloadMfsTask extends AsyncTask<String, Void, Void> {

    public Exception exception;

    ViewActivity view;
    private String ftpFilePath;
    private String localFilePath;
    private MediaFileSegment mfs;
    private int counter = 0;

    private String hostName = SharedClass.HOST_NAME;
    private String userName = SharedClass.FTP_USERNAME;
    private String password = SharedClass.FTP_PASSWORD;
    private String ftpMediaFolder = SharedClass.FTP_MEDIA_FOLDER;
    private int portNumber = SharedClass.FTP_PORT_NUMBER;

    private boolean blResult = false;
    private boolean downloaded = false;
    private boolean uploaded = false;


    public DownloadMfsTask(MediaFileSegment mfs, ViewActivity view) {



        this.view = view;
        this.mfs = mfs;

        String ftpFilePath = ftpMediaFolder + "/" + mfs.mediaFileName + "/" + mfs.fileName;
        File localFolderPath = new File(SharedClass.LOCAL_MEDIA_FOLDER + "/" + mfs.mediaFileName);
        if (!localFolderPath.exists()) {
            localFolderPath.mkdirs();
        }
        String localFilePath = localFolderPath + "/" + mfs.fileName;

        this.ftpFilePath = ftpFilePath;
        this.localFilePath = localFilePath;
    }

    @Override
    protected Void doInBackground(String... params) {

        SFTPBean sftpBean = new SFTPBean();

        blResult = sftpBean.connect(hostName, portNumber, userName, password);

        if (blResult) {

            int retry = 0 ;
            while (!downloaded && retry++< SharedClass.MAX_RETRIES) {

                downloaded = sftpBean.downloadFile(ftpFilePath, localFilePath);
                counter++;
            }
            sftpBean.close();
        }

        return null;
    }


    @Override
    protected void onPostExecute(Void aVoid) {

        if (blResult) {
            view.printLine("Connect succeeded");

            if (downloaded) {

                view.printLine("Successful Download : " + FileHelper.getName(localFilePath));
                mfs.setToDownload(false);
                mfs.save();

            } else {
                view.printLine("Failed to download to " + localFilePath);
                SharedClass.setLastSyncSuccessful(false, view);
            }

            view.printLine("Tries needed : " + counter);
        } else {
            view.printLine("Connect failed");
        }

        if (new Select().from(MediaFileSegment.class).where("toDownload = 1 OR toUpload = 1").count()==0){
            view.printLine("MediaFiles up to Date!");
        }
    }
}

