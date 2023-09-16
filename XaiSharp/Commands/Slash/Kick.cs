using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class Kick : InteractionModuleBase<SocketInteractionContext>
    {
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
        [SlashCommand("kick", "kick a user")]
        public async Task Handle(IUser user, string? reason = null)
        {
            IGuildUser guildUser = user as IGuildUser;
            EmbedBuilder kickEmbed = new();
            if (guildUser == null)
            {
                kickEmbed.Color = Colors.Error;
                kickEmbed.Description = "You can't kick someone who isn't in this server!";
                await RespondAsync(embed: kickEmbed.Build(),ephemeral:true);
                return;
            }

            if (guildUser.Hierarchy >= Context.Guild.CurrentUser.Hierarchy)
            {
                kickEmbed.Color = Colors.Error;
                kickEmbed.Description = "You cannot kick someone higher than you!";
                await RespondAsync(embed: kickEmbed.Build(), ephemeral: true);
            }
            else {
                if (user == Context.Interaction.User)
                {
                    kickEmbed.Color = Colors.Error;
                    kickEmbed.Description = "You can't kick yourself!";
                    await RespondAsync(embed:kickEmbed.Build());
                    return;
                }

                kickEmbed.Color = Colors.Success;
                kickEmbed.Description = $"Kicked **{user.Username}#{user.Discriminator}**";
                if (reason != null)
                    kickEmbed.Description += $". Reason: **{reason}**";
                try
                {
                    EmbedBuilder userBanEmbed = new();
                    userBanEmbed.Color = Colors.Negative;
                    userBanEmbed.Description = $"You have been kicked from **{Context.Guild.Name}**";
                    userBanEmbed.Description += $" for **{reason}**";
                    await user.SendMessageAsync(embed: userBanEmbed.Build());

                }
                catch (Discord.Net.HttpException)
                {
                    kickEmbed.Description += "\nI couldn't DM them.";
                }

                await guildUser.KickAsync(options: new RequestOptions { AuditLogReason = $"{reason ?? "No reason provided"} - Kicked by {Context.User.Username}#{Context.User.Discriminator}" });
                
                await RespondAsync(embed:kickEmbed.Build());
            }

        }
    }
}
