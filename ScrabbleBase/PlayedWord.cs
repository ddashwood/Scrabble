using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrabbleBase
{
    public class PlayedWord
    {
        List<PlayedWordLetter> playedWordLetters;

        public PlayedWord(List<PlayedWordLetter> playedWordLetters)
        {
            this.playedWordLetters = playedWordLetters;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var letter in playedWordLetters)
            {
                sb.Append(letter.Tile);
            }

            return sb.ToString();
        }

        public int Score
        {
            get
            {
                int score = 0;
                int wordMultiplier = 1;

                foreach (var letter in playedWordLetters)
                {
                    int letterScore = GameBase.LetterScore(letter.Tile);
                    if (letter.Multiplier == Multiplier.DoubleLetter)
                    {
                        letterScore *= 2;
                    }
                    else if (letter.Multiplier == Multiplier.TrippleLetter)
                    {
                        letterScore *= 3;
                    }
                    else if (letter.Multiplier == Multiplier.DoubleWord)
                    {
                        wordMultiplier *= 2;
                    }
                    else if (letter.Multiplier == Multiplier.TrippleWord)
                    {
                        wordMultiplier *= 3;
                    }

                    score += letterScore;
                }

                return score * wordMultiplier;
            }
        }
    }

    static class PlayedWordExtensions
    {
        public static List<string> ToStringList(this IEnumerable<PlayedWord> words)
        {
            return words.Select(w => w.ToString()).ToList();
        }
    }
}
