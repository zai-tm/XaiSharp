using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class Unban : InteractionModuleBase<SocketInteractionContext>
    {
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
        [SlashCommand("unban", "unban a user")]
        public async Task Handle(IGuildUser user, string? reason = null)
        {
            EmbedBuilder unbanEmbed = new();
            if (Context.Guild.GetBanAsync(user).Result == null)
            {
                unbanEmbed.Color = Colors.Error;
                unbanEmbed.Description = $"This user isn't banned!";
                await RespondAsync(embed: unbanEmbed.Build(), ephemeral: true);
                return;
            }
            unbanEmbed.Color = Colors.Success;
            unbanEmbed.Description = $"Unbanned **{user.Username}#{user.Discriminator}**";
            if (reason != null)
                unbanEmbed.Description += $". Reason: **{reason}**";
            await Context.Guild.RemoveBanAsync(user,
                new RequestOptions { AuditLogReason = $"{reason ?? "No reason provided"} - Unbanned by {Context.User.Username}#{Context.User.Discriminator}" }
                );
            await RespondAsync(embed: unbanEmbed.Build());
        }
    }
}
