using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class Countdown : InteractionModuleBase<SocketInteractionContext>
    {
        public class Event
        {
            public string Name { get; set; }
            public string Emoji { get; set; }
            public DateTimeOffset Date { get; set; }
        }

        [SlashCommand("countdown", "Countdown until an event")]
        public async Task Handle()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var nextEventJson = await client.GetStringAsync("https://pb95discord.cf/event.json");
                    Event nextEvent = JsonConvert.DeserializeObject<Event>(nextEventJson);

                    EmbedBuilder countdownEmbed = new()
                    {
                        Title = "Days until " + nextEvent.Name,
                        Color = Colors.Negative,
                        //Description = $"{nextEvent.Emoji} {nextEvent.Name} is <t:{nextEvent.Date.ToUnixTimeSeconds()}:R>! {nextEvent.Emoji}",
                        Footer = new()
                        {
                            Text = $"Target date: {nextEvent.Date.ToString("F")} UTC"
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

                    await RespondAsync(embed: countdownEmbed.Build());

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
