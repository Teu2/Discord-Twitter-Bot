using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using Discord.Commands;
using Discord;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Auth;

namespace IGbot // Note: actual namespace depends on the project name.
{
    internal class Program 
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
       
        private DiscordSocketClient? _client;
        
        private CommandService? _commands;
        private IServiceProvider _services;

        private string? _botToken; // discord bot token access token
        private ulong? _channelId; // discord channel to post in
        private string? applicationId;

        // twitter tokens
        private string? _apiKey;
        private string? _apiSecretKey;
        private string? _accessToken;
        private string? _accessTokenSecret;

        private TwitterClient? _userClient;

        public async Task MainAsync()
        {
            var config = new DiscordSocketConfig 
            { 
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent 
            };

            _client = new DiscordSocketClient(config);
            _client.MessageReceived += _client_MessageReceived;
            _client.Log += Log;
            _commands = new();

            var httpClient = new HttpClient();

            // adding all required tokens & authentication requirments
            Dictionary<string, string> tokens = new();
            string[] readLines = File.ReadAllLines(@"Tokens.txt");

            foreach (var line in readLines)
            {
                string[] delimeter = line.Split(',');
                string key = delimeter[0].Trim();
                string val = delimeter[1].Trim();

                tokens.Add(key, val);
            }

            // assign tokens
            _botToken = tokens["botToken"];
            _channelId = ulong.Parse(tokens["discChannel"]);
            _apiKey = tokens["apiKey"];
            _apiSecretKey = tokens["apiKeySecret"];
            _accessToken = tokens["accessToken"];
            _accessTokenSecret = tokens["accessTokenSecret"];

            _userClient = new TwitterClient(_apiKey, _apiSecretKey, _accessToken, _accessTokenSecret);

            // request the user's information from Twitter API
            var user = await _userClient.Users.GetAuthenticatedUserAsync();
            Console.WriteLine("Hello " + user);

            await _client.LoginAsync(TokenType.Bot, _botToken);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task _client_MessageReceived(SocketMessage arg)
        {
            Console.WriteLine($"{arg.Author} sent '{arg.Content}' in {arg.Channel}");
            if (arg.Author.IsBot) return Task.CompletedTask;

            switch (arg.Content.ToLower()) // commands
            {
                case "good bot": arg.Channel.SendMessageAsync($"Thank you ＝( ^o^)ノ"); return Task.CompletedTask;
                case "!commands": arg.Channel.SendMessageAsync($"List of my commands: \n- !commands\n- !help\n- !newestposts\n- !tweet\n- !sourcecode"); return Task.CompletedTask;
                case "!help": arg.Channel.SendMessageAsync($"To get a tweet, type with the single quotes: !newestpost -twitterUser\nTo tweet something, type with single quotes: !tweet -message"); return Task.CompletedTask;
                case "!sourcecode": arg.Channel.SendMessageAsync($"https://github.com/Teu2/Discord-Twitter-Bot"); return Task.CompletedTask;
            };

            if (arg.Content.ToLower().Contains("!tweet")) // sending a tweet
            {
                string message = CleanCommand(arg.Content);
                Console.WriteLine($"message: {message}");

                var tweet = _userClient.Tweets.PublishTweetAsync(message);
                var postedTweet = _userClient.Tweets.GetTweetAsync(tweet.Id);

                Console.WriteLine("You published the tweet : " + tweet);
                arg.Channel.SendMessageAsync($"Tweeted! {postedTweet.Id}"); return Task.CompletedTask;
            }

            if (arg.Content.ToLower().Contains("!newestpost")) // sending a tweet
            {
                // Get the tweets available on the user's home page
                var homeTimeline = _userClient.Timelines.GetHomeTimelineAsync();

                string twitterAccount = CleanCommand(arg.Content);
                Console.WriteLine($"@ => {twitterAccount}");

                try // Get tweets from a specific user
                {
                    var userTimeline = _userClient.Timelines.GetUserTimelineAsync(twitterAccount);
                    if (userTimeline.Result[0].Url == null) arg.Channel.SendMessageAsync($"Sorry :c, That user doesn't exist or doesn't have any posts");

                    var link = userTimeline.Result[0].Url;
                    var item = userTimeline.Result[0].FullText;
                    var test = userTimeline.Result[0].Hashtags;

                    arg.Channel.SendMessageAsync($"{link}");

                    return Task.CompletedTask;
                }
                catch (System.IndexOutOfRangeException ex)
                {
                    Console.WriteLine("Exception " + ex.Message);
                    arg.Channel.SendMessageAsync($"Sorry :c, That user doesn't exist or doesn't have any posts");
                }
            }
            return Task.CompletedTask;
        }

        private string CleanCommand(string command)
        {
            string input = command;
            int index = 0;

            if (command.Contains('-')) index = input.LastIndexOf('-');

            string message = input.Substring(index + 1);
            Console.WriteLine($"message: {message}");

            return message;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
