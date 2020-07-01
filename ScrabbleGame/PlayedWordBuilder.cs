using System;
using System.Collections.Generic;
using System.Text;

namespace ScrabbleGame
{
    class PlayedWordBuilder
    {
        List<PlayedWordLetter> playedWordLetters = new List<PlayedWordLetter>();

        public void AddBoardTileAtStart(PlayedWordLetter letter)
        {
            playedWordLetters.Insert(0, letter);
        }

        public void AddBoardTileAtEnd(PlayedWordLetter letter)
        {
            playedWordLetters.Add(letter);
        }

        public PlayedWord GetPlayedWord()
        {
            return new PlayedWord(playedWordLetters);
        }
    }
}
