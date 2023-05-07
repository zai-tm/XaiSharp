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
using System.Reactive.Disposables;
using Discord.Webhook;

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
            List<ulong> bannedChannelIds = new List<ulong>()
                            {
                                1022207551531659335,
                                990334292683026432,
                                990334296801820692,
                                990334300232765510,
                                990334302728355900,
                                990334305169457214,
                                990334306754895932,
                                990334309162430534,
                                1019642793079099495,
                                1019634553779925082,
                                990334299037372477,
                                990334312803106876,
                                990334314199801896,
                                997195885165428816,
                                990334307870593084,
                                990334334768652288,
                                990334344730120242,
                                990334348865732608,
                                990334350065291334,
                                990334351214526525,
                                1027902246941380618,
                                994712600220794900,
                                990334352888041502,
                                1035630182289133568,
                                990334318427668500,
                                990334310370394252,
                                1019982537860325498,
                                1021139297916690617,
                                990334311435747378,
                                990334329861337139,
                                990334319753044018,
                                990334291651207180,
                                990334290170609758,
                                990334295195414569
                            };
            try
            {
                switch (react.Emote)
                {
                    case var _ when react.Emote.Equals(new Emoji("🔁")):
                        {
                            var message = await msg.GetOrDownloadAsync();
                            var emotes = await message.GetReactionUsersAsync(new Emoji("🔁"), 1000).FlattenAsync();
                            int count = emotes.Count();

                            // path to the database
                            string dbPath = @"URI=file:" + _config.SQLiteDatabase;

                            using var conn = new SQLiteConnection(dbPath);
                            conn.Open();

                            if (count < 4)
                            {
                                Console.WriteLine("Not enough reactions! Got " + count + ", expected 4");
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
                                if ((long)msg.Id == rdr.GetInt64(0))
                                {
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
                            if (message.Attachments.FirstOrDefault() != null)
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
                                await tumblr.CreatePostAsync(_config.TumblrBlog, PostData.CreatePhoto(imageBinaries, $"<h1>Message from {message.Author.Username}#{message.Author.Discriminator}</h1><br><p>{Markdig.Markdown.ToHtml(message.Content)}</p>"));
                            }
                            else
                            {
                                await tumblr.CreatePostAsync(_config.TumblrBlog, PostData.CreateText($"<h1>Message from {message.Author.Username}#{message.Author.Discriminator}</h1><br><p>{Markdig.Markdown.ToHtml(message.Content)}</p>"));
                            }
                            Console.WriteLine("Posted message " + message.Id + ", content:\n" + message.Content);
                        }
                        break;
                    case var _ when react.Emote.Equals(new Emoji("⬇️")):
                        {
                            var message = await msg.GetOrDownloadAsync();
                            var emotes = await message.GetReactionUsersAsync(new Emoji("⬇️"), 1000).FlattenAsync();
                            var upEmotes = await message.GetReactionUsersAsync(new Emoji("⬆️"), 1000).FlattenAsync();
                            int count = emotes.Count() - upEmotes.Count();

                            if (count < 5)
                            {
                                Console.WriteLine("not enough rections");
                                return;
                            }

                            if (bannedChannelIds.Contains(message.Channel.Id)) return;
                            if (message.IsPinned) return;

                            DiscordWebhookClient webhook = new(_config.WebhookId, _config.WebhookToken);

                            var BadMessageEmbed = new EmbedBuilder()
                                .WithAuthor(message.Author.Username, message.Author.GetAvatarUrl())
                                .WithDescription(message.Content)
                                .WithTimestamp(message.Timestamp);
                            if (message.Attachments.Count != 0)
                            {
                                BadMessageEmbed.ImageUrl = message.Attachments.FirstOrDefault().Url;
                            }
                            await webhook.SendMessageAsync(null, false, new[] { BadMessageEmbed.Build() }, "really bad message");
                            await message.DeleteAsync();
                        }
                        break;
                    case var _ when react.Emote.Equals(new Emoji("⬆️")):
                        {
                            var message = await msg.GetOrDownloadAsync();
                            var emotes = await message.GetReactionUsersAsync(new Emoji("⬆️"), 1000).FlattenAsync();
                            if (bannedChannelIds.Contains(message.Channel.Id)) return;
                            if (message.IsPinned) return;

                            foreach (var emote in emotes)
                            {
                                if (emote.Id == message.Author.Id)
                                {

                                    DiscordWebhookClient webhook = new(_config.WebhookId, _config.WebhookToken);

                                    var BadMessageEmbed = new EmbedBuilder()
                                        .WithAuthor(message.Author.Username, message.Author.GetAvatarUrl())
                                        .WithDescription(message.Content)
                                        .WithFooter("laugh at this user!! they tried to upvote their own message")
                                        .WithTimestamp(message.Timestamp);
                                    if (message.Attachments.Count != 0)
                                    {
                                        BadMessageEmbed.ImageUrl = message.Attachments.FirstOrDefault().Url;
                                    }
                                    await webhook.SendMessageAsync(null, false, new[] { BadMessageEmbed.Build() }, "really bad message");
                                    await message.DeleteAsync();
                                }
                            }
                            
                        }
                        break;
                    default:
                        Console.WriteLine("Not valid emoji!");
                        break;

                }
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

    }
}
