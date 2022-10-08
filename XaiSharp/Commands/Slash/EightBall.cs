using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace XaiSharp.Commands.Slash
{
    public class EightBall : InteractionModuleBase<SocketInteractionContext>
    {

        [SlashCommand("8-ball", "Ask the magic 8-ball a question")]
        public async Task Handle(string question)
        {
            Random random = new();
            var responses = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("JSON/8ball.json")).ToArray();
            var eightBallEmbed = new EmbedBuilder()
                .WithColor(0x000000)
                .WithTitle("Magic 8-ball")
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/857415925324840960/1023778034936979456/unknown.png")
                .AddField("You asked:", question)
                .AddField("The 8-ball says", responses[random.Next(0, responses.Length)]);
            if (question.Length > 256)
                await RespondAsync("Your question is too long.", ephemeral: true);
            else
                await RespondAsync(embed: eightBallEmbed.Build());

        }
    }
}
