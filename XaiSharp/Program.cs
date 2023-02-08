using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using DontPanic.TumblrSharp;
using DontPanic.TumblrSharp.Client;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.Data.SQLite;

namespace XaiSharp
{
    public class Config
    {
        public ulong BotOwnerId { get; set; }
        public string Token { get; set; }
        public ulong WebhookId { get; set; }
        public string WebhookToken { get; set; }
        public string SQLiteDatabase { get; set; }
        public string Repository { get; set; }
        public string GithubToken { get; set; }
        public string ActivityType { get; set; }
        public string Activity { get; set; }
        public bool TumblrReposter { get; set; }
        public string? TumblrBlog { get; set; }
        public string? TumblrConsumerKey { get; set; }
        public string? TumblrConsumerSecret { get; set; }
        public string? TumblrOauthToken { get; set; }
        public string? TumblrOauthTokenSecret { get; set; }
    }
    public class Program
    {
        Config _config = Util.ParseConfig(File.ReadAllText("config.yml"));
        public static Task Main() => new Program().MainAsync();

        public async Task MainAsync()
        {
            
            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                services
                .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged+0x2,
                    AlwaysDownloadUsers = true
                }))
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                )
                .Build();

            await RunAsync(host);
        }

        public async Task RunAsync(IHost host)
        {
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var _client = provider.GetRequiredService<DiscordSocketClient>();
            var slashCommmands = provider.GetRequiredService<InteractionService>();
            await provider.GetRequiredService<InteractionHandler>().InitializeAsync();


            _client.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };
            slashCommmands.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };

            _client.Ready += async () =>
            {
                // Confirm that the bot is ready
                Console.WriteLine($"Logged in as {_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator} ({_client.CurrentUser.Id})");
                await slashCommmands.RegisterCommandsGloballyAsync();
            };

            // tumblr reposter
            if (_config.TumblrReposter) _client.ReactionAdded += HandleReactionAdded;

            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();
            ActivityType at = _config.ActivityType.ToLower() switch
            {
                "playing" => ActivityType.Playing,
                "watching" => ActivityType.Watching,
                "streaming" => ActivityType.Streaming,
                "competing" => ActivityType.Competing,
                "listening" => ActivityType.Listening,
                _ => ActivityType.Playing,
            };
            
            await _client.SetActivityAsync(new Game(_config.Activity, at, 0));

            await Task.Delay(-1);

        }

        public async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> msg, Cacheable<IMessageChannel, ulong> chan, SocketReaction react)
        {
            try
            {
                if (!react.Emote.Equals(new Emoji("🔁")))
                {
                    Console.WriteLine("Not 🔁 (repeat) emoji!");
                    return;
                }

                var message = await msg.GetOrDownloadAsync();
                var emotes = await message.GetReactionUsersAsync(new Emoji("🔁"), 1000).FlattenAsync();
                int count = emotes.Count();

                // path to the database
                string dbPath = @"URI=file:"+_config.SQLiteDatabase;

                using var conn = new SQLiteConnection(dbPath);
                conn.Open();

                if (count <= 4)
                {
                    Console.WriteLine("Not enough reactions! Got "+count+", expected 4");
                    return;
                }
                // only allow pb95 discord server
                if ((message.Channel as IGuildChannel).GuildId != 990326151987724378)
                {
                    Console.WriteLine("wrong server");

                    // DO YOU LIKE HOW I DEBUG MY CODE ?
                    return;
                }

                // check if it has already been posted
                string cmdTxt = "SELECT * FROM RepostedMessages";
                using var cmd = new SQLiteCommand(cmdTxt, conn);
                using SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    if ((long)msg.Id == rdr.GetInt64(0)) {
                        Console.WriteLine("Message " + message.Id + " has already been posted!");
                        return;
                    }
                }

                rdr.Close();

                cmd.CommandText = "INSERT INTO RepostedMessages(MessageId) VALUES(@mid)";
                cmd.Parameters.AddWithValue("@mid", message.Id);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                var tumblr = new TumblrClientFactory().Create<TumblrClient>(
                    _config.TumblrConsumerKey,
                    _config.TumblrConsumerSecret,
                    new DontPanic.TumblrSharp.OAuth.Token(
                        _config.TumblrOauthToken, _config.TumblrOauthTokenSecret));


                //check attachments 
                if (message.Attachments.FirstOrDefault() != null )
                {
                    List<string>? imageUrls = new();
                    List<BinaryFile> imageBinaries = new();

                    // TODO: videos
                    var webClient = new HttpClient();
                    foreach (var attachment in message.Attachments)
                    {
                        if (Regex.IsMatch(attachment.ContentType, "/png|jpeg/"))
                        {
                            byte[] imageBytes = await webClient.GetByteArrayAsync(attachment.Url);
                            imageBinaries.Add(new(imageBytes));
                        }
                    }
                    await tumblr.CreatePostAsync(_config.TumblrBlog, PostData.CreatePhoto(imageBinaries, CreateHyperlinks(message.CleanContent)));
                }
                else
                {
                    await tumblr.CreatePostAsync(_config.TumblrBlog, PostData.CreateText(CreateHyperlinks(message.CleanContent)));
                }
                Console.WriteLine("Posted message "+message.Id+", content:\n"+message.CleanContent);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public string CreateHyperlinks(string input)
        {
            Regex regex = new(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)");
            return regex.Replace(input, @"[$0](<$0>)");
        }

    }
}