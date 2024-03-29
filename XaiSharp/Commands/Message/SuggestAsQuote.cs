﻿using Discord;
using Discord.Interactions;
using Discord.Webhook;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Message
{
    [IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
    public class SuggestAsQuote : InteractionModuleBase<SocketInteractionContext>
    {
        Config config = Util.ParseConfig(File.ReadAllText("config.yml"));
        [MessageCommand("Suggest as quote")]
        public async Task Handle(IMessage message)
        {
            DiscordWebhookClient webhook = new(config.WebhookId, config.WebhookToken);



            var suggestQuoteEmbed = new EmbedBuilder()
                .WithAuthor(message.Author.Username, message.Author.GetAvatarUrl())
                .WithDescription(message.Content)
                .WithFooter($"Submitted by {Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp();
            //Console.WriteLine(suggestQuoteEmbed.ToString());
            if (message.Attachments.Count != 0 )
            {
                suggestQuoteEmbed.ImageUrl = message.Attachments.FirstOrDefault().Url;
            }
            if (message.Embeds.Count != 0)
            {
                suggestQuoteEmbed.Description += $"\n{message.Embeds?.FirstOrDefault()?.Author}\n{message.Embeds?.FirstOrDefault()?.Title}\n{message.Embeds?.FirstOrDefault()?.Description}\n{message.Embeds?.FirstOrDefault()?.Footer}";
            }
            suggestQuoteEmbed.AddField("JSON", $"```json\n{{\n    \"Text\": \"{suggestQuoteEmbed.Description.Replace("\n", "\\\\n")}\",\n    \"Author\": \"{suggestQuoteEmbed.Author.Name}\"\n}}```", false);

            if (message.Author == Context.User)
            {
                await RespondAsync("You cannot sumbit your own messages. Cry about it", ephemeral: true);
                return;
            }

            await webhook.SendMessageAsync(String.Empty, false, embeds: new[] { suggestQuoteEmbed.Build() }, "Quote suggestion");
            await RespondAsync("Your quote suggestion was sent.", ephemeral: true);
        }
    }
}
