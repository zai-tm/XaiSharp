using Discord.Interactions;
using Discord.Webhook;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class Suggest : InteractionModuleBase<SocketInteractionContext>
    {
        Config config = Util.ParseConfig(File.ReadAllText("config.yml"));

        [SlashCommand("suggest", "Suggest a feature")]
        public async Task Handle(string suggestion)
        {
            DiscordWebhookClient webhook = new(config.WebhookId, config.WebhookToken);
            if (suggestion.Length > 2000)
            {
                await RespondAsync("Your suggestion is too long.", ephemeral: true);
            }
            else
            {
                await RespondAsync("Your suggestion has been sent.", ephemeral: true);
                await webhook.SendMessageAsync(suggestion, false, null, Context.User.Username, Context.User.GetAvatarUrl());
            }
        }
    }
}
