using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using System.Net;
namespace XaiSharp.Commands.Slash
{
    public class CtrlAltDel : InteractionModuleBase<SocketInteractionContext>
    {
        [IntegrationType(ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall)]
        [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
        [Group("cad", "That other webcomic")]
        public class CtrlAltDelGroup : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("latest", "Latest comic")]
            public async Task HandleLatest()
            {

                EmbedBuilder cadEmbed = new()
                {
                    Title = $"Loss",
                    Url = "https://cad-comic.com/comic/loss/",
                    ImageUrl = "https://cad-comic.com/wp-content/uploads/2017/03/cad-20080602-358b1.x68566.jpg",
                    Color = Convert.ToUInt32(Util.CreateMD5Hash("Loss")[..6], 16),
                    Footer = new()
                    {
                        Text = Util.CreateDateString("2008", "6", "2")
                    }//,
                    //Timestamp = new DateTimeOffset(Convert.ToInt32(data.year), Convert.ToInt32(data.month), Convert.ToInt32(data.day), 0, 0, 0, new TimeSpan(0))
                };

                await RespondAsync(embed: cadEmbed.Build());
            }

            [SlashCommand("random", "A random comic")]
            public async Task HandleRandom()
            {
                EmbedBuilder cadEmbed = new()
                {
                    Title = $"Loss",
                    Url = "https://cad-comic.com/comic/loss/",
                    ImageUrl = "https://cad-comic.com/wp-content/uploads/2017/03/cad-20080602-358b1.x68566.jpg",
                    Color = Convert.ToUInt32(Util.CreateMD5Hash("Loss")[..6], 16),
                    Footer = new()
                    {
                        Text = Util.CreateDateString("2008", "6", "2")
                    }//,
                    //Timestamp = new DateTimeOffset(Convert.ToInt32(data.year), Convert.ToInt32(data.month), Convert.ToInt32(data.day), 0, 0, 0, new TimeSpan(0))
                };

                await RespondAsync(embed: cadEmbed.Build());
            }

            [SlashCommand("search", "Search for a comic")]
            public async Task HandleSearch(int number)
            {
                EmbedBuilder cadEmbed = new()
                {
                    Title = $"Loss",
                    Url = "https://cad-comic.com/comic/loss/",
                    ImageUrl = "https://cad-comic.com/wp-content/uploads/2017/03/cad-20080602-358b1.x68566.jpg",
                    Color = Convert.ToUInt32(Util.CreateMD5Hash("Loss")[..6], 16),
                    Footer = new()
                    {
                        Text = Util.CreateDateString("2008", "6", "2")
                    }//,
                    //Timestamp = new DateTimeOffset(Convert.ToInt32(data.year), Convert.ToInt32(data.month), Convert.ToInt32(data.day), 0, 0, 0, new TimeSpan(0))
                };

                await RespondAsync(embed: cadEmbed.Build());
            }
        }
    }
}
