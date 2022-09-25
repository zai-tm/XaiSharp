using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace XaiSharp
{
    public class Config
    {
        public ulong BotOwnerId { get; set; }
        public string Token { get; set; }
        public ulong ClientId { get; set; }
        public ulong GuildId { get; set; }
        public ulong WebhookId { get; set; }
        public string WebhookToken { get; set; }
        public string SqlUser { get; set; }
        public string SqlPass { get; set; }
        public string SqlDb { get; set; }
        public string ActivityType { get; set; }
        public string Activity { get; set; }
    }
    public class Program
    {
        Config _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
        public static Task Main() => new Program().MainAsync();

        public async Task MainAsync()
        {
            
            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                services
                .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged,
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
                Console.WriteLine("Ready!");
                await slashCommmands.RegisterCommandsGloballyAsync();
            };

            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();
            await _client.SetGameAsync("you", "", ActivityType.Watching);

            await Task.Delay(-1);

        }
    }
}