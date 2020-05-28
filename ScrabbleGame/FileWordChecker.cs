using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScrabbleGame
{
    /// <summary>
    /// Used to check if a word is allowed
    /// </summary>
    public class FileWordChecker : IWordChecker
    {
        private HashSet<string> words;
        private Stream wordsStream;

        /// <summary>
        /// Create a FileWordChecker with the default SOWPODS word list
        /// </summary>
        public FileWordChecker()
        {
        }

        /// <summary>
        /// Create a FileWordChecker with a custom word list. Useful for unit testing
        /// </summary>
        /// <param name="wordsStream">A stream containing the word list</param>
        public FileWordChecker(Stream wordsStream)
        {
            this.wordsStream = wordsStream;
        }

        public bool IsWord(string word)
        {
            if (words == null)
            {
                LoadWords();
            }

            return words.Contains(word.ToUpper());
        }

        private void LoadWords()
        {
            words = new HashSet<string>();

            using(StreamReader sr = wordsStream != null ? new StreamReader(wordsStream) : new StreamReader( "SOWPODS.txt"))
            {
                while(!sr.EndOfStream)
                {
                    string word = sr.ReadLine();
                    words.Add(word);
                }
            }
        }
    }
}
