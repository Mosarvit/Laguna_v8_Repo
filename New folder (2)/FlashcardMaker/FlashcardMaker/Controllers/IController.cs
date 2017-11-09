using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Controllers
{
    public interface IController
    {
        void printLine(string str);
        void printStatusLabel(string v);
    }
}
