using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Models
{
    public class Flashcard
    {
        public int id { get; set; }
        
        public int remote_id { get; set; }

        //[Column(TypeName = "NVARCHAR")]
        //[StringLength(255)]
        private string _question { get; set; }
        public string question { get { return _question; } set { utlocal = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); _question = value; } }

        private long _duetime { get; set; }
        public long duetime {
            get { return _duetime; }
            set {
                utlocal = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                _duetime = value; } }

        public long utserverwhenloaded { get; set; }

        public long utlocal { get; set; }

        public bool toDelete { get; set; }

        public void setToDelete(bool toDelete)
        {
            utlocal = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this.toDelete = toDelete;
        }

        public bool getToDelete()
        {
            return toDelete;
        }

        public bool newFc { get; set; }

        //[Column(TypeName = "NVARCHAR")]
        //[StringLength(255)]
        //[Column(TypeName = "NVARCHAR")]
        //public string Question2 { get; set; }

        //public string newdde { get; set; }

        //public virtual SubtitleLinePack SubtitleLinePack { get; set; }


        public override string ToString()
        {
            return  "\r\nid : " + id +
                    "\r\nquestion : " + question +
                    "\r\nduetime : " + duetime +
                    "\r\nutserverwhenloaded : " + utserverwhenloaded +
                    "\r\nutlocal : " + utlocal +
                    "\r\ntoDelete : " + toDelete;
        }

        public static string AllToString()
        {
            StringBuilder sb = new StringBuilder();
            return sb.ToString();
        }
    }    
}
