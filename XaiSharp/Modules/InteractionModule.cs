using Discord;
using Discord.Interactions;
using Discord.Webhook;
using Newtonsoft.Json;

namespace XaiSharp.Modules
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        public Config _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
        public class Quote
        {
            public string Text { get; set; }
            public string Author { get; set; }
            public string? Image { get; set; }
        }

        public class DeathMessage
        {
            public List<string> Message { get; set; }
        }


        //----------------//
        // SLASH COMMANDS //
        //----------------//
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
            var roll = random.Next(1, 6);
            if (sides != null)
            {
                if (sides >= 0)
                    await RespondAsync("You cannot enter 0 or a negative number!", ephemeral: true);
                else
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

        [SlashCommand("8-ball", "Ask the magic 8-ball a question")]
        public async Task HandleEightBallCommand(string question)
        {
            Random random = new();
            var responses = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("JSON/8ball.json")).ToArray();
            var eightBallEmbed = new EmbedBuilder()
                .WithColor(0x000000)
                .WithTitle("Magic 8-ball")
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/857415925324840960/1023778034936979456/unknown.png")
                .AddField("You asked:", question)
                .AddField("The 8-ball says", responses[random.Next(0, responses.Length)]);
            if (question.Length > 256)
                await RespondAsync("Your question is too long.", ephemeral: true);
            else
                await RespondAsync(embed: eightBallEmbed.Build());

        }

        [SlashCommand("credit", "What is your Progress Credit™?")]
        public async Task HandleCreditCommand()
        {
            Random random = new();
            var image = $"https://zai-tm.github.io/credit/{random.Next(0, 9)}.png";
            var creditEmbed = new EmbedBuilder()
                .WithTitle("Progress Credit™")
                .WithImageUrl(image)
                .WithCurrentTimestamp();

            await RespondAsync(embed: creditEmbed.Build());
        }

        [SlashCommand("suggest", "Suggest a feature")]
        public async Task HandleSuggestCommand(string suggestion)
        {
            DiscordWebhookClient webhook = new(_config.WebhookId, _config.WebhookToken);
            if (suggestion.Length > 2000)
            {
                await RespondAsync("Your suggestion is too long.", ephemeral: true);
            } else
            {
                await RespondAsync("Your suggestion has been sent.", ephemeral: true);
                await webhook.SendMessageAsync(suggestion, false, null, Context.User.Username, Context.User.GetAvatarUrl());
            }
            //await RespondAsync("not implemented yet",ephemeral: true);
        }
        [SlashCommand("activity", "Play a voice chat activity because the Discord API lets you even if the server doesn't have them!")]
        public async Task HandleActivityCommand(
            [ChannelTypes(ChannelType.Voice)] IChannel channel,
            [
            Choice ("Poker Night", 0),
            Choice ("Chess In The Park", 1),
            Choice ("Checkers In The Park", 2),
            Choice ("Blazing 8s", 3),
            Choice ("SpellCast", 4),
            Choice ("Letter League", 5),
            Choice ("Word Snacks (Free)", 6),
            Choice ("Watch Together (Free)", 7),
            Choice ("Sketch Heads (Free)", 8),
            Choice ("Land-io", 9),
            Choice ("Putt Party", 10),
            Choice ("Bobble League", 11),
            Choice ("Know What I Meme", 12),
            Choice ("Ask Away (Free)", 13),
            Choice ("Bash Out", 14),

            ] int activity)
        {
            ulong[] activities = {
                755827207812677713, //< Poker Night
                832012774040141894, //< Chess In The Park
                832013003968348200, //< Checkers In The Park
                832025144389533716, //< Blazing 8s
                852509694341283871, //< SpellCast
                879863686565621790, //< Letter League
                879863976006127627, //< Word Snacks
                880218394199220334, //< Watch Together
                902271654783242291, //< Sketch Heads
                903769130790969345, //< Land-io
                945737671223947305, //< Putt Party
                947957217959759964, //< Bobble League
                950505761862189096, //< Know What I Meme
                976052223358406656, //< Ask Away
               1006584476094177371, //< Bash Out
            };

            string[] activityStr =
            {
                "Poker Night",
                "Chess In The Park",
                "Checkers In The Park",
                "Blazing 8s",
                "SpellCast",
                "Letter League",
                "Word Snacks",
                "Watch Together",
                "Sketch Heads",
                "Land-io",
                "Putt Party",
                "Bobble League",
                "Know What I Meme",
                "Ask Away",
                "Bash Out",
            };

            var invite = await Context.Guild.GetVoiceChannel(channel.Id).CreateInviteToApplicationAsync(applicationId: activities[activity], maxAge:null);

            var joinActivityButton = new ButtonBuilder()
            {
                Label = $"Join {activityStr[activity]}",
                //CustomId = "join",
                Url = invite.ToString(),
                Style = ButtonStyle.Link
            };

            var component = new ComponentBuilder()
            .WithButton(joinActivityButton)
            .Build();

            if (((activity <= 5 || activity >= 9) && activity != 13) && Context.Guild.PremiumTier == PremiumTier.None)
                await RespondAsync($"This server needs a boost level of at least **1** to play {activityStr[activity]}.", ephemeral: true);
            else
                await RespondAsync($"Click the button below to join {activityStr[activity]}!", components: component);
        }

        //---------------//
        // USER COMMANDS //
        //---------------//

        [UserCommand("Kill")]
        public async Task HandleKillThisUserCommand(IUser user)
        {
            Random random = new();
            List<DeathMessage> messages = JsonConvert.DeserializeObject<List<DeathMessage>>(File.ReadAllText("JSON/death.json"));
            var messagesArr = messages.ToArray();
            //Console.WriteLine(messages);
            var deathMessageEmbed = new EmbedBuilder()
                .WithTitle("You Died!")
                .WithColor(0x910700);;
            if (random.Next(0,1) == 0)
            {
                deathMessageEmbed.Description = $"{user.Mention} {messagesArr[0].Message.ToArray()[random.Next(0, messagesArr[0].Message.Count)]} {Context.User.Mention}";
            }
            else
            {
                deathMessageEmbed.Description = $"{user.Mention} {messagesArr[1].Message.ToArray()[random.Next(0, messagesArr[1].Message.Count)]}";
            }
            await RespondAsync(embed: deathMessageEmbed.Build());
        }

        //------------------//
        // MESSAGE COMMANDS //
        //------------------//
        [MessageCommand("Suggest as quote")]
        public async Task HandleSuggestAsQuoteCommand(IMessage message)
        {
            DiscordWebhookClient webhook = new(_config.WebhookId, _config.WebhookToken);



            var suggestQuoteEmbed = new EmbedBuilder()
                .WithAuthor(message.Author.Username, message.Author.GetAvatarUrl())
                .WithDescription(message.Content)
                .WithFooter($"Submitted by {Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp();
            //Console.WriteLine(suggestQuoteEmbed.ToString());

            if (message.Attachments.Count != 0)
            {
                suggestQuoteEmbed.ImageUrl = message.Attachments.FirstOrDefault().Url;
            } 
            await webhook.SendMessageAsync(String.Empty, false, embeds: new[] { suggestQuoteEmbed.Build() }, "Quote suggestion");
            await RespondAsync("Your quote suggestion was sent.", ephemeral: true);
        }
    }
}
