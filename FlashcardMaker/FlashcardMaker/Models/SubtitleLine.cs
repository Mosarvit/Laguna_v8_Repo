using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class SubtitleLine
    {
        public int Id { get; set; }

        //[Index("IX_FirstAndSecond", 1, IsUnique = true)]
        public int Position { get; set; }

        //[Index("IX_FirstAndSecond", 2, IsUnique = true)]
        [MaxLength(100)]
        public String MovieFileName { get; set; }

        public string Chinese { get; set; }
        public string TimeFrameString { get; set; }
        
        public int CanRead { get; set; }

        //public virtual SubtitleLinePack subtitleLinePack { get; set; }

        public virtual List<ChineseWord> ToLearnWords { get; set; }

        public int NumberOfCharacters { get; set; }
        public int NumberOfToLearnWords { get; set; }

    }
}
