using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace XaiSharp.Commands.User
{
    [IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    public class Kill : InteractionModuleBase<SocketInteractionContext>
    {
        private class DeathMessage
        {
            public List<string> Message { get; set; }
        }

        [UserCommand("Kill")]
        public async Task Handle(IUser user)
        {
            Random random = new();
            List<DeathMessage> messages = JsonConvert.DeserializeObject<List<DeathMessage>>(File.ReadAllText("JSON/death.json"));
            var messagesArr = messages.ToArray();
            //Console.WriteLine(messages);
            var deathMessageEmbed = new EmbedBuilder()
                .WithTitle("You Died!")
                .WithColor(0x910700);
            if (random.Next(0, 2) == 0)
            {
                deathMessageEmbed.Description = $"{user.Mention} {messagesArr[0].Message.ToArray()[random.Next(0, messagesArr[0].Message.Count)]} {Context.User.Mention}";
            }
            else
            {
                deathMessageEmbed.Description = $"{user.Mention} {messagesArr[1].Message.ToArray()[random.Next(0, messagesArr[1].Message.Count)]}";
            }
            await RespondAsync(embed: deathMessageEmbed.Build());
        }
    }
}
