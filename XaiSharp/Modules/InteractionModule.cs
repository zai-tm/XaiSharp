using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XaiSharp.Modules
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        public class Quote
        {
            public string Text;
            public string Author;
            public string? Image;
        }

        [SlashCommand("crash", "Crash the bot... maybe")]
        public async Task HandleCrashCommand()
        {
            await RespondAsync("Crashing the bot in 3...");
            await Task.Delay(1000);
            await ModifyOriginalResponseAsync(m => m.Content = "Crashing the bot in 2...");
            await Task.Delay(1000);
            await ModifyOriginalResponseAsync(m => m.Content = "Crashing the bot in 1...");
            await Task.Delay(1000);
            await ModifyOriginalResponseAsync(m => m.Content = $"{Context.User.Username} is not in the sudoers file. This incident will be reported.");
        }

        [SlashCommand("dice", "Rolls a dice!")]
        public async Task HandleDiceCommand(int? sides = null)
        {
            Random random = new();
            var roll = random.Next(1,6);
            if (sides != null)
            {
                roll = random.Next(1, sides.Value);
            }

            var diceEmbed = new EmbedBuilder()
                .WithTitle("Dice")
                .WithColor(0xffffff)
                .AddField("The dice landed on...", $"**{roll}**");
            await RespondAsync(embed: diceEmbed.Build());
        }

        [SlashCommand("quotes", "A random quote that is most likely dumb")]
        public async Task HandleQuotesCommand()
        {
            List<Quote> quotesList = JsonConvert.DeserializeObject<List<Quote>>(File.ReadAllText("JSON/quotes.json"));
            var quotes = quotesList.ToArray();
            Random random = new();
            var quote = random.Next(0, quotes.Length);

            var quoteEmbed = new EmbedBuilder()
                .WithColor((uint)Math.Floor(random.NextDouble() * (0xffffff + 1)))
                .WithAuthor(quotes[quote].Author)
                .WithDescription($"{quotes[quote].Text.Replace("\\n", "\n")}\n\n[Suggest a quote](https://forms.gle/BAZ2rzgYcaDXf9tR7)")
                .WithImageUrl(quotes[quote].Image)
                .WithFooter("Inspirational quotes provided by the Progressbar95 community")
                .WithCurrentTimestamp();
            await RespondAsync(embed: quoteEmbed.Build());
        }
    }
}
