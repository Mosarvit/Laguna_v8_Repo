using FlashcardMaker.Models;
using FlashcardMaker.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Helpers
{
    class Updater
    {
        internal static void updateDbMediaFiles(MyDbContext db, ISessionView view)
        {
            view.printLine("Local MediaFiles Inventory started");

            List<MediaFile> mf_s = new List<MediaFile>();

            string[] subDirectories = Directory.GetDirectories(Properties.Settings.Default.ApplicationsMediaFolder);
            List<string> directoryNames = new List<string>();
            foreach (string directory in subDirectories)
            {
                string folderName = Path.GetFileName(directory);
                directoryNames.Add(folderName);
                MediaFile mf = db.MediaFiles.Where(c => c.FileName.Equals(folderName)).SingleOrDefault();

                if (mf == null)
                {
                    FileManagementHelper.DeleteDirectory(directory);
                    view.printLine("Deleting " + folderName);
                    break;
                }

                view.printLine("Checking in  " + directory);

                // delete Files that are not in db

                string[] files = Directory.GetFiles(directory);
                List<string> fileNames = new List<string>();
                foreach (string file in files)
                {


                    string fileName = Path.GetFileName(file);
                    fileNames.Add(fileName);

                    view.printLine("fileName:  " + fileName);
                    if (mf.MediaFileSegments.Where(c => c.FileName.Equals(fileName)).Count() == 0)
                    {
                        File.Delete(file);
                        view.printLine("Deleting " + fileName);
                        break;
                    }
                }

                // delete db MediaFileSegments, that we have no File of

                foreach (var mfs in mf.MediaFileSegments.ToList())
                {
                    if (!fileNames.Contains(mfs.FileName))
                    {
                        Factory.DeleteMediaFileSegment(db, view, mfs);
                        //db.MediaFileSegments.Remove(mfs);                        
                    }
                }

                db.SaveChanges();
            }

            // delete db MediaFiles, that we have no File of

            foreach (var mf in db.MediaFiles.ToList())
            {
                if (mf.MediaFileSegments.Count()==0)
                {
                    Factory.DeleteMediaFile(db, view, mf);
                }
            }

            view.printLine("Local MediaFiles Inventory finished");
        }
    }
}
