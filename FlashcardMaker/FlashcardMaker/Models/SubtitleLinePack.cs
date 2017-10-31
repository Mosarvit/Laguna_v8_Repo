using FlashcardMaker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class SubtitleLinePack : ILinePack
    {
        public int Id { get; set; }
        public virtual List<SubtitleLine> SubtitleLines { get; set; }
        public virtual List<ChineseWord> ChineseWords { get; set; }
        public virtual List<Flashcard> Flashcards { get; set; }
        public int NumberOfNotYetInTempSortWords { get; set; }
        public int NumberOfCharacters { get; set; }
        public int NumberOfToLearnWords { get; set; }
        public double DensityOfToLearnWords { get; set; }
        public bool AddedToFlashcards { get; set; }
    }
}
