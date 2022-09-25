using System;
using Newtonsoft.Json;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Net;

namespace XaiSharp
{
    public class Commands
    {
        public Task CreateCommands()
        {
            var pingCommand = new SlashCommandBuilder()
                .WithName("ping")
                .WithDescription("pong")
                .AddOption("text", ApplicationCommandOptionType.String, "text to say", isRequired: true);

            return Task.CompletedTask;
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            await command.RespondAsync($"🏓 {command.User.Mention} {command.Data.Options.First().Value}");
        }
    }
}