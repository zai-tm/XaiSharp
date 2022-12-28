using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class About : InteractionModuleBase<SocketInteractionContext>
    {
        Config config = Util.ParseConfig(File.ReadAllText("config.yml"));
        [SlashCommand("about", "About this bot")]
        public async Task Handle()
        {
            EmbedBuilder aboutEmbed = new()
            {
                Author = new()
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Name = Context.Client.CurrentUser.Username + "#" + Context.Client.CurrentUser.Discriminator
                },
                Title = "XaiSharp"
            };

            string url = $"https://api.github.com/repos/{config.Repository}/commits?per_page=1";
            string[] GithubDetails = config.Repository.Split('/');

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.GithubToken}");
                    client.DefaultRequestHeaders.UserAgent.Add(new(GithubDetails[1], null));
                    client.DefaultRequestHeaders.UserAgent.Add(new($"(+https://github.com/{config.Repository})"));
                    var response = client.GetAsync(url).Result;
                    var json = response.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine(json);
                    dynamic commits = JsonConvert.DeserializeObject(json);
                    string sha = commits[0].sha;
                    //Console.WriteLine(commits[0].commit.author.date.ToString("yyyy-MM-dd HH_mm_ss"));
                    aboutEmbed.Description = $"Version {commits[0].commit.author.date.ToString("yyyy-MM-dd HH_mm_ss")} \\(commit [`{sha[..6]}`]({commits[0].html_url})\\)";
                    aboutEmbed.Color = Convert.ToUInt32(sha[..6], 16);
                    await RespondAsync(embed: aboutEmbed.Build());
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }


        }
    }
}
