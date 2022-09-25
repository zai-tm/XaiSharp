using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XaiSharp.Modules
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
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

        [SlashCommand("two", "wow there are 2 commands now")]
        public async Task HandleTwoCommands()
        {
            await RespondAsync("2 (reference to BATTLE FOR DREAM ISALAND)))!!!!!!!!!");
        }
    }
}
