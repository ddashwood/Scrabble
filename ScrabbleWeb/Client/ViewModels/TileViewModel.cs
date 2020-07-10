using ScrabbleMoveChecker;
using ScrabbleWeb.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.ViewModels
{
    public class TileViewModel
    {
        public ITilePosition Position { get; set; }
        public string Contents { get; set; }
        public int? Score { get; set; }
        public bool Clickable { get; set; }
        public bool Selected { get; set; }
        public bool PartOfLastMove { get; set; }
        public bool Valid { get; set; }
        public bool Invalid { get; set; }
        public bool Centre { get; set; }

        public char[] MultiplierLetters { get; private set; }
        private string multiplierClass;
        private Multiplier multiplier;
        public Multiplier Multiplier
        {
            get { return multiplier; }
            set
            {
                multiplier = value;
                // Multipliers only affect tiles that aren't played on yet
                switch (multiplier)
                {
                    case Multiplier.DoubleLetter:
                        (multiplierClass, MultiplierLetters) = (" double-letter", new char[] { 'D', 'L' });
                        break;
                    case Multiplier.TrippleLetter:
                        (multiplierClass, MultiplierLetters) = (" tripple-letter", new char[] { 'T', 'L' });
                        break;
                    case Multiplier.DoubleWord:
                        (multiplierClass, MultiplierLetters) = (" double-word", new char[] { 'D', 'W' });
                        break;
                    case Multiplier.TrippleWord:
                        (multiplierClass, MultiplierLetters) = (" tripple-word", new char[] { 'T', 'W' });
                        break;
                    default:
                        (multiplierClass, MultiplierLetters) = ("", new char[] { ' ', ' ' });
                        break;
                }
            }
        }
        public Func<Task> OnClickCallback { get; set; }

        public string TileSpaceClass
        {
            get
            {
                string result = "tile-space";
                if (Contents == null)
                {
                    result += multiplierClass;
                }
                if (Selected)
                {
                    result += " tile-selected";
                }
                if (Clickable)
                {
                    result += " clickable";
                }

                return result;
            }
        }
        public string TileContainerClass
        {
            get
            {
                if (Contents == null && Multiplier == Multiplier.None && !Centre)
                {
                    // No need for a container if there are no contents
                    return "";
                }

                string result = "tile-container";
                if (PartOfLastMove)
                {
                    result += " tile-last-move";
                }
                if (Valid)
                {
                    result += " tile-valid";
                }
                if (Invalid)
                {
                    result += " tile-invalid";
                }
                if (Centre && Contents == null)
                {
                    result += " tile-container-centre";
                }

                return result;
            }
        }

        public async Task OnClick()
        {
            if(Clickable)
            {
                await OnClickCallback();
            }
        }
    }
}
