using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashcardMaker.Views;
using FlashcardMaker.Models;
using FlashcardMaker.Helpers;
using System.IO;
using System.Data.Entity.Migrations;

namespace FlashcardMaker.Controllers
{
    class MediaFilesController
    {
        private ISessionView view;

        public MediaFilesController(ISessionView view)
        {
            this.view = view;
        }

        internal void creatMediaFiles()
        {
            using (MyDbContext db = new MyDbContext())
            {
                view.printLine("Starting creating Mediafiles");
                Updater.updateDbMediaFiles(db, view);

                //return;

                int totalCount = 0;

                foreach (SubtitleLinePack stlp in db.SubtitleLinePacks.Where(p=>p.Rank>0).ToList())
                {
                    if (stlp.MediaFileSegments_remote_id > 0)
                    {
                        continue;
                    }

                    view.printLine("stlp: " + stlp.StartTime);

                    if (++totalCount > ProgramController.MAX_MEDIA_FILES_TO_CREATE && ProgramController.DEBUGGING_MEDIA_FILES)
                    {
                        view.printLine("achieved MAX_MEDIA_FILES_TO_CREATE");
                        break;
                    }

                        

                    //foreach (SubtitleLine stl in stlp.SubtitleLines)
                    //{
                    //    view.printLine("Position: " + stl.Position);
                    //}

                    //db.Movies.ToList();

                    // TO-DO What if duplicate fileNames?

                    string movieNameWithoutExtention = Path.GetFileNameWithoutExtension(stlp.Movie.fileName);
                    string usersMediaFolder = Properties.Settings.Default.UsersMediaFolder;

                    string inputFileName = "";

                    foreach (string mediaFileName in Directory.GetFiles(path: usersMediaFolder))
                    {
                        if (movieNameWithoutExtention.Equals(Path.GetFileNameWithoutExtension(mediaFileName)))
                        {
                            inputFileName = mediaFileName;
                        }
                    }

                    int starttime = stlp.StartTime;
                    int endtime = stlp.EndTime;
                    string outpuFileName = starttime + "-" + endtime + Path.GetExtension(inputFileName);

                    if (inputFileName.Equals(""))
                    {
                        view.printLine("No file found for : " + movieNameWithoutExtention);
                        break;
                    }

                    // check if maybe such file already exists in db

                    var mf = db.MediaFiles.Where(c => c.FileName.Equals(movieNameWithoutExtention)).SingleOrDefault();
                    if (mf != null)
                    {
                        var mfs = mf.MediaFileSegments.Where(c => c.FileName.Equals(outpuFileName)).SingleOrDefault();
                        if (mfs != null)
                        {
                            view.printLine("continue");
                            continue;
                        }
                    }                  

                    

                    // create mediaFiles Folder

                    string mediaFilesPath = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName, @"MediaFiles");

                    if (!Directory.Exists(mediaFilesPath))
                    {
                        Directory.CreateDirectory(mediaFilesPath);
                    }

                    // create the folder for this Movie

                    string outputFileFolder = Path.Combine(mediaFilesPath, movieNameWithoutExtention);

                    if (!Directory.Exists(outputFileFolder))
                    {
                        Directory.CreateDirectory(outputFileFolder);
                    }

                    string inputFileFullPath = Path.Combine(usersMediaFolder, inputFileName);

                    view.printLine("mediaFileNameToProcess : " + inputFileName);




                    string outputFileFullPath = Path.Combine(outputFileFolder, outpuFileName);


                    view.printLine("inputFileName : " + inputFileName);
                    view.printLine("outputFileName : " + outputFileFullPath);


                    VideoEditor ve = new VideoEditor(view);

                    if (ve.splitVideo(inputFileName, outputFileFullPath, starttime, endtime))
                    {
                        MediaFileSegment mfs = Factory.InsertMediaFileSegment(db, view, outpuFileName, movieNameWithoutExtention, true);
                        stlp.MediaFileSegments_remote_id = mfs.remote_id;
                        db.SaveChanges();
                    }
                        

                        
                        

                    //break;
                }
            }
        }



    }
}
