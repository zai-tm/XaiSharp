﻿using Discord;
using Discord.Interactions;
using System.Text;
using System.Diagnostics;
using static XaiSharp.Util;

namespace XaiSharp.Commands.Slash
{
    public class Info : InteractionModuleBase<SocketInteractionContext>
    {

        [Group("info", "Info about a server or user")]
        public class InfoGroup : InteractionModuleBase<SocketInteractionContext>
       
        {
            [EnabledInDm(false)]
            [SlashCommand("server", "View info about the current server")]
            public async Task HandleServer()
            {
                EmbedBuilder serverEmbed = new()
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = Context.Guild.Name,
                        IconUrl = Context.Guild.IconUrl
                    },
                    Color = Colors.Random(),
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "Owner",
                            Value = Context.Guild.Owner.Mention,
                            IsInline = true,
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "Roles",
                            Value = Context.Guild.Roles.Count - 1,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "Members",
                            Value = Context.Guild.MemberCount,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "Emojis",
                            Value = Context.Guild.Emotes.Count,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "Stickers",
                            Value = Context.Guild.Stickers.Count,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "Rules",
                            Value = Context.Guild.RulesChannel != null ? Context.Guild.RulesChannel.Mention : "None",
                            IsInline = true
                        }
                    },
                    Description = Context.Guild.Description ?? "",
                    ThumbnailUrl = Context.Guild.IconUrl,
                    ImageUrl = Context.Guild.BannerUrl,
                    Footer = new EmbedFooterBuilder
                    {
                        Text = $"ID: {Context.Guild.Id}"
                    },
                    Timestamp = Context.Guild.CreatedAt
                };
                await RespondAsync(embed: serverEmbed.Build(), ephemeral: true);
            }

            [EnabledInDm(false)]
            [SlashCommand("user", "View info about a user")]
            public async Task HandleUser(IGuildUser user)
            {
                EmbedBuilder userEmbed = new()
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = $"{user.Username}#{user.Discriminator}",
                        IconUrl = user.GetAvatarUrl()
                    },
                    Color = Colors.Random(),
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "Joined",
                            Value = $"<t:{ToUnixTime(user.JoinedAt)}:f> (<t:{ToUnixTime(user.JoinedAt)}:R>)",
                            IsInline = true,
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "Created",
                            Value = $"<t:{ToUnixTime(user.CreatedAt)}:f> (<t:{ToUnixTime(user.CreatedAt)}:R>)",
                            IsInline = true
                        }
                    },
                    Footer = new EmbedFooterBuilder
                    {
                        Text = $"ID: {user.Id}"
                    }
                };

                StringBuilder roles = new();

                foreach (ulong roleId in user.RoleIds)
                {
                    //Console.WriteLine(Context.Guild.GetRole(roleId).Name);
                    if (roleId == Context.Guild.Id) continue;
                    roles.Append($"{Context.Guild.GetRole(roleId).Mention}, ");
                }
                roles.Remove(roles.Length - 2, 2);

                userEmbed.AddField($"Roles ({user.RoleIds.Count-1})", roles, false);
                await RespondAsync(embed: userEmbed.Build(), ephemeral: true);
            }
        }
    }
}