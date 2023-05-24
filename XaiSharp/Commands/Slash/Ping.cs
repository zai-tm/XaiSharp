using Discord;
using Discord.Interactions;
using System.Diagnostics;

namespace XaiSharp.Commands.Slash
{
    public class Ping : InteractionModuleBase<SocketInteractionContext>
    {

        [SlashCommand("ping", "View the latency of the bot")]
        public async Task Handle()
        {
            EmbedBuilder pingEmbed = new()
            {
                Title = "🏓 Pong!",
                Color = Colors.Random()
            };
            pingEmbed.AddField("Latency", $"{Context.Client.Latency}ms", true);
            Stopwatch stopwatch = Stopwatch.StartNew();
            await RespondAsync(embed:pingEmbed.Build(),ephemeral:true);
            stopwatch.Stop();
            pingEmbed.AddField("Round-trip", $"{stopwatch.ElapsedMilliseconds}ms", true);
            await ModifyOriginalResponseAsync(m => m.Embed = pingEmbed.Build());
        }
    }
}
