using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashcardMaker.Models;
using FlashcardMaker.Controllers;
using FlashcardMaker.Interfaces;

namespace FlashcardMaker.Helpers
{
    public class FlashcardCreator
    {
        internal void CreateFlashcards(List<ILinePack> tempSortSubtitleLinePackList, IController controller)
        {
            using (MyDbContext db = new MyDbContext())
            {
                controller.printLine("Creating Flashcards");

                foreach (var stlp in tempSortSubtitleLinePackList)
                {
                    Flashcard fc = new Flashcard { };
                    //fc.SubtitleLinePack = stlp;
                    db.Flashcards.Add(fc);
                }

                db.SaveChanges();

                controller.printLine("Done creating Flashcards");
            }                
        }
    }
}
