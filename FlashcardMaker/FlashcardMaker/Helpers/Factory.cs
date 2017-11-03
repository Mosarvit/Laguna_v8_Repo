using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashcardMaker.Models;
using FlashcardMaker.Views;

namespace FlashcardMaker.Helpers
{
    public class Factory
    {
        internal static MediaFileSegment InsertMediaFileSegment(MyDbContext db, ISessionView view, string fileName, string mediaFileName)
        {
            int remote_id = 0;

            if (db.MediaFileSegments.Count() > 0)
            {
                remote_id = db.MediaFileSegments.Max(u => u.remote_id) + 1;
            }


            MediaFileSegment mfs = new MediaFileSegment
            {
                FileName = fileName,
                MediaFileName = mediaFileName,
                remote_id = remote_id,
                utlocal = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                utserverwhenloaded = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                isNew = true
            };

            var result = db.MediaFiles.Where(x => x.FileName.Equals(mediaFileName));

            if (result.Count() > 0)
            {
                MediaFile mf = db.MediaFiles.Where(x => x.FileName.Equals(mediaFileName)).FirstOrDefault();
                mf.MediaFileSegments.Add(mfs);
            }
            else
            {
                MediaFile mf = new MediaFile { FileName = mediaFileName };
                mf.MediaFileSegments.Add(mfs);
                db.MediaFiles.Add(mf);
            }

            try
            {
                int answer = db.SaveChanges();
                view.printLine("Number of changes executed: " + answer);
            }
            catch (Exception)
            {
                throw;
            }

            return mfs;
        }


        internal static MediaFileSegment InsertOrUpdateMediaFileSegment(MyDbContext db, ISessionView view, int remote_id, long utlocal, string fileName, string mediaFileName, bool isNew)
        {
            MediaFileSegment mfs = db.MediaFileSegments.Where(c => c.remote_id == remote_id).SingleOrDefault();

            if (mfs == null)
            {
                mfs = new MediaFileSegment
                {
                    remote_id = remote_id,
                    FileName = fileName,
                    MediaFileName = mediaFileName,                    
                    utlocal = utlocal,
                    utserverwhenloaded = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    isNew = isNew
                };
            }
            else
            {
                mfs.FileName = fileName;
                mfs.MediaFileName = mediaFileName;
                mfs.utlocal = utlocal;
                mfs.utserverwhenloaded = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                mfs.isNew = isNew;
                mfs.toDelete = false;
            }

            var result = db.MediaFiles.Where(x => x.FileName.Equals(mediaFileName));

            if (result.Count() > 0)
            {
                MediaFile mf = db.MediaFiles.Where(x => x.FileName.Equals(mediaFileName)).FirstOrDefault();
                mf.MediaFileSegments.Add(mfs);
            }
            else
            {
                MediaFile mf = new MediaFile { FileName = mediaFileName };
                mf.MediaFileSegments.Add(mfs);
                db.MediaFiles.Add(mf);
            }

            try
            {
                int answer = db.SaveChanges();
                view.printLine("Number of changes executed: " + answer);
            }
            catch (Exception)
            {
                throw;
            }

            return mfs;
        }

 

        internal static void InsertOrUpdateMediaFileSegment(MyDbContext db, ISessionView view, MediaFileSegment mfs)
        {
            InsertOrUpdateMediaFileSegment(db, view, mfs.remote_id, mfs.utlocal, mfs.FileName, mfs.MediaFileName, mfs.isNew);
        }

        internal static void DeleteMediaFile(MyDbContext db, ISessionView view, MediaFile mf)
        {
            foreach(var mfs in mf.MediaFileSegments.ToList())
            {
                db.MediaFileSegments.Remove(mfs);
            }
            db.MediaFiles.Remove(mf);
            db.SaveChanges();
        }
    }
}
