using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardMaker.Views
{
    public interface ISessionView
    {
        void printLine(string v);
        void printStatusLabel(string v);
        void refresh();
    }
}
