using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class NoneChineseCharacter
    {
        public int id { get; set; }

        //[Index(IsUnique = true)]
        [MaxLength(10)]
        public string Character { get; set; }
    }
}
