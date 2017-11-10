using FlashcardMaker.Controllers;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashcardMaker.Views;

namespace FlashcardMaker.Helpers
{
    class VideoEditor
    {

        private IController controller;
        //private int StartTime = 30;
        //private int EndTime = 32;
        //private string SourceFile = "E:\\Users\\Mosarvit\\Downloads\\Nick Sibicky Go Lecture #71 - 4-4 Joseki Workshop.mp4";
        //private string DestinationFile = "E:\\Users\\Mosarvit\\Downloads\\Nick Sibicky Go Lecture #71 - 4-4 Joseki Workshop9.mp4";
        private ISessionView view;

        public VideoEditor(ISessionView view)
        {
            this.view = view;
        }

        public void test(IController controller)
        {
            this.controller = controller;
            //splitVideo();
        }

        public void splitVideo1(string SourceFile, string DestinationFile, int StartTime, int EndTime)
        {
            var ffMpegConverter = new FFMpegConverter();
            ffMpegConverter.ConvertMedia(SourceFile, null, DestinationFile, null,
                new ConvertSettings()
                {
                    Seek = StartTime,
                    MaxDuration = (EndTime - StartTime), // chunk duration
                    VideoCodec = "libx264",
                    AudioCodec = "mp3"
                });
            controller.printLine("Done");
        }

        public bool splitVideo(string inputFile, string outputFile, int start, int end)
        {
            bool result = true;
            int interval = end - start;
            string startString = TimeSpan.FromMilliseconds(start).ToString();
            startString = startString.Substring(0, startString.Length - 4);
            string intervalString = TimeSpan.FromMilliseconds(interval).ToString();
            intervalString = intervalString.Substring(0, intervalString.Length - 4);
            
            string _ffExe = @"E:\Users\Mosarvit\Documents\GitHubRepos\Laguna_v8_Repo\Laguna\FlashcardMaker\FlashcardMaker\Other Resources\ffmpeg.exe";
            string Parameters = "-i " + inputFile + " -ss " + startString + " -t " + intervalString + " " + outputFile;
            //string Parameters = "-i E:\\Users\\Mosarvit\\Downloads\\a.mp4 -ss 50.684 -t 0.869 E:\\Users\\Mosarvit\\Downloads\\a27.mp4";

            view.printLine("Parameters : " + Parameters);

            //create a process info
            ProcessStartInfo oInfo = new ProcessStartInfo(_ffExe, Parameters);
            oInfo.WindowStyle = ProcessWindowStyle.Hidden;
            oInfo.CreateNoWindow = true;
            oInfo.UseShellExecute = false;
            Process proc = new Process();
            proc.StartInfo = oInfo;
            
            //proc.StartInfo.FileName = "ffmpeg";
            //proc.StartInfo.Arguments = "-i E:\\Users\\Mosarvit\\Downloads\\a.mp4 -ss 50.684 -t 0.869 E:\\Users\\Mosarvit\\Downloads\\a5.mp4";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            if (!proc.Start())
            {
                view.printLine("Error starting");
                return false;
            }
            StreamReader reader = proc.StandardError;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                view.printLine(line);
                if (line.StartsWith("Error"))
                    result = false;
                else
                    result = true;
            }
            proc.Close();
            return result;
        }
 

        public void splitVideo3()
        {
            var inputFile = new MediaFile { Filename = @"E:\Users\Mosarvit\Downloads\a.mp4" };
            var outputFile = new MediaFile { Filename = @"E:\Users\Mosarvit\Downloads\a14.mp4" };

            var conversionOptions = new ConversionOptions
            {
                Seek = new TimeSpan(0, 1, 31, 458),
                MaxVideoDuration = new TimeSpan(0, 0, 3, 798)
            };

            using (var engine = new Engine())
            {
                engine.Convert(inputFile, outputFile, conversionOptions);
            }

        }

        public void splitVideo4()
        {
            string _ffExe = @"E:\Users\Mosarvit\Documents\GitHubRepos\Laguna_v8_Repo\FlashcardMaker\FlashcardMaker\Other Resources\ffmpeg.exe";
            string Parameters = "-i E:\\Users\\Mosarvit\\Downloads\\a.mp4 -ss 00:01:50.479 -t 2.134 E:\\Users\\Mosarvit\\Downloads\\a17.mp4";
            //create a process info
            ProcessStartInfo oInfo = new ProcessStartInfo(_ffExe, Parameters);
            oInfo.UseShellExecute = false;
            oInfo.CreateNoWindow = true;
            oInfo.RedirectStandardOutput = true;
            oInfo.RedirectStandardError = true;

            //Create the output and streamreader to get the output
            string output = null;
            /* StreamReader srOutput = null;*/



            ////try the process
            //try
            //{
            //    //run the process
            //    Process proc = System.Diagnostics.Process.Start(oInfo);

            //    proc.WaitForExit();

            //    //get the output
            //    srOutput = proc.StandardError;

            //    //now put it in a string
            //    output = srOutput.ReadToEnd();

            //    proc.Close();
            //}
            //catch (Exception)
            //{
            //    output = string.Empty;
            //}
            //finally
            //{
            //    //now, if we succeded, close out the streamreader
            //    if (srOutput != null)
            //    {
            //        srOutput.Close();
            //        srOutput.Dispose();
            //    }
            //}

            controller.printLine(output);
        }


    }
}
