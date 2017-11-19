using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class PinYin : Model
    {
        [MaxLength(7)]
        [Index("IX_FirstAndSecond", 1, IsUnique = true)]
        public string Plain { get; set; }

        [Index("IX_FirstAndSecond", 2, IsUnique = true)]
        public int Tone { get; set; }

        public string Written { get; set; }

        public List<ChineseCharacter> ChineseCharacter { get; set; }
    }
}


