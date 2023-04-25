using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class Quotes : InteractionModuleBase<SocketInteractionContext>
    {

        [Group("quotes", "want a not-so-inspirational quote? this command is for you.")]

        public class QuoteGroup : InteractionModuleBase<SocketInteractionContext>
        {
            public class Quote
            {
                public string Text { get; set; }
                public string Author { get; set; }
                public string? Image { get; set; }
            }

            [SlashCommand("random", "A random quote")]
            public async Task HandleRandom()
            {
                List<Quote> quotesList = JsonConvert.DeserializeObject<List<Quote>>(File.ReadAllText("JSON/quotes.json"));
                var quotes = quotesList.ToArray();
                Random random = new();
                var quote = random.Next(0, quotes.Length);

                var quoteEmbed = new EmbedBuilder()
                    .WithColor(Convert.ToUInt32(Util.CreateMD5Hash(quotes[quote].Text)[..6], 16))
                    .WithAuthor(quotes[quote].Author)
                    .WithDescription(quotes[quote].Text.Replace("\\n", "\n"))
                    .WithImageUrl(quotes[quote].Image)
                    .WithFooter($"ID: {quote+1} | Inspirational quotes provided by the Progressbar95 community")
                    .WithCurrentTimestamp();
                await RespondAsync(embed: quoteEmbed.Build());
            }

            [SlashCommand("search", "choose a quote")]
            public async Task HandleSearch(int id)
            {
                try
                {
                    List<Quote> quotesList = JsonConvert.DeserializeObject<List<Quote>>(File.ReadAllText("JSON/quotes.json"));
                    var quotes = quotesList.ToArray();

                    var quoteEmbed = new EmbedBuilder()
                        .WithColor(Convert.ToUInt32(Util.CreateMD5Hash(quotes[id - 1].Text)[..6], 16))
                        .WithAuthor(quotes[id - 1].Author)
                        .WithDescription(quotes[id - 1].Text.Replace("\\n", "\n"))
                        .WithImageUrl(quotes[id - 1].Image)
                        .WithFooter($"ID: {id} | Inspirational quotes provided by the Progressbar95 community")
                        .WithCurrentTimestamp();
                    await RespondAsync(embed: quoteEmbed.Build());
                }
                catch (IndexOutOfRangeException)
                {
                    var errorEmbed = new EmbedBuilder()
                        .WithColor(Colors.Error)
                        .WithDescription("❌ There is no quote with that ID");
                    await RespondAsync(embed: errorEmbed.Build(), ephemeral:true);
                }
            }
        }
    }
}
