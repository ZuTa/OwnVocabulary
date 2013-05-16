using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OwnVocabulary
{
    public class LanguageValueAttribute : Attribute
    {
        public string StringValue { get; protected set; }

        public LanguageValueAttribute(string value)
        {
            this.StringValue = value;
        }
    }
}
