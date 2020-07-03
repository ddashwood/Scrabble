using System;
using System.Collections.Generic;
using System.Text;

namespace ScrabbleGame
{
    public class WordCheckerFactory : IWordCheckerFactory
    {
        public IWordChecker GetWordChecker()
        {
            return new FileWordChecker();
        }
    }
}
