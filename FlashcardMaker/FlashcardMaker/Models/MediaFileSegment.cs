using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class MediaFileSegment : OnServerModel
    {
        public string MediaFileName { get; set; }
        public string FileName { get; set; }
        public virtual MediaFile MediaFile { get; set; }
    }
}
