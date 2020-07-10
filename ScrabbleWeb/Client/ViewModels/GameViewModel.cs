using Microsoft.JSInterop;
using ScrabbleBase;
using ScrabbleWeb.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.ViewModels
{
    class GameViewModel
    {
        public Game Game { get; set; }
        public ITilePosition TileBeingMoved { get; set; }
        public bool IsValidMove { get; set; }
        public string Message { get; set; }
        public string MessageBootstrapContext { get; set; }
        public bool IsSwapping { get; private set; }
        public bool[] SwappingTiles { get; private set; }

        public bool CanSwap => Game.TilesRemaining > 0;

        private Func<Task<char>> blankTileGetter;
        private Action stateHasChanged;
        private IJSRuntime jsRuntime;

        public GameViewModel(Game game, Func<Task<char>> blankTileGetter, Action stateHasChanged, IJSRuntime jsRuntime)
        {
            Game = game;
            this.blankTileGetter = blankTileGetter;
            this.stateHasChanged = stateHasChanged;
            this.jsRuntime = jsRuntime;
        }

        public void StartSwapping()
        {
            SwappingTiles = new bool[Game.MyTiles.Length];
            IsSwapping = true;
        }

        public void StopSwapping()
        {
            IsSwapping = false;
        }

        public TileViewModel GetBoardTile(int x, int y)
        {
            // Display either a tile from the already-played board, or one which the user has
            // placed on the board as part of this move
            char boardTile = Game[x, y];
            char userTile = Game.Move[x, y];
            char? contents = (boardTile, userTile) switch
            {
                (char tile, _) when tile != ' ' => tile,
                (_, char tile) when tile != ' ' => tile,
                _ => null
            };
            var position = new BoardPosition(Game, x, y, blankTileGetter);

            return new TileViewModel
            {
                Position = position,
                Contents = contents?.ToString()?.ToUpper(),
                Score = contents switch
                {
                    null => null,
                    char tile when tile >= 'A' && tile <= 'Z' => GameBase.LetterScore(tile),
                    _ => null
                },
                Multiplier = GameBase.SquareMultiplier(x, y),
                // Clickable - always false if we're swapping
                // If we're not swapping, then if no tile is being moved, only allow clicking on user-placed tiles
                //                        if a tile *is* being moved, only allow clicking on blank squares, or on the tile being moved
                Clickable = !IsSwapping && ((TileBeingMoved == null) ? userTile != ' ' : (contents == null || TileBeingMoved.Equals(position))),
                Selected = position.Equals(TileBeingMoved),
                PartOfLastMove = Game.LastMoveTiles.Exists(t => (t.X, t.Y) == (x, y)),
                Valid = userTile != ' ' && IsValidMove,
                Invalid = userTile != ' ' && !IsValidMove,
                Swapping = false,
                Centre = x == (GameBase.BOARD_WIDTH - 1) / 2 && y == (GameBase.BOARD_HEIGHT - 1) / 2,
                OnClickCallback = () => ClickOnSpace(position)
            };
        }

        public TileViewModel GetRackTile(int space)
        {
            char? contents = Game.MyTiles[space] switch
            {
                ' ' => null,
                '*' => ' ',
                char tile => tile
            };
            var position = new RackPosition(Game, space);


            bool clickable;
            if (TileBeingMoved == null)
            {
                if (IsSwapping)
                {
                    // If we're in swapping mode, allow clicking anywhere there is a tile -
                    // but only if there are enough tiles remaining.
                    // If there aren't enough tiles, only allow clicking on tiles areadly
                    // selected, so they can be de-selected
                    clickable = contents != null &&
                        (SwappingTiles.Count(t => t) < Game.TilesRemaining || SwappingTiles[space]);
                }
                else
                {
                    // If we're not in swapping mode, and no tile is being moved, allow clicking
                    // anywhere there is a tile
                    clickable = contents != null;
                }
            }
            else
            {
                if (TileBeingMoved is BoardPosition)
                {
                    // If a tile *is* being moved, and it's from the board, only allow it to be
                    // dropped where there's a space
                    clickable = contents == null;
                }
                else
                {
                    // If a tile is being moved and it's from the rack, it can be dropped anywhere
                    // on the rack
                    clickable = true;
                }
            }

            return new TileViewModel
            {
                Position = position,
                Contents = contents?.ToString(),
                Score = contents switch
                {
                    null => null,
                    char tile when tile >= 'A' && tile <= 'Z' => GameBase.LetterScore(tile),
                    _ => null
                },
                Multiplier = Multiplier.None,
                Clickable = clickable,
                Selected = position.Equals(TileBeingMoved),
                PartOfLastMove = false,
                Valid = false,
                Invalid = false,
                Swapping = IsSwapping && SwappingTiles[space],
                Centre = false,
                OnClickCallback = () => ClickOnSpace(position)
            };
        }

        public async Task ClickOnSpace(ITilePosition position)
        {
            if (IsSwapping && position is RackPosition rackPosition)
            {
                SwappingTiles[rackPosition.Space] = !SwappingTiles[rackPosition.Space];
            }
            else if(TileBeingMoved == null)
            {
                // Start moving a tile
                TileBeingMoved = position;
            }
            else if (TileBeingMoved.Equals(position))
            {
                // User has clicked back on the tile being moved. Stop moving it
                TileBeingMoved = null;

                // If it was a blank tile, the user may want to change its letter
                if (position.GetTile() <= 'A' || position.GetTile() >= 'Z')
                {
                    // Moving the tile back to its own position allows the user to choose a letter
                    // if the tile is on the board
                    await Game.MoveTile(position, position);
                }
            }
            else if (TileBeingMoved is RackPosition from && position is RackPosition to && to.GetTile() != ' ')
            {
                // If moving tiles on the rack but into a blank space, this won't apply,
                // and the next block of code will just move the tile.
                // But if moving to a non-blank space, this will shuffle the intermmediate
                // tiles along to make space.
                Game.MoveTilesOnRack(from, to);
                TileBeingMoved = null;
            }
            else
            {
                await Game.MoveTile(TileBeingMoved, position);
                TileBeingMoved = null;
            }

            await jsRuntime.InvokeVoidAsync("scrollPreventer.saveTop", "game");
            UpdateMessage();
            stateHasChanged();
            await jsRuntime.InvokeVoidAsync("scrollPreventer.restoreTop", "game");
        }

        public void UpdateMessage()
        {
            if (Game.Move.Placements.Count() == 0)
            {
                // If there are no tiles placed, ignore any messages from the server
                // and remove any previous messages
                Message = "";
                return;
            }

            int score = Game.Move.GetScore(out string error);
            IsValidMove = string.IsNullOrEmpty(error);
            if (IsValidMove)
            {
                Message = "Score: " + score;
                MessageBootstrapContext = "primary";
            }
            else
            {
                Message = error;
                MessageBootstrapContext = "danger";
            }
        }

    }
}
