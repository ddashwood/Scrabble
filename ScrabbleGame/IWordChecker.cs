using System;

namespace ScrabbleGame
{
    public interface IWordChecker
    {
        /// <summary>
        /// Check if a word is allowed
        /// </summary>
        /// <param name="word">The word (case insensitive)</param>
        /// <returns>True if the word is allowed; else false</returns>
        bool IsWord(string word);
    }
}
