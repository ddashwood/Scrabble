using IdentityServer4.Test;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ScrabbleData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly:InternalsVisibleTo("ScrabbleGameTests")]

namespace ScrabbleGame
{
    /// <summary>
    /// Represents a Scrabble game. When referring to a square on the board,
    /// ' ' means the square is empty, lower case means a blank tile has been
    /// used, upper case means a regular tile has been used. X/Y coordinates are
    /// zero-based, with (0,0) being to the top-left corner of the board.
    /// </summary>
    public class Game
    {
        public const int BOARD_WIDTH = 15;
        public const int BOARD_HEIGHT = 15;

        private readonly GameData gameData;
        public Game(GameData gameData)
        {
            this.gameData = gameData ?? throw new ArgumentNullException(nameof(gameData));
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
                return gameData.Board[index];
            }

            internal set
            {
                if (x < 0 || x >= BOARD_WIDTH)
                    throw new ArgumentOutOfRangeException(nameof(x), x, null);
                if (y < 0 || y >= BOARD_WIDTH)
                    throw new ArgumentOutOfRangeException(nameof(y), y, null);

                char[] spaces = gameData.Board.ToCharArray();
                int index = y * BOARD_WIDTH + x;
                spaces[index] = value;
                gameData.Board = new string(spaces);
            }
        }
    }
}
