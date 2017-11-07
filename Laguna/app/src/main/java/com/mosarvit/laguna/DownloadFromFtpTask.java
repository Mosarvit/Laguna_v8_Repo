package com.mosarvit.laguna;

import android.os.AsyncTask;

import com.jcraft.jsch.ChannelSftp;

import org.apache.commons.net.ftp.FTPClient;

import java.util.Vector;

/**
 * Created by Mosarvit on 11/5/2017.
 */


public class DownloadFromFtpTask extends AsyncTask<String, Void, Void> {

    public Exception exception;

    ViewActivity view;
    private String ftpFilePath;
    private String localFilePath;

    private String userName = SharedData.FTP_USERNAME;
    private String password = SharedData.FTP_PASSWORD;
    private String ftpMediaFolder = SharedData.FTP_MEDIA_FOLDER;
    private int portNumber = SharedData.FTP_PORT_NUMBER;

    Vector<ChannelSftp.LsEntry> vtFiles = new Vector<>();

    private boolean blResult = false;
    private boolean downloaded = false;
    private boolean uploaded = false;


    public DownloadFromFtpTask(String ftpFilePath, String localFilePath, ViewActivity view){

        this.ftpFilePath = ftpFilePath;
        this.localFilePath = localFilePath;
        this.view = view;

    }

