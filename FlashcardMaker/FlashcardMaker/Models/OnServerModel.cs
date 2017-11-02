using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public abstract class OnServerModel : Model
    { 
        public int remote_id { get; set; }

        public long utserverwhenloaded { get; set; }

        public long utlocal { get; set; }

        public bool toDelete { get; set; }

        public bool isNew { get; set; }

        public virtual List<MediaFileSegment> MediaFileSegments { get; set; }
    }
}
