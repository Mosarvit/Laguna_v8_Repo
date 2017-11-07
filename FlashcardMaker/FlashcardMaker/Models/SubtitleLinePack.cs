using FlashcardMaker.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class SubtitleLinePack : Model, ILinePack
    {

        public virtual List<SubtitleLine> SubtitleLines { get; set; } = new List<SubtitleLine>();
        public virtual List<ChineseWord> ChineseWords { get; set; } = new List<ChineseWord>();
        public virtual List<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
        public int NumberOfNotYetInTempSortWords { get; set; }
        public int NumberOfCharacters { get; set; }
        public int NumberOfToLearnWords { get; set; }
        public double DensityOfToLearnWords { get; set; }
        public bool AddedToFlashcards { get; set; }
        public virtual Movie Movie { get; set; }
        public int EndTime { get; set; }
        public int StartTime { get; set; }
         
        public virtual int MediaFileSegments_remote_id { get; set; } 
    }
}
