using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OwnVocabulary
{
    public class VocabularyItem
    {
        private string name;
        public string Name
        {
            get { return name; }
            set 
            { 
                name = value;
            }
        }

        private int languageID;
        public int LanguageID
        {
            get { return languageID; }
            set { languageID = value; }
        }


        private int id;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public VocabularyItem(int id, string name, int languageID)
        {
            this.id = id;
            this.name = name;
            this.languageID = languageID;
        }

        public override string ToString()
        {
            return this.name;
        }

    }
}
