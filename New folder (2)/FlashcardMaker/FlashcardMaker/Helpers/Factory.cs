using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashcardMaker.Models;
using FlashcardMaker.Views;
using FlashcardMaker.Controllers;
using System.Data.Entity.Migrations;

namespace FlashcardMaker.Helpers
{
    public class Factory
    {
        internal static MediaFileSegment InsertMediaFileSegment(MyDbContext db, ISessionView view, string fileName, string mediaFileName, bool isNew)
        {
            var result = db.MediaFiles.Where(x => x.FileName.Equals(mediaFileName));

            MediaFile mf = db.MediaFiles.Where(x => x.FileName.Equals(mediaFileName)).FirstOrDefault();

            if (mf == null)
            {
                mf = new MediaFile { FileName = mediaFileName };
                db.MediaFiles.Add(mf);
            }


            MediaFileSegment mfs = new MediaFileSegment
            {
                FileName = fileName,
                MediaFileName = mediaFileName,
                utlocal = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                utserverwhenloaded = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                MediaFile = mf,
                isNew = isNew
            };


            //if (db.MediaFileSegments.Where(p=>p.FileName == fileName).Where(p=> p.MediaFileName == mediaFileName)!=null

            db.MediaFileSegments.Add(mfs);
            db.SaveChanges();
            mfs.remote_id = ProgramController.CLIENT_ID + mfs.Id * 1000;
            db.SaveChanges();



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

        internal static Flashcard InsertFlashcard(MyDbContext db, ISessionView view, string question, long duetime, bool isNew)
        {
            Flashcard fc = new Flashcard
            {
                question = question,
                duetime = duetime,
                utlocal = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                utserverwhenloaded = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                isNew = isNew
            };

            db.Flashcards.Add(fc);
            db.SaveChanges();
            fc.remote_id = ProgramController.CLIENT_ID + fc.Id * 1000;
            db.SaveChanges();

            return fc;
        }

        internal static Flashcard InsertOrUpdateFlashcard(MyDbContext db, ISessionView view, int remote_id, long updatetime, long duetime, string question, int MediaFileSegment_remote_id, bool isNew)
        {

            Flashcard fc = new Flashcard
            {
                remote_id = remote_id,
                question = question,
                duetime = duetime,
                utserverwhenloaded = updatetime,
                utlocal = updatetime,
                MediaFileSegment_remote_id = MediaFileSegment_remote_id,
                isNew = isNew
            };

            db.Flashcards.AddOrUpdate(p => new { p.remote_id }, fc);
            db.SaveChanges();

            return fc;
        }


        internal static MediaFileSegment InsertOrUpdateMediaFileSegment(MyDbContext db, ISessionView view, int remote_id, long utlocal, string fileName, string mediaFileName, bool isNew)
        {
            MediaFileSegment mfs = db.MediaFileSegments.Where(c => c.remote_id == remote_id).SingleOrDefault();

            if (mfs == null)
            {
                mfs = new MediaFileSegment
                {
                    FileName = fileName,
                    MediaFileName = mediaFileName,
                    utlocal = utlocal,
                    utserverwhenloaded = utlocal,
                    isNew = isNew
                };

                db.MediaFileSegments.AddOrUpdate(p => new { p.FileName, p.MediaFileName }, mfs);
                db.SaveChanges();
                mfs.remote_id = ProgramController.CLIENT_ID + mfs.Id * 1000;
                db.SaveChanges();
            }
            else
            {
                mfs.FileName = fileName;
                mfs.MediaFileName = mediaFileName;
                mfs.utlocal = utlocal;
                mfs.utserverwhenloaded = utlocal;
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
            foreach (var mfs in mf.MediaFileSegments.ToList())
            {
                DeleteMediaFileSegment(db, view, mfs);
            }
            db.MediaFiles.Remove(mf);
            db.SaveChanges();
        }

        internal static void DeleteMediaFileSegment(MyDbContext db, ISessionView view, MediaFileSegment mfs)
        {
            SubtitleLinePack stlp = db.SubtitleLinePacks.Where(p => p.MediaFileSegments_remote_id == mfs.remote_id).SingleOrDefault();
            if (stlp != null)
            {
                stlp.MediaFileSegments_remote_id = 0;
            }
            db.MediaFileSegments.Remove(entity: (MediaFileSegment)mfs);
            db.SaveChanges();
        }


    }
}
