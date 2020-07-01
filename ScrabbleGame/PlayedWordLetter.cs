namespace ScrabbleGame
{
    internal class PlayedWordLetter
    {
        public char Tile { get; }
        public Multiplier Multiplier { get; }

        public PlayedWordLetter(char tile, int x, int y)
        {
            Tile = tile;
            Multiplier = Game.SquareMultiplier(x, y);
        }
        public PlayedWordLetter(char tile)
        {
            Tile = tile;
            Multiplier = Multiplier.None;
        }
    }
}