package com.mosarvit.laguna;

import com.jcraft.jsch.ChannelSftp;
import com.jcraft.jsch.JSch;
import com.jcraft.jsch.Session;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.util.Vector;

/**
 * Created by Mosarvit on 11/5/2017.
 */

public class SFTPBean {
    //variable let using for sftp channel
    private JSch mJschSession = null;
    private Session mSSHSession = null;

    //sftp channel
    private ChannelSftp mChannelSftp = null;

    //connect fucntion let connect to sftp server
    //in here or any protocol please remember some the other variable very important
    //timeout but in this demo i only demo with normal function.
    //still having many function u can add for sftp: create, delete, set right.. for sftp server
    public boolean connect(String strHostAddress, int iPort, String strUserName, String strPassword) {
        boolean blResult = false;

        try {
            //creating a new jsch session
            this.mJschSession = new JSch();

            //set sftp server no check key when login
            java.util.Properties config = new java.util.Properties();
            config.put("StrictHostKeyChecking", "no");
            this.mJschSession.setConfig(config);

            //creating session with user, host port
            this.mSSHSession = mJschSession.getSession(strUserName, strHostAddress, iPort);

            //set password
            this.mSSHSession.setPassword(strPassword);

            //connect to host
            this.mSSHSession.connect();

            //open sftp channel
            this.mChannelSftp = (ChannelSftp) this.mSSHSession.openChannel("sftp");

            //connect to sftp session
            this.mChannelSftp.connect();
            if (this.mChannelSftp != null) {
                blResult = true;

                System.out.println(mChannelSftp.pwd());



            }




        } catch (Exception exp) {
            exp.printStackTrace();
        }
        return blResult;
    }

    //list file on sftp server
    public Vector<ChannelSftp.LsEntry> listFile(String strPath) {
        Vector<ChannelSftp.LsEntry> vtFile = null;

        try {
            vtFile = this.mChannelSftp.ls(strPath);
        } catch (Exception exp) {
            exp.printStackTrace();
        }
        return vtFile;
    }

    //download file
    public boolean downloadFile(String strSftpFile, String strLocalFile) {
        boolean blResult = false;

        try {
            this.mChannelSftp.get(strSftpFile, strLocalFile);
            blResult = true;
        } catch (Exception exp) {
            exp.printStackTrace();
        }
        return blResult;
    }

    //upload file
    public boolean uploadFile(String strLocalFile, String strSftpFile) {
        boolean blResult = false;

        try {
            FileInputStream fis = new FileInputStream(new File(strLocalFile));
            this.mChannelSftp.put(fis, strSftpFile);
            blResult = true;
        } catch (Exception exp) {
            exp.printStackTrace();
        }
        return blResult;
    }

    //close session
    public void close() {
        try {
            this.mChannelSftp.disconnect();
        } catch (Exception exp) {

        }

        try {
            this.mSSHSession.disconnect();
        } catch (Exception exp) {

        }

        this.mChannelSftp = null;
        this.mSSHSession = null;
        this.mJschSession = null;
    }
}
