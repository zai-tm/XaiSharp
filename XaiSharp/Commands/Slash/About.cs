using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    [IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
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

            try
            {
                Console.WriteLine(ThisAssembly.Git.RepositoryUrl);
                string sha = ThisAssembly.Git.Commit;
                DateTimeOffset commitDate = DateTimeOffset.Parse(ThisAssembly.Git.CommitDate);
                DateTime commitDateUtc = commitDate.ToUniversalTime().DateTime;
                aboutEmbed.Description = $"Version {commitDateUtc.ToString("yyyy-MM-dd HH_mm_ss")} \\(commit [`{sha[..7]}`]({ThisAssembly.Git.RepositoryUrl}/commit/{sha})\\)";
                aboutEmbed.Color = Convert.ToUInt32(sha[..6], 16);
                await RespondAsync(embed: aboutEmbed.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


        }
    }
}
