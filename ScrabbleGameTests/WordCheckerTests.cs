using ScrabbleGame;
using System;
using System.IO;
using Xunit;

namespace ScrabbleGameTests
{
    public class WordCheckerTests : TestBase
    {
        [Fact]
        public void IsWordTest()
        {
            FileWordChecker wordChecker;
            string words = $"CAT{Environment.NewLine}RABBIT{Environment.NewLine}";
            using(Stream wordsStream = GenerateStreamFromString(words))
            {
                wordChecker = new FileWordChecker(wordsStream);
                Assert.True(wordChecker.IsWord("cat"));
            }

            // We should still be able to check words even after the stream is destroyed
            Assert.True(wordChecker.IsWord("cat"));
            Assert.True(wordChecker.IsWord("rabbit"));
            Assert.False(wordChecker.IsWord("dog"));
            Assert.False(wordChecker.IsWord("rab"));
        }
    }
}
