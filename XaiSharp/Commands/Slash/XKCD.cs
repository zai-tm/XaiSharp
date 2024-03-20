using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using System.Net;
namespace XaiSharp.Commands.Slash
{
    public class XKCD : InteractionModuleBase<SocketInteractionContext>
    {
        [Group("xkcd", "The webcomic everyone knows and loves")]
        [IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
        [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
        public class XKCDGroup : InteractionModuleBase<SocketInteractionContext>
        {
            HttpClient client = new();
            [SlashCommand("latest", "Latest comic")]
            public async Task HandleLatest()
            {
                string url = "https://xkcd.com/info.0.json";
                string result = await client.GetStringAsync(url);
                dynamic data = JsonConvert.DeserializeObject(result);
                string title = data.safe_title;
                string year = data.year;
                string month = data.month;
                string day = data.day;

                EmbedBuilder xkcdEmbed = new()
                {
                    Title = $"{data.num}: {data.safe_title}",
                    Url = $"https://xkcd.com/{data.num}",
                    ImageUrl = data.img,
                    Color = Convert.ToUInt32(Util.CreateMD5Hash(title)[..6], 16),
                    Footer = new()
                    {
                        Text = $"{data.alt} | {Util.CreateDateString(year, month, day)}"
                    }//,
                    //Timestamp = new DateTimeOffset(Convert.ToInt32(data.year), Convert.ToInt32(data.month), Convert.ToInt32(data.day), 0, 0, 0, new TimeSpan(0))
                };

                if (data.extra_parts != null)
                {
                    xkcdEmbed.Description = "*This comic is interactive!*";
                }

                await RespondAsync(embed: xkcdEmbed.Build());
            }

            [SlashCommand("random", "A random comic")]
            public async Task HandleRandom()
            {
                string urlForNum = "https://xkcd.com/info.0.json";
                string resultForNum = await client.GetStringAsync(urlForNum);

                dynamic dataForNum = JsonConvert.DeserializeObject(resultForNum);

                int num = dataForNum.num+1;

                int randomComicNum = new Random().Next(1, num);

                string url = $"https://xkcd.com/{randomComicNum}/info.0.json";
                string result = await client.GetStringAsync(url);
                dynamic data = JsonConvert.DeserializeObject(result);
                string title = data.safe_title;
                string year = data.year;
                string month = data.month;
                string day = data.day;

                EmbedBuilder xkcdEmbed = new()
                {
                    Title = $"{data.num}: {data.safe_title}",
                    Url = $"https://xkcd.com/{data.num}",
                    ImageUrl = data.img,
                    Color = Convert.ToUInt32(Util.CreateMD5Hash(title)[..6], 16),
                    Footer = new()
                    {
                        Text = $"{data.alt} | {Util.CreateDateString(year, month, day)}"
                    }//,
                    //Timestamp = new DateTimeOffset(Convert.ToInt32(data.year), Convert.ToInt32(data.month), Convert.ToInt32(data.day), 0, 0, 0, new TimeSpan(0))
                };

                if (data.extra_parts != null)
                {
                    xkcdEmbed.Description = "*This comic is interactive!*";
                }
                await RespondAsync(embed: xkcdEmbed.Build());
            }

            [SlashCommand("search", "Search for a comic")]
            public async Task HandleSearch(int number)
            {
                string url = $"https://xkcd.com/{number}/info.0.json";
                string result = String.Empty;
                try {
                    result = await client.GetStringAsync(url);
                } catch (HttpRequestException)
                {
                    // 404 "comic"
                    if (number == 404)
                    {
                        EmbedBuilder xkcd404Embed = new()
                        {
                            Title = "404: Not Found",
                            Url = "https://xkcd.com/404",
                            ImageUrl = "https://www.explainxkcd.com/wiki/images/9/92/not_found.png",
                            Color = Convert.ToUInt32(Util.CreateMD5Hash("Not Found")[..6], 16),
                            Footer = new()
                            {
                                Text = "2008-04-01"
                            }
                        };
                        await RespondAsync(embed: xkcd404Embed.Build());
                        return;
                    }
                    EmbedBuilder errorEmbed = new()
                    {
                        Description = "This comic doesn't exist!",
                        Color = Colors.Error
                    };
                    await RespondAsync(embed: errorEmbed.Build(), ephemeral:true);
                }
                dynamic data = JsonConvert.DeserializeObject(result);
                string title = data.safe_title;
                string year = data.year;
                string month = data.month;
                string day = data.day;

                EmbedBuilder xkcdEmbed = new()
                {
                    Title = $"{data.num}: {data.safe_title}",
                    Url = $"https://xkcd.com/{data.num}",
                    ImageUrl = data.img,
                    Color = Convert.ToUInt32(Util.CreateMD5Hash(title)[..6], 16),
                    Footer = new()
                    {
                        Text = $"{data.alt} | {Util.CreateDateString(year, month, day)}"
                    }//,
                    //Timestamp = new DateTimeOffset(Convert.ToInt32(data.year), Convert.ToInt32(data.month), Convert.ToInt32(data.day), 0, 0, 0, new TimeSpan(0))
                };

                if (data.extra_parts != null)
                {
                    xkcdEmbed.Description = "*This comic is interactive!*";
                }
                await RespondAsync(embed: xkcdEmbed.Build());
            }
        }
    }
}
