using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace OwnVocabulary
{
    internal enum Languages
    {
        /// <summary>
        /// Ukrainian
        /// </summary>
        [LanguageValue("Ukrainian")]
        uk,
        /// <summary>
        /// Russian
        /// </summary>
        [LanguageValue("Russian")]
        ru,
        /// <summary>
        /// English
        /// </summary>
        [LanguageValue("English")]
        en,
        /// <summary>
        /// Dutch
        /// </summary>
        [LanguageValue("Dutch")]
        de
    }

    internal class Translator
    {
        private string strRequest = @"http://translate.google.com/translate_a/t?client=t&text={0}&tl={1}&multires=1&otf=2&pc=0&sc=1";

        private string lang = "uk";
        public string Lang
        {
            get { return lang; }
            set { lang = value; }
        }
        
        private HttpWebResponse wresScrape;

        private string phrase;
        public string Phrase
        {
            get { return phrase; }
            set { phrase = value; }
        }

        private string translatedPhrase;
        public string TranslatedPhrase
        {
            get { return translatedPhrase; }
        }

        private List<TranslatedItem> translated;
        public List<TranslatedItem> Translated
        {
            get { return translated; }
        }
        
        public Translator()
        {
            this.phrase = "";
            translated = new List<TranslatedItem>();
        }

        public void Translate(string phrase, Languages lang)
        {
            this.lang = lang.ToString();
            translatedPhrase = string.Empty;
            translated.Clear();
            this.phrase = phrase;

            bool exist;
            string strResult = ConvertStreamTostring(GetHttpStream(string.Format(strRequest, phrase, lang)));
            string innerBlock = GetBlock(strResult, 2, 2, out exist);
            if (exist)
            {
                int index = 0;
                do
                {
                    index++;
                    string str = GetBlock(innerBlock, 1, index, out exist);
                    if (exist)
                    {
                        string type = GetTypeOfPhrase(ref str);
                        bool flag;
                        str = GetBlock(str, 1, 1, out flag);
                        if (flag)
                            translated.Add(new TranslatedItem(type, GetTranslatedPhrases(str)));
                    }
                }
                while (exist);
            }
            else
            {
                // maybe only one phrase

                string str = GetBlock(strResult, 3, 1, out exist);
                if (exist)
                {
                    str = str.Replace('"', ',');
                    string[] phrases = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (phrases.Length > 1 && phrases[0] != phrases[1])
                        translatedPhrase = phrases[0];
                }
            }
        }

        #region Parsing response from google

        private string GetTypeOfPhrase(ref string s)
        {
            string result = "";
            int index = s.IndexOf(',');
            result = s.Substring(1, index - 2);
            s = s.Remove(0, index + 1);
            return result;
        }

        private List<string> GetTranslatedPhrases(string s)
        {
            s = s.Replace('"', ',');
            string[] phrases = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return phrases.ToList<string>();
        }
        /// <summary>
        /// Find block by index 
        /// </summary>
        /// <param name="s">String</param>
        /// <param name="block">Block ID</param>
        /// <param name="index">Index</param>
        /// <returns></returns>
        private string GetBlock(string s, int block, int index, out bool exist)
        {
            exist = false;
            string result = string.Empty;

            s = s.Trim();

            Stack<int> blockStartFrom = new Stack<int>();
            int i = 0;
            int length = s.Length;

            while (i < length)
            {
                if (s[i] == '[')
                    blockStartFrom.Push(i);
                else if (s[i] == ']')
                {
                    if (blockStartFrom.Count == block)
                        index--;

                    int popIndex = blockStartFrom.Pop();
                    if (index == 0)
                    {
                        result = s.Substring(popIndex + 1, i - popIndex - 1);
                        exist = true;
                        break;
                    }
                }
                else if (s[i] == ',' && s[i - 1] == ',')
                    break;
                i++;
            }

            return result;
        }
        #endregion

        private string ConvertStreamTostring(Stream stmSource)
        {
            StreamReader sr = null;
            if (stmSource != null)
            {
                try
                {
                    sr = new StreamReader(stmSource);
                    return sr.ReadToEnd();
                }
                catch
                {
                    throw new Exception();
                }
                finally
                {
                    wresScrape.Close();
                    sr.Close();
                }
            }
            else
                return null;
        }

        private Stream GetHttpStream(string url)
        {
            HttpWebRequest wreqScrape = (HttpWebRequest)(WebRequest.Create(url));
            wreqScrape.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0b; Windows NT 5.1)";
            wreqScrape.Method = "GET";
            wreqScrape.Timeout = 10000;
            try
            {
                wresScrape = (HttpWebResponse)(wreqScrape.GetResponse());
                return wresScrape.GetResponseStream();
            }
            catch
            {
                throw new Exception("Please check your connection to the Internet and try again.");
            }
        }

    }
}
