using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OwnVocabulary
{
    public class TranslatedItem
    {
        private string type;
        public string Type
        {
            get { return type; }
        }

        private List<string> phrases;
        public List<string> Phrases
        {
            get { return phrases; }
        }

        public TranslatedItem(string type, List<string> phrases)
        {
            this.type = type;
            this.phrases = phrases;
        }

    }
}
