using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class ChineseWord
    {
        public int id { get; set; }

        //[Index(IsUnique = true)]
        public int position { get; set; }
        
        public string Chinese { get; set; }

        public int knowLevel { get; set; }

        public string PinYin { get; set; }
        
        public string English { get; set; }

        public virtual List<ChineseCharacter> ChineseCharacters { get; set; }
        public virtual List<SubtitleLine> SubtitleLines { get; set; }
        public virtual List<SubtitleLinePack> SubtitleLinePacks { get; set; }
        //public virtual List<Flashcard> Flashcards { get; set; }

        public bool AddedToTempSort { get; set; }

        public int numberOfFlashcardsItsIn { get; set; }
    }
}