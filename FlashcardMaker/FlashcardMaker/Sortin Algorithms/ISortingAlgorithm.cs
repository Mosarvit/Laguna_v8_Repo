using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashcardMaker.Models;
using FlashcardMaker.Interfaces;
using FlashcardMaker.Controllers;

namespace FlashcardMaker.Sortin_Algorithms
{
    interface ISortingAlgorithm<T>
    {
        List<T> SortedSubtitleLinePackList();
    }
}
