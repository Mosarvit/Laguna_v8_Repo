package com.mosarvit.laguna;

import android.os.AsyncTask;

import com.jcraft.jsch.ChannelSftp;

import java.util.Vector;

/**
 * Created by Mosarvit on 11/5/2017.
 */


public class UploadToFtpTask extends AsyncTask<String, Void, Void> {

    public Exception exception;

    ViewActivity view;
    private String ftpFilePath;
    private String localFilePath;

    private String hostName = SharedData.HOST_NAME;
    private String userName = SharedData.FTP_USERNAME;
    private String password = SharedData.FTP_PASSWORD;
    private String ftpMediaFolder = SharedData.FTP_MEDIA_FOLDER;
    private int portNumber = SharedData.FTP_PORT_NUMBER;

    Vector<ChannelSftp.LsEntry> vtFiles = new Vector<>();

    private boolean blResult = false;
    private boolean uploaded = false;


    public UploadToFtpTask(String ftpFilePath, String localFilePath, ViewActivity view){

        this.ftpFilePath = ftpFilePath;
        this.localFilePath = localFilePath;
        this.view = view;
    }

    @Override
    protected Void doInBackground(String... params) {

        SFTPBean sftpBean = new SFTPBean();

        blResult = sftpBean.connect(hostName, portNumber, userName, password);

        if (blResult) {

            uploaded = sftpBean.uploadFile(localFilePath, ftpFilePath);
            sftpBean.close();
        }
        return null;
    }

    @Override
    protected void onPostExecute(Void aVoid) {

        if (blResult) {
            view.printLine("Connect succeeded");
        } else {
            view.printLine("Connect failed");
        }

        if (uploaded) {
            view.printLine("Successful Upload from " + ftpFilePath + " to " + localFilePath);
        }else{
            view.printLine("Successful Upload from " + ftpFilePath + " to " + localFilePath);
        }
    }
}

