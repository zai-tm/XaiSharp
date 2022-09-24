using System;
using Newtonsoft.Json;
using Discord;
using Discord.Interactions;
//using Discord.Commands;
using Discord.WebSocket;
using Discord.Net;

namespace XaiSharp
{
    public class Program
    {
        public class Config
        {
            public string Token { get; set; }
            public ulong GuildId { get; set; }
        }
        private DiscordSocketClient? _client;

        Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;

            //Login and start the bot
            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        public async Task Client_Ready()
        {
            var guild = _client.GetGuild(config.GuildId);
            //command
            var guildCommand = new SlashCommandBuilder()
                .WithName("ping")
                .WithDescription("pong")
                .AddOption("text", ApplicationCommandOptionType.String, "text to say", isRequired: true);

            //create commands
            try
            {
                await guild.CreateApplicationCommandAsync(guildCommand.Build());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                //log the error
                Console.WriteLine(json);
            }
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            await command.RespondAsync($"🏓 {command.User.Mention} {command.Data.Options.First().Value}");
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}