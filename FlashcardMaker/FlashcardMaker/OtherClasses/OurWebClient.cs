using System;
using System.Net;

namespace FlashcardMaker.Controllers
{
    internal class OurWebClient : WebClient
    {
        public OurWebClient()
        {
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 10 * 1000;
            return w;
        }
    }
}