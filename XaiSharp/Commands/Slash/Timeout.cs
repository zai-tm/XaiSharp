using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class Timeout : InteractionModuleBase<SocketInteractionContext>
    {
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
        [SlashCommand("timeout", "timeout a user")]
        public async Task Handle(IGuildUser user, [Summary("duration", "Duration of timeout (f.e. 1d, 4h, 10m, 2m30s)")] TimeSpan duration)
        {
            EmbedBuilder timeoutEmbed = new();
            if (user.Hierarchy >= Context.Guild.CurrentUser.Hierarchy)
            {
                timeoutEmbed.Color = 0xED4245;
                timeoutEmbed.Description = "You cannot timeout someone higher than you!";
                await RespondAsync(embed: timeoutEmbed.Build(), ephemeral: true);
            }

            else
            {
                if (duration > new TimeSpan(28, 0, 0, 0))
                {
                    timeoutEmbed.Color = 0xED4245;
                    timeoutEmbed.Description = "Max length is 28 days!";

                    await RespondAsync(embed: timeoutEmbed.Build(), ephemeral: true);
                } else
                {

                    await user.SetTimeOutAsync(duration);
                    timeoutEmbed.Color = 0x57F287;
                    timeoutEmbed.Description = $"Timed out **{user.Username}#{user.Discriminator}** for **{duration:g}**";
                    await RespondAsync(embed:timeoutEmbed.Build());
                }

            }

        }
    }
}
