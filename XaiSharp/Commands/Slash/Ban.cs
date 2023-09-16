using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class Ban : InteractionModuleBase<SocketInteractionContext>
    {
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
        [SlashCommand("ban", "ban a user")]
        public async Task Handle(IUser user, [Summary("prune_days", "Number of days to remove messages (Must be bettween 0 and 7!)")] int? prune_days = null, string? reason = null)
        {
            IGuildUser guildUser = user as IGuildUser;
            EmbedBuilder banEmbed = new();
            if (guildUser == null)
            {
                banEmbed.Color = Colors.Error;
                banEmbed.Description = "You can't ban someone who isn't in this server!";
                await RespondAsync(embed: banEmbed.Build(),ephemeral:true);
                return;
            }

            if (guildUser.Hierarchy >= Context.Guild.CurrentUser.Hierarchy)
            {
                banEmbed.Color = Colors.Error;
                banEmbed.Description = "You cannot ban someone higher than you!";
                await RespondAsync(embed: banEmbed.Build(), ephemeral: true);
            }
            else {
                if (user == Context.Interaction.User)
                {
                    banEmbed.Color = Colors.Error;
                    banEmbed.Description = "You can't ban yourself!";
                    await RespondAsync(embed:banEmbed.Build());
                    return;
                }

                banEmbed.Color = Colors.Success;
                banEmbed.Description = $"Banned **{user.Username}#{user.Discriminator}**";
                if (reason != null)
                    banEmbed.Description += $". Reason: **{reason}**";
                try
                {
                    EmbedBuilder userBanEmbed = new();
                    userBanEmbed.Color = Colors.Negative;
                    userBanEmbed.Description = $"You have been banned from **{Context.Guild.Name}**";
                    userBanEmbed.Description += $" for **{reason}**";
                    await user.SendMessageAsync(embed: userBanEmbed.Build());

                }
                catch (Discord.Net.HttpException)
                {
                    banEmbed.Description += "\nI couldn't DM them.";
                }

                await Context.Guild.AddBanAsync(user, prune_days != null ? (int)prune_days : 0, // Set prune days to 0 if not specified
                    options:new RequestOptions { AuditLogReason = $"{reason ?? "No reason provided"} - Banned by {Context.User.Username}#{Context.User.Discriminator}" }
                    );
                
                await RespondAsync(embed:banEmbed.Build());
            }

        }
    }
}
