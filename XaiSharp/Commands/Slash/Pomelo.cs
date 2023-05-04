using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using System.Security.Policy;

namespace XaiSharp.Commands.Slash
{
    public class Pomelo : InteractionModuleBase<SocketInteractionContext>
    {
        public class PomeloResponse
        {
            public string Status { get; set; }
            public string Message { get; set; }
        }

        HttpClient client = new();
        [SlashCommand("pomelo", "Check if a pomelo name is available")]
        public async Task Handle([Summary("name", "name to check")]string? name = null)
        {
            string nameFinal = name ?? Context.User.Username;

            string url = "https://pomelo-checker.onrender.com/check?pomelo="+nameFinal;
            string result = await client.GetStringAsync(url);
            PomeloResponse data =  JsonConvert.DeserializeObject<PomeloResponse>(result);

            var pomeloEmbed = new EmbedBuilder()
                .WithFooter("Data comes from https://pomelo-checker.onrender.com");

            switch (data.Status)
            {
                case "available":
                    {
                        pomeloEmbed.Color = Colors.Positive;
                        pomeloEmbed.Description = $"✅ **@{nameFinal.ToLower()}** is available!";
                        break;
                    }
                case "available_ratelimit":
                    {
                        pomeloEmbed.Color = Colors.Positive;
                        pomeloEmbed.Description = $"✅ **@{nameFinal.ToLower()}** is available! (Rate limited)";
                        break;
                    }
                case "unavailable":
                    {
                        pomeloEmbed.Color = Colors.Negative;
                        pomeloEmbed.Description = $"❌ **@{nameFinal.ToLower()}** is unavailable. {data.Message}";
                        break;
                    }
            }
            await RespondAsync(embed: pomeloEmbed.Build());
        }
    }
}
