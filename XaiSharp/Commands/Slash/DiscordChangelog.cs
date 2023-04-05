using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace XaiSharp.Commands.Slash
{
    public class DiscordChangelog : InteractionModuleBase<SocketInteractionContext>
    {
        public class Changelog
        {
            [JsonProperty("date")]
            public DateTimeOffset Date { get; set; }
            [JsonProperty("asset")]
            public string Asset { get; set; }
            [JsonProperty("content")]
            public string Content { get; set; }
        }
        [Group("discord-changelog", "The latest Discord changelog")]
        public class DiscordChangelogGroup : InteractionModuleBase<SocketInteractionContext>
        {
            HttpClient client = new();
            [SlashCommand("desktop", "desktop changelog")]
            public async Task HandleDesktop()
            {
                //Get the version 
                string versionUrl = "https://cdn.discordapp.com/changelogs/config_0.json";
                string versionRes = await client.GetStringAsync(versionUrl);
                JObject versionObj = JObject.Parse(versionRes);
                string versionString = versionObj.Properties().First().Name;

                string changelogUrl = "https://cdn.discordapp.com/changelogs/0/" + versionString + "/en-US.json";
                string changelogRes = await client.GetStringAsync(changelogUrl);
                Changelog changelog = JsonConvert.DeserializeObject<Changelog>(changelogRes);

                string changelogDate = changelog.Date.ToString("MMMM dd, yyyy");

                EmbedBuilder changelogEmbed = new()
                {
                    Title = "Changelog for " + changelogDate,
                    Description = RemoveDiscordFormatting(changelog.Content),
                    Color = Colors.Random()
                };

                if (changelog.Asset.StartsWith("http"))
                {
                    changelogEmbed.ImageUrl = changelog.Asset;
                }

                await RespondAsync(embed: changelogEmbed.Build());
            }
            [SlashCommand("mobile", "mobile changelog")]
            public async Task HandleMobile()
            {
                //Get the version 
                string versionUrl = "https://cdn.discordapp.com/changelogs/config_1.json";
                string versionRes = await client.GetStringAsync(versionUrl);
                JObject versionObj = JObject.Parse(versionRes);
                string versionString = versionObj.Properties().First().Name;

                string changelogUrl = "https://cdn.discordapp.com/changelogs/1/" + versionString + "/en-US.json";
                string changelogRes = await client.GetStringAsync(changelogUrl);
                Changelog changelog = JsonConvert.DeserializeObject<Changelog>(changelogRes);

                string changelogDate = changelog.Date.ToString("MMMM dd, yyyy");

                EmbedBuilder changelogEmbed = new()
                {
                    Title = "Changelog for " + changelogDate,
                    Description = RemoveDiscordFormatting(changelog.Content),
                    Color = Colors.Random()
                };

                if (changelog.Asset.StartsWith("http"))
                {
                    changelogEmbed.ImageUrl = changelog.Asset;
                }

                await RespondAsync(embed: changelogEmbed.Build());
            }

            public string RemoveDiscordFormatting(string input)
            {
                Regex regex = new("{(added marginTop|progress)}");
                return regex.Replace(input, "");
            }
        }
    }
}