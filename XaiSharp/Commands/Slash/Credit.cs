using Discord;
using Discord.Interactions;

namespace XaiSharp.Commands.Slash
{
    [IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    public class Credit : InteractionModuleBase<SocketInteractionContext>
    {

        [SlashCommand("credit", "What is your Progress Credit™?")]
        public async Task Handle()
        {
            Random random = new();
            var image = $"https://zai-tm.github.io/credit/{random.Next(0, 9)}.png";
            var creditEmbed = new EmbedBuilder()
                .WithTitle("Progress Credit™")
                .WithImageUrl(image)
                .WithCurrentTimestamp();

            await RespondAsync(embed: creditEmbed.Build());
        }
    }
}
