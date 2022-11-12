using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class Timeout : InteractionModuleBase<SocketInteractionContext>
    {
        public class Quote
        {
            public string Text { get; set; }
            public string Author { get; set; }
            public string? Image { get; set; }
        }

        [SlashCommand("timeout", "timeout a user")]
        public async Task Handle(IGuildUser user, [Summary("duration", "Duration of timeout (f.e. 1d, 4h, 10m)")] TimeSpan duration)
        {

            if (user.Hierarchy >= Context.Guild.CurrentUser.Hierarchy)
                await RespondAsync("that user is higher than you", ephemeral: true);
            else
            {
                if (duration > new TimeSpan(28, 0, 0, 0))
                {
                    await RespondAsync("Max length is 28 days!", ephemeral: true);
                } else
                {
                    await user.SetTimeOutAsync(duration);
                    await RespondAsync($"Timed out **{user.Username}#{user.Discriminator}** for **{duration:g}**");
                }

            }

        }
    }
}
