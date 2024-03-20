using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    [IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    public class Countdown : InteractionModuleBase<SocketInteractionContext>
    {
        public class Event
        {
            public string Name { get; set; }
            public string Emoji { get; set; }
            [JsonProperty("image")]
            public string? ImageUrl { get; set; }
            public DateTimeOffset Date { get; set; }
        }

        HttpClient client = new();

        [SlashCommand("countdown", "Countdown until an event")]
        public async Task Handle()
        {
            try
            {
                string url = "http://pb95discord.com/event.json";
                string result = await client.GetStringAsync(url);
                Event nextEvent = JsonConvert.DeserializeObject<Event>(result);

                EmbedBuilder countdownEmbed = new()
                {
                    Title = "Days until " + nextEvent.Name,
                    Color = Colors.Negative,
                    //Description = $"{nextEvent.Emoji} {nextEvent.Name} is <t:{nextEvent.Date.ToUnixTimeSeconds()}:R>! {nextEvent.Emoji}",
                    Footer = new()
                    {
                        Text = $"Target date: {nextEvent.Date.ToString("MMMM d, yyyy hh:mm:ss tt")} UTC"
                    }
                };
                Console.WriteLine(nextEvent.Date);

                switch (DateTimeOffset.UtcNow)
                {
                    case var _ when DateTimeOffset.UtcNow.Date < nextEvent.Date.Date:
                        countdownEmbed.Description = $"{nextEvent.Emoji} {nextEvent.Name} is <t:{nextEvent.Date.ToUnixTimeSeconds()}:R>! {nextEvent.Emoji}";
                        break;
                    case var _ when DateTimeOffset.UtcNow.Date == nextEvent.Date.Date:
                        countdownEmbed.Description = $"{nextEvent.Emoji} **{nextEvent.Name.ToUpper()} IS TODAY!!!** {nextEvent.Emoji}";
                        countdownEmbed.Color = Colors.Positive;
                        break;
                    case var _ when DateTime.UtcNow.Date > nextEvent.Date.Date:
                        countdownEmbed.Description = $"{nextEvent.Emoji} {nextEvent.Name} was <t:{nextEvent.Date.ToUnixTimeSeconds()}:R>! {nextEvent.Emoji}";
                        countdownEmbed.Color = 0x888888;
                        break;
                }

                if (nextEvent.ImageUrl != null)
                {
                    countdownEmbed.ThumbnailUrl = nextEvent.ImageUrl;
                }

                await RespondAsync(embed: countdownEmbed.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
}
    }
}
