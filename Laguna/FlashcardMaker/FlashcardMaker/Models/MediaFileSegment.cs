using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class MediaFileSegment : OnServerModel
    {
        [MaxLength(100)]
        [Index("IX_FirstAndSecond", 1, IsUnique = true)]
        public string MediaFileName { get; set; }

        [MaxLength(100)]
        [Index("IX_FirstAndSecond", 2, IsUnique = true)]
        public string FileName { get; set; }
        public virtual MediaFile MediaFile { get; set; } 

        public virtual SubtitleLinePack SubtitleLinePack { get; set; }

        public bool ToUpload { get; set; } = false;
        public bool ToDownload { get; set; } = false;

    }
}