    @Override
    protected Void doInBackground(String... params) {

        FTPClient ftp = null;




//        try {
//            ftp = new FTPClient();
//
//            ftp.connect("ricky.heliohost.org", portNumber);
//
//            Log.d("DownloadFromServer()", "Connected. Reply: " + ftp.getReplyString());
//
//            ftp.login(userName, password);
//            Log.d("DownloadFromServer()", "Logged in");
//            ftp.setFileType(FTP.BINARY_FILE_TYPE);
//            Log.d("DownloadFromServer()", "Downloading");
//            ftp.enterLocalPassiveMode();
//
//            OutputStream outputStream = null;
//            try {
//                outputStream = new BufferedOutputStream(new FileOutputStream(
//                        "log.txt"));
//                success = ftp.retrieveFile("/mediafiles/Caught.in.the.Web.2012/103904-113130.mp4", outputStream);
//            } finally {
//                if (outputStream != null) {
//                    outputStream.close();
//                }
//            }
//
//
//        } catch (SocketException e) {
//            e.printStackTrace();
//        } catch (FileNotFoundException e) {
//            e.printStackTrace();
//        } catch (IOException e) {
//            e.printStackTrace();
//        } finally {
//            if (ftp != null) {
//                try {
//                    ftp.logout();
//                } catch (IOException e) {
//                    e.printStackTrace();
//                }
//                try {
//                    ftp.disconnect();
//                } catch (IOException e) {
//                    e.printStackTrace();
//                }
//            }
//
//
//        JSch jsch = new JSch();
//        Session session = null;
//        try {
//            session = jsch.getSession(userName, "ricky.heliohost.org", 1312);
//
//            session.setConfig("PreferredAuthentications", "password");
//            session.setPassword(password);
//
//            session.connect(10000);
//
//            Channel channel = null;
//
//            channel = session.openChannel("sftp");
//
//            ChannelSftp sftp = (ChannelSftp) channel;
//
//            sftp.connect(10000);
//        } catch (JSchException e) {
//            e.printStackTrace();
//        }
//
//


//        String SFTPHOST = "ricky.heliohost.org";
//        int SFTPPORT = 1312;
//        String SFTPUSER = userName;
//        String SFTPPASS = password;
//        String SFTPWORKINGDIR = "/";
//
//        Session session = null;
//        Channel channel = null;
//        ChannelSftp channelSftp = null;
//
//        try {
//            JSch jsch = new JSch();
//            session = jsch.getSession(SFTPUSER, SFTPHOST, SFTPPORT);
//            session.setPassword(SFTPPASS);
//            java.util.Properties config = new java.util.Properties();
//            config.put("StrictHostKeyChecking", "no");
//            session.setConfig(config);
//            session.connect();
//            channel = session.openChannel("sftp");
//            channel.connect();
//            channelSftp = (ChannelSftp) channel;
//            channelSftp.cd(SFTPWORKINGDIR);
//            byte[] buffer = new byte[1024];
//            BufferedInputStream bis = new BufferedInputStream(channelSftp.get("xczx.php"));
//            File newFile = new File("Test.java");
//            OutputStream os = new FileOutputStream(newFile);
//            BufferedOutputStream bos = new BufferedOutputStream(os);
//            int readCount;
//            while ((readCount = bis.read(buffer)) > 0) {
//                MainActivity.printLine("Writing: ");
//                bos.write(buffer, 0, readCount);
//            }
//            bis.close();
//            bos.close();
//        } catch (Exception ex) {
//            ex.printStackTrace();
//        }


//        Session session = null;
//        Channel channel = null;
//        ChannelSftp channelSftp = null;
//        boolean success = false;
//        JSch jsch = new JSch();
//        try {
//            session = jsch.getSession(userName, "ricky.heliohost.org", 1312);
//
//        session.setPassword(password);
//
//        session.setConfig("StrictHostKeyChecking", "no");
//
//        channel = session.openChannel("sftp");
//        channel.connect();
//        channelSftp = (ChannelSftp) channel;
//
//        String fileToDownload = "xczx.php";
//
//        channelSftp.get(fileToDownload , "/home/mosar/xczx.php");
//        success = true;
//        if (success)
//            MainActivity.printLine("Downloaded file: " + fileToDownload );
//    } catch (JSchException e) {
//        e.printStackTrace();
//    } catch (SftpException e) {
//            e.printStackTrace();
//        }


        SFTPBean sftpBean = new SFTPBean();

        blResult = sftpBean.connect("ricky.heliohost.org", 1312, "mosar", "Fahrenheit");

        if (blResult) {

            //now we will try to download file
//			blResult = sftpBean.uploadFile( "E:\\test.c","/test/16LF1824_6.c_1");
//			if(blResult) {
//				System.out.println("upload successed");
//			}
//			else {
//				System.out.println("u failed");
//			}
////			//in here i demo list file first.
            //checking again file that u have just uploaded file
//            vtFiles = sftpBean.listFile("/home/mosar");

            downloaded = sftpBean.downloadFile(ftpFilePath , localFilePath);
//            String localPath = SharedData.ROOT_FOLDER + "/hello_file";

//            uploaded = sftpBean.uploadFile(localPath , "/home/mosar/mediafiles/hello_file.php" );



            sftpBean.close();
        }
        return null;
    }


    @Override
    protected void onPostExecute(Void aVoid) {


        view.printLine("userName : " + userName);
        view.printLine("password : " + password);
        view.printLine("ftpMediaFolder : " + ftpMediaFolder);
        view.printLine("portNumber : " + portNumber);
        view.printLine("localPath : " + SharedData.ROOT_FOLDER + "/hello_file");

        if (blResult) {
            view.printLine("Connect succeeded");
        } else {
            view.printLine("Connect failed");
        }

//        if (uploaded) {
//
//            MainActivity.printLine("Upload succeeded");
//
//        }else{
//            MainActivity.printLine("Upload failed");
//        }

//        if (vtFiles != null) {
//            for (ChannelSftp.LsEntry lsEntry : vtFiles) {
//                MainActivity.printLine(lsEntry.getFilename() + "\r\n");
//            }
//        }

        if (downloaded) {

            view.printLine("Successful Download from " + ftpFilePath + " to " + localFilePath);


        }else{
            view.printLine("Successful Download from " + ftpFilePath + " to " + localFilePath);
        }


//
//        super.onPostExecute(aVoid);
//        if (success)
//            MainActivity.printLine("ok");
//        else
//            MainActivity.printLine("not ok");


    }
}

