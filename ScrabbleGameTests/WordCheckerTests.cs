using ScrabbleGame;
using System;
using System.IO;
using Xunit;

namespace ScrabbleGameTests
{
    public class WordCheckerTests
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

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
