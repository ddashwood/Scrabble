using System;
using System.Collections.Generic;
using System.Text;

namespace ScrabbleGame
{
    public interface IWordCheckerFactory
    {
        IWordChecker GetWordChecker();
    }
}
