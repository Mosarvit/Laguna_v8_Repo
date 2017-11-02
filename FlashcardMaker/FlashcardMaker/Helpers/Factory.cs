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
        internal static MediaFileSegment CreateMediaFileSegment(MyDbContext db, ISessionView view, string fileName, string mediaFileName)
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
                mfs.MediaFileSegments.Add(mfs);
            }
            else
            {
                MediaFile mf = new MediaFile { FileName = fileName };
                mfs.MediaFileSegments.Add(mfs);
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
    }
}
