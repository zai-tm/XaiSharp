using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


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
        [SlashCommand("discord-changelog", "The latest Discord changelog")]
        public async Task Handle()
        {
            HttpClient client = new();

            //Get the version 
            string versionUrl = "https://cdn.discordapp.com/changelogs/config_0.json";
            string versionRes = await client.GetStringAsync(versionUrl);
            JObject versionObj = JObject.Parse(versionRes);
            string versionString = versionObj.Properties().First().Name;

            string changelogUrl = "https://cdn.discordapp.com/changelogs/0/"+versionString+"/en-US.json";
            string changelogRes = await client.GetStringAsync(changelogUrl);
            Changelog changelog = JsonConvert.DeserializeObject<Changelog>(changelogRes);

            string changelogDate = changelog.Date.ToString("D");

            EmbedBuilder changelogEmbed = new()
            {
                Title = "Changelog for " + changelogDate,
                Description = RemoveRandomDiscordFormattingStuff(changelog.Content),
                ImageUrl = changelog.Asset,
                Color = Colors.Random()
            };

            await RespondAsync(embed: changelogEmbed.Build());
        }
        public string RemoveRandomDiscordFormattingStuff(string input)
        {
            return input.Replace("{added margintop}", "");
        }
    }

}
