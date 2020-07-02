using ScrabbleData;
using ScrabbleGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScrabbleGameTests
{
    public abstract class TestBase
    {
        protected static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        protected Game GetGameWithEmptyBoard()
        {
            string board = new string(' ', Game.BOARD_HEIGHT * Game.BOARD_WIDTH);
            GameData data = new GameData { Board = board };
            return new Game(data);
        }

        protected Game GetGameWithSingleVerticalWord(IWordChecker wordChecker = null)
        {
            string board = string.Concat(Enumerable.Repeat("               ", 3)) +
                           "  T            " + // TEST starts at position 2, 3
                           "  E            " +
                           "  S            " +
                           "  T            " + // and extends to position 2, 6
                           string.Concat(Enumerable.Repeat("               ", 8));
            GameData data = new GameData { Board = board };
            return new Game(data, wordChecker);
        }

        protected Game GetGameWithSingleVerticalWordAtTop()
        {
            string board = "  T            " + // TEST starts at position 2, 0
                           "  E            " +
                           "  S            " +
                           "  T            " + // and extends to position 2, 3
                           string.Concat(Enumerable.Repeat("               ", 11));
            GameData data = new GameData { Board = board };
            return new Game(data);
        }

        protected Game GetGameWithSingleVerticalWordAtBottom()
        {
            string board = string.Concat(Enumerable.Repeat("               ", 11)) +
                           "  T            " + // TEST starts at position 2, 11
                           "  E            " +
                           "  S            " +
                           "  T            ";  // and extends to position 2, 14
            GameData data = new GameData { Board = board };
            return new Game(data);
        }

        protected Game GetGameWithSingleHorizontalWord()
        {
            string board = string.Concat(Enumerable.Repeat("               ", 6)) +
                           "  TEST         " + // TEST starts at position 2, 6
                                               // and extends to position 5, 6
                           string.Concat(Enumerable.Repeat("               ", 8));
            GameData data = new GameData { Board = board };
            return new Game(data);
        }
    }
}
