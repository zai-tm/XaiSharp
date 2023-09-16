using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class Slowmode : InteractionModuleBase<SocketInteractionContext>
    {
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels)]
        [SlashCommand("slowmode", "set the slowmode of a channel")]
        public async Task Handle(ITextChannel channel, [Summary("duration", "Duration of slowmode (f.e. 10s, 1m, 2m30s)")] TimeSpan duration, string? reason = null)
        {
            EmbedBuilder slowmodeEmbed = new();

            if (duration > new TimeSpan(6, 0, 0))
            {
                slowmodeEmbed.Color = Colors.Error;
                slowmodeEmbed.Description = "Max length is 6 hours!";

                await RespondAsync(embed: slowmodeEmbed.Build(), ephemeral: true);
            }
            else
            {;
                await channel.ModifyAsync(c => c.SlowModeInterval = Convert.ToInt16(duration.TotalSeconds), 
                    new RequestOptions { AuditLogReason = $"{reason ?? "No reason provided"} - Timed out by {Context.User.Username}#{Context.User.Discriminator}" });
                slowmodeEmbed.Color = Colors.Success;
                slowmodeEmbed.Description = $"Set the slowmode of {channel.Mention} to **{duration:g}**";
                if (reason != null)
                    slowmodeEmbed.Description += $". Reason: **{reason}**";
                await RespondAsync(embed: slowmodeEmbed.Build());
            }

        }
    }
}
