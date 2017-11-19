package com.mosarvit.laguna;

import android.os.AsyncTask;

import com.jcraft.jsch.ChannelSftp;

import java.io.File;
import java.util.Vector;

/**
 * Created by Mosarvit on 11/5/2017.
 */


public class UploadToFtpTask extends AsyncTask<String, Void, Void> {

    public Exception exception;

    ViewActivity view;
    private String ftpFilePath;
    private String localFilePath;

    private MediaFileSegment mfs;

    private String hostName = SharedClass.HOST_NAME;
    private String userName = SharedClass.FTP_USERNAME;
    private String password = SharedClass.FTP_PASSWORD;
    private String ftpMediaFolder = SharedClass.FTP_MEDIA_FOLDER;
    private int portNumber = SharedClass.FTP_PORT_NUMBER;

    Vector<ChannelSftp.LsEntry> vtFiles = new Vector<>();

    private boolean blResult = false;
    private boolean uploaded = false;


    public UploadToFtpTask(MediaFileSegment mfs, ViewActivity view) {

        this.view = view;
        this.mfs = mfs;

        String ftpFilePath = ftpMediaFolder + "/" + mfs.mediaFileName + "/" + mfs.fileName;
        String localFilePath = SharedClass.LOCAL_MEDIA_FOLDER + "/" + mfs.mediaFileName + "/" + mfs.fileName;

        this.ftpFilePath = ftpFilePath;
        this.localFilePath = localFilePath;
    }

    @Override
    protected Void doInBackground(String... params) {

        SFTPBean sftpBean = new SFTPBean();

        blResult = sftpBean.connect(hostName, portNumber, userName, password);

        if (blResult) {

            int retry = 0 ;
            while (!uploaded && retry++< SharedClass.MAX_RETRIES) {
                uploaded = sftpBean.uploadFile(localFilePath, ftpFilePath);
            }
            sftpBean.close();
        }
        return null;
    }

    @Override
    protected void onPostExecute(Void aVoid) {

        if (blResult) {
            view.printLine("Connect succeeded");
            if (uploaded) {
                view.printLine("Successful Upload : " + FileHelper.getName(localFilePath));
                mfs.setToUpload(false);

            }else{
                view.printLine("Failed to upload : " + FileHelper.getName(localFilePath));
                SharedClass.setLastSyncSuccessful(false, view);
            }
        } else {
            view.printLine("Connect failed");
        }


    }
}

