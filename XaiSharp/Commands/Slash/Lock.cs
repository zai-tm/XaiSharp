using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class Lock : InteractionModuleBase<SocketInteractionContext>
    {
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels)]
        [SlashCommand("lock", "lock a channel's write access")]
        public async Task Handle(ITextChannel channel, string? reason = null)
        {

            EmbedBuilder lockEmbed = new();
            lockEmbed.Color = Colors.Success;
            lockEmbed.Description = $"Locked channel **{channel.Mention}**";

            if (reason != null)
                lockEmbed.Description += $". Reason: **{reason}**";
            OverwritePermissions perm = new(
                sendMessages: PermValue.Deny,
                sendMessagesInThreads: PermValue.Deny,
                createPublicThreads: PermValue.Deny
                );
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, perm, new RequestOptions { AuditLogReason = $"{reason ?? "No reason provided"} - Locked by {Context.User.Username}#{Context.User.Discriminator}" });
            await RespondAsync(embed: lockEmbed.Build());
        }
    }
}
