using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class MediaFile : Model
    { 
        public string FileName { get; set; }
        public virtual List<MediaFileSegment> MediaFileSegments { get; set; } = new List<MediaFileSegment>();
        
    }
}
