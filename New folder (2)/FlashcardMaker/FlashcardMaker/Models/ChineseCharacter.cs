using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class ChineseCharacter : Model
    {   
        public string Chinese { get; set; }

        public int Rank { get; set; }

        public int KnowLevel { get; set; }

        public string PinYin { get; set; }

        public string English { get; set; }

        public virtual List<ChineseWord> ChineseWords { get; set; }

        //public ICollection<SubtitleLine> subtitleLines { get; set; }
    }
}
