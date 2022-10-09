using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class Quotes : InteractionModuleBase<SocketInteractionContext>
    {
        public class Quote
        {
            public string Text { get; set; }
            public string Author { get; set; }
            public string? Image { get; set; }
        }

        [SlashCommand("quotes", "A random quote that is most likely dumb")]
        public async Task Handle()
        {
            List<Quote> quotesList = JsonConvert.DeserializeObject<List<Quote>>(File.ReadAllText("JSON/quotes.json"));
            var quotes = quotesList.ToArray();
            Random random = new();
            var quote = random.Next(0, quotes.Length);

            var quoteEmbed = new EmbedBuilder()
                .WithColor((uint)Math.Floor(random.NextDouble() * (0xffffff + 1)))
                .WithAuthor(quotes[quote].Author)
                .WithDescription(quotes[quote].Text.Replace("\\n", "\n"))
                .WithImageUrl(quotes[quote].Image)
                .WithFooter("Inspirational quotes provided by the Progressbar95 community")
                .WithCurrentTimestamp();
            await RespondAsync(embed: quoteEmbed.Build());
        }
    }
}
