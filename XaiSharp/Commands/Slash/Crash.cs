using Discord;
using Discord.Interactions;

namespace XaiSharp.Commands.Slash
{
    [IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    public class Crash : InteractionModuleBase<SocketInteractionContext>
    {

        [SlashCommand("crash", "Crash the bot... maybe")]
        public async Task Handle()
        {
            await RespondAsync("Crashing the bot in 3...");
            await Task.Delay(1000);
            await ModifyOriginalResponseAsync(m => m.Content = "Crashing the bot in 2...");
            await Task.Delay(1000);
            await ModifyOriginalResponseAsync(m => m.Content = "Crashing the bot in 1...");
            await Task.Delay(1000);
            await ModifyOriginalResponseAsync(m => m.Content = $"{Context.User.Username} is not in the sudoers file. This incident will be reported.");
        }
    }
}
