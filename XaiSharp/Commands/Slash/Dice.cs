using Discord;
using Discord.Interactions;

namespace XaiSharp.Commands.Slash
{
    public class Dice : InteractionModuleBase<SocketInteractionContext>
    {

        [SlashCommand("dice", "Rolls a dice!")]
        public async Task Handle(int? sides = null)
        {
            Random random = new();
            var roll = random.Next(1, 6);
            if (sides != null)
            {
                if (sides <= 0)
                    await RespondAsync("You cannot enter 0 or a negative number!", ephemeral: true);
                else
                    roll = random.Next(1, sides.Value+1);
            }

            var diceEmbed = new EmbedBuilder()
                .WithTitle("Dice")
                .WithColor(0xffffff)
                .AddField("The dice landed on...", $"**{roll}**");
            await RespondAsync(embed: diceEmbed.Build());
        }
    }
}
