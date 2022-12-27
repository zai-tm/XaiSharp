using Discord;
using Discord.Interactions;

namespace XaiSharp.Commands.Slash
{
    public class Dice : InteractionModuleBase<SocketInteractionContext>
    {

        [SlashCommand("dice", "Rolls a dice!")]
        public async Task Handle(long? sides = null)
        {
            Random random = new();
            long roll = random.NextInt64(1, 6);
            if (sides != null)
            {
                if (sides <= 0)
                    await RespondAsync("You cannot enter 0 or a negative number!", ephemeral: true);
                else
                    roll = random.NextInt64(1, sides.Value + 1);
            }

            var diceEmbed = new EmbedBuilder()
                .WithTitle("Dice")
                .WithColor(0xffffff)
                .AddField("The dice landed on...", $"**{roll}**");
            await DeferAsync();
            await FollowupAsync(embed: diceEmbed.Build());
        }
    }
}
