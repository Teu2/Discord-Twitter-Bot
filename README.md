# Discord-Twitter-Bot
Discord Twitter bot made in C# that can post tweets and get the latest tweets or replies from a given user. (Tokens are not included in this repository for privacy reasons)

###### Tools Used
- Discord.NET
- Twitter API
- Tweetinvi Library

###### Core Commands
```
!newestpost -user
!tweet -message
```

###### All Commands
```
- !commands
- !help
- !newestposts
- !tweet
- !sourcecode
```

### Using The Bot In Your Server
This really isn't intended to be uploaded to top.gg or similar sites since I just wanted to write a small bot (it's also still in development to add more features), but If you want to use this bot and change a few things, create a bot first and invite it to your server, copy this repository and create a Tokens.txt file in the directory path below:
> IGBot\bin\Debug\net6.0

Once you create your Twitter developer account, you need to upgrade to elevated status and give your app read and write permissions. Once done, get your tokens and paste it in the following format below.
```
applicationId,xxx
botToken,xxx
apiKey,xxx
apiKeySecret,xxx
accessToken,xxx
accessTokenSecret,xxx
clientId,xxx
clientSecret,xxx
bearerToken,xxx
```

### Requirments
If you want to work on the bot make sure you install the following packages in your package manager:
- Visual Studio > Tools > NuGet Package Manager > Package Manager Console

Copy and paste the following to install: Discored.NET
```
NuGet\Install-Package Discord.Net -Version 3.9.0
```

Copy and paste the following to install: Tweetinvi Library
```
NuGet\Install-Package TweetinviAPI -Version 5.0.4
```