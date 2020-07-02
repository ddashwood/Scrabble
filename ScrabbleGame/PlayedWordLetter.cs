namespace ScrabbleGame
{
    internal class PlayedWordLetter
    {
        public char Tile { get; }
        public Multiplier Multiplier { get; private set; }

        private PlayedWordLetter(char tile)
        {
            Tile = tile;
            Multiplier = Multiplier.None;
        }

        public static PlayedWordLetter Create(char tile)
        {
            return new PlayedWordLetter(tile);
        }

        public static PlayedWordLetter CreateWithBoardMultiplier(char tile, int x, int y)
        {
            return new PlayedWordLetter(tile) { Multiplier = Game.SquareMultiplier(x, y) };
        }
    }
}