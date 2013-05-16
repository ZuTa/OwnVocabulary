using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OwnVocabulary.Testing
{
    public class TestItem
    {
        private string phrase;
        public string Phrase
        {
            get { return phrase; }
            set { phrase = value; }
        }

        private string translate;
        public string Translate
        {
            get { return translate; }
            set { translate = value; }
        }

        public TestItem()
        {
        }

        public TestItem(string phrase, string translate)
        {
            this.phrase = phrase;
            this.translate = translate;
        }
    }
}
