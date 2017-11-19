using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class CwInStl : Model
    {
        public string PinYin { get; set; }

        public ChineseWord ChineseWord { get; set; }
    }
}