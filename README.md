***Scrabble***

A Blazor implementation of the classic word game.

**Installation**

- In Package Manager Console, run `update-database`
- Deploy to a web server
  - Don't forget to deploy Scrabble.db
- You're good to go!

**Known issues**

- Error handling is minimal
- Need to include a key pair for Identity Server
- There are no tests for the front end
- Notifications of player doing something get sent to *every* 
player, not just the opponent of the player who played
- Currently, we use SqLite. Could probably be changed to SQL
Server easily enough if needed

**Credits**

Word list from <a href="https://www.wordgamedictionary.com/sowpods/download/sowpods.txt">WordGameDictionary.com</a>

Icons made by <a href="https://www.flaticon.com/authors/freepik" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/" title="Flaticon"> www.flaticon.com</a>