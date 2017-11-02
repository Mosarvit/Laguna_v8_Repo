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

                foreach (SubtitleLinePack stlp in db.SubtitleLinePacks.ToList())
                {
                    //foreach (SubtitleLine stl in stlp.SubtitleLines)
                    //{
                    //    view.printLine("Position: " + stl.Position);
                    //}

                    //db.Movies.ToList();

                    // TO-DO What if duplicate fileNames?

                    string movieNameWithoutExtention = Path.GetFileNameWithoutExtension(stlp.Movie.fileName);

                    string mediaFolder = Properties.Settings.Default.MediaFolder;

                    string inputFileName = "";

                    foreach (string mediaFileName in Directory.GetFiles(path: mediaFolder))
                    {
                        if (movieNameWithoutExtention.Equals(Path.GetFileNameWithoutExtension(mediaFileName)))
                        {
                            inputFileName = Path.Combine(mediaFolder, mediaFileName);
                        }
                    }

                    if (inputFileName.Equals(""))
                    {
                        view.printLine("No file found for : " + movieNameWithoutExtention);
                        break;
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

                    view.printLine("mediaFileNameToProcess : " + inputFileName);

                    int starttime = stlp.StartTime;
                    int endtime = stlp.EndTime;

                    string outputFileName = Path.Combine(outputFileFolder, starttime + "-" + endtime + Path.GetExtension(inputFileName));


                    view.printLine("inputFileName : " + inputFileName);
                    view.printLine("outputFileName : " + outputFileName);


                    VideoEditor ve = new VideoEditor(view);

                    if (!File.Exists(outputFileName))
                    {
                        ve.splitVideo(inputFileName, outputFileName, starttime, endtime);

                        int remote_id;

                        if (db.MediaFileSegments.Count() == 0)
                        {
                            remote_id = 0;
                        }
                        else
                        {
                            remote_id = db.MediaFileSegments.Max(u => u.remote_id) + 1;
                        }

                        MediaFileSegment mfs = new MediaFileSegment
                        {
                            MediaFileName = movieNameWithoutExtention,
                            remote_id = remote_id,
                            isNew = true
                        };
                        mfs.utlocal = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        mfs.utserverwhenloaded = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        mfs.toDelete = false;

                        db.MediaFileSegments.AddOrUpdate(p => p.remote_id, mfs);
                        db.SaveChanges();
                    }

                    //break;
                }
            }
        }



    }
}
