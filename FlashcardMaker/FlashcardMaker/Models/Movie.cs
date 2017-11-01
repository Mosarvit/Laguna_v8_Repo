using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class Movie
    {
        public int id { get; set; }

        //[Index(IsUnique = true)]
        [MaxLength(100)]
        public string fileName { get; set; }
        public string fullFileName { get; set; }
        public string fileExtention { get; set; }
        public bool added { get; set; }
        public virtual List<SubtitleLine> SubtitleLines { get; set; }
        public virtual List<SubtitleLinePack> SubtitleLinePacks { get; set; }

        //public ICollection<SubtitleLine> subtitleLines { get; set; }
    }
}
