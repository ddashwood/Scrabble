using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using static ScrabbleMoveChecker.Multiplier;

[assembly:InternalsVisibleTo("ScrabbleGameTests")]

namespace ScrabbleMoveChecker
{
    /// <summary>
    /// Represents a Scrabble game. When referring to a square on the board,
    /// ' ' means the square is empty, lower case means a blank tile has been
    /// used, upper case means a regular tile has been used. X/Y coordinates are
    /// zero-based, with (0,0) being to the top-left corner of the board.
    /// </summary>
    public class GameBase
    {
        private static readonly ReadOnlyDictionary<char, int> letterScores = new ReadOnlyDictionary<char, int>
            (new Dictionary<char, int>
            {
                ['A'] = 1,  ['B'] = 3, ['C'] = 3,  ['D'] = 2,
                ['E'] = 1,  ['F'] = 4, ['G'] = 2,  ['H'] = 4,
                ['I'] = 1,  ['J'] = 8, ['K'] = 5,  ['L'] = 1,
                ['M'] = 3,  ['N'] = 1, ['O'] = 1,  ['P'] = 3,
                ['Q'] = 10, ['R'] = 1, ['S'] = 1,  ['T'] = 1,
                ['U'] = 1,  ['V'] = 4, ['W'] = 4,  ['X'] = 8,
                ['Y'] = 4,  ['Z'] = 10
            });
        private static readonly ReadOnlyDictionary<(int x, int y), Multiplier> multipliers = new ReadOnlyDictionary<(int x, int y), Multiplier>
            (new Dictionary<(int x, int y), Multiplier>
            {
                [(0,0)] = TrippleWord, [(3,0)] = DoubleLetter, [(7,0)] = TrippleWord, [(11,0)] = DoubleLetter, [(14,0)] = TrippleWord,
                [(1,1)] = DoubleWord, [(5,1)] = TrippleLetter, [(9,1)] = TrippleLetter, [(13,1)] = DoubleWord,
                [(2,2)] = DoubleWord, [(6,2)] = DoubleLetter, [(8,2)] = DoubleLetter, [(12,2)] = DoubleWord,
                [(0,3)] = DoubleLetter, [(3,3)] = DoubleWord, [(7,3)] = DoubleLetter, [(11,3)] = DoubleWord, [(14,3)] = DoubleLetter,
                [(4,4)] = DoubleWord, [(10,4)] = DoubleWord,
                [(1,5)] = TrippleLetter, [(5,5)] = TrippleLetter, [(9,5)] = TrippleLetter, [(13,5)] = TrippleLetter,
                [(2,6)] = DoubleLetter, [(6,6)] = DoubleLetter, [(8, 6)] = DoubleLetter, [(12,6)] = DoubleLetter,
                [(0,7)] = TrippleWord, [(3, 7)] = DoubleLetter, [(11,7)] = DoubleLetter, [(14,7)] = TrippleWord,
                [(2,8)] = DoubleLetter, [(6,8)] = DoubleLetter, [(8, 8)] = DoubleLetter, [(12,8)] = DoubleLetter,
                [(1,9)] = TrippleLetter, [(5,9)] = TrippleLetter, [(9,9)] = TrippleLetter, [(13,9)] = TrippleLetter,
                [(4,10)] = DoubleWord, [(10,10)] = DoubleWord,
                [(0,11)] = DoubleLetter, [(3,11)] = DoubleWord, [(7,11)] = DoubleLetter, [(11,11)] = DoubleWord, [(14,11)] = DoubleLetter,
                [(2,12)] = DoubleWord, [(6,12)] = DoubleLetter, [(8,12)] = DoubleLetter, [(12,12)] = DoubleWord,
                [(1,13)] = DoubleWord, [(5,13)] = TrippleLetter, [(9,13)] = TrippleLetter, [(13,13)] = DoubleWord,
                [(0,14)] = TrippleWord, [(3,14)] = DoubleLetter, [(7,14)] = TrippleWord, [(11,14)] = DoubleLetter, [(14,14)] = TrippleWord,
            });


        public const int BOARD_WIDTH = 15;
        public const int BOARD_HEIGHT = 15;

        protected string board;

        public GameBase(string board)
        {
            this.board = board;
        }

        public static int LetterScore(char tile)
        {
            if (letterScores.TryGetValue(tile, out int score))
            {
                return score;
            }
            return 0;
        }
        public static Multiplier SquareMultiplier(int x, int y)
        {
            if (multipliers.TryGetValue((x, y), out Multiplier result))
            {
                return result;
            }
            return Multiplier.None;
        }

        public char this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= BOARD_WIDTH)
                    throw new ArgumentOutOfRangeException(nameof(x), x, null);
                if (y < 0 || y >= BOARD_WIDTH)
                    throw new ArgumentOutOfRangeException(nameof(y), y, null);

                int index = y * BOARD_WIDTH + x;
                return board[index];
            }
        }

        internal bool IsFirstMove() => board.All(c => c == ' ');
    }
}
