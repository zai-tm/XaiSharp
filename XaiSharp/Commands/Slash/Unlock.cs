using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class Unlock : InteractionModuleBase<SocketInteractionContext>
    {
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels)]
        [SlashCommand("unlock", "unlock a channel's write access")]
        public async Task Handle(ITextChannel channel, string? reason = null)
        {

            EmbedBuilder unlockEmbed = new();
            unlockEmbed.Color = 0x57F287;
            unlockEmbed.Description = $"Unlocked channel **{channel.Mention}**";

            if (reason != null)
                unlockEmbed.Description += $". Reason: **{reason}**";
            OverwritePermissions perm = new(
                sendMessages: PermValue.Inherit,
                sendMessagesInThreads: PermValue.Inherit,
                createPublicThreads: PermValue.Inherit
                );
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, perm, new RequestOptions { AuditLogReason = $"{reason ?? "No reason provided"} - Unlocked by {Context.User.Username}#{Context.User.Discriminator}" });
            await RespondAsync(embed: unlockEmbed.Build());
        }
    }
}
