using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OwnVocabulary
{
    internal static class ParsingHelper
    {
        internal static string ParseToList(string s)
        {
            StringBuilder result = new StringBuilder();

            StringBuilder sb = new StringBuilder(s);
            int i = 0;
            bool openBracket = false;
            int index = 0;
            while (i < sb.Length)
            {
                if (sb[i] == '[')
                {
                    openBracket = true;
                    index = i;
                }
                else if (sb[i] == ']' && openBracket)
                {
                    string str = sb.ToString().Substring(index + 1, i - index - 1);
                    string[] phrases = str.Split(new char[] { ',' }, StringSplitOptions.None);
                    for (int j = 1; j < phrases.Length; j++)
                        result.Append(phrases[j]).Append(", ");
                    openBracket = false;
                }
                i++;
            }
            return result.Remove(result.Length - 2, 2).ToString();
        }

    }
}
