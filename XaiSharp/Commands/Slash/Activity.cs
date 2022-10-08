using Discord;
using Discord.Interactions;

namespace XaiSharp.Commands.Slash
{
    public class Activity : InteractionModuleBase<SocketInteractionContext>
    {
        private ulong[] activities = {
                755827207812677713, //< Poker Night
                832012774040141894, //< Chess In The Park
                832013003968348200, //< Checkers In The Park
                832025144389533716, //< Blazing 8s
                852509694341283871, //< SpellCast
                879863686565621790, //< Letter League
                879863976006127627, //< Word Snacks
                880218394199220334, //< Watch Together
                902271654783242291, //< Sketch Heads
                903769130790969345, //< Land-io
                945737671223947305, //< Putt Party
                947957217959759964, //< Bobble League
                950505761862189096, //< Know What I Meme
                976052223358406656, //< Ask Away
               1006584476094177371, //< Bash Out
            };

        private string[] activityStr =
        {
                "Poker Night",
                "Chess In The Park",
                "Checkers In The Park",
                "Blazing 8s",
                "SpellCast",
                "Letter League",
                "Word Snacks",
                "Watch Together",
                "Sketch Heads",
                "Land-io",
                "Putt Party",
                "Bobble League",
                "Know What I Meme",
                "Ask Away",
                "Bash Out",
            };

        [SlashCommand("activity", "Play a voice chat activity because the Discord API lets you even if the server doesn't have them!")]
        public async Task HandleActivityCommand(
            [ChannelTypes(ChannelType.Voice)] IChannel channel,
            [
            Choice ("Poker Night", 0),
            Choice ("Chess In The Park", 1),
            Choice ("Checkers In The Park", 2),
            Choice ("Blazing 8s", 3),
            Choice ("SpellCast", 4),
            Choice ("Letter League", 5),
            Choice ("Word Snacks (Free)", 6),
            Choice ("Watch Together (Free)", 7),
            Choice ("Sketch Heads (Free)", 8),
            Choice ("Land-io", 9),
            Choice ("Putt Party", 10),
            Choice ("Bobble League", 11),
            Choice ("Know What I Meme", 12),
            Choice ("Ask Away (Free)", 13),
            Choice ("Bash Out", 14),

            ] int activity)
        {

            var invite = await Context.Guild.GetVoiceChannel(channel.Id).CreateInviteToApplicationAsync(applicationId: activities[activity], maxAge: null);

            var joinActivityButton = new ButtonBuilder()
            {
                Label = $"Join {activityStr[activity]}",
                //CustomId = "join",
                Url = invite.ToString(),
                Style = ButtonStyle.Link
            };

            var component = new ComponentBuilder()
            .WithButton(joinActivityButton)
            .Build();

            if (((activity <= 5 || activity >= 9) && activity != 13) && Context.Guild.PremiumTier == PremiumTier.None)
                await RespondAsync($"This server needs a boost level of at least **1** to play {activityStr[activity]}.", ephemeral: true);
            else
                await RespondAsync($"Click the button below to join {activityStr[activity]}!", components: component);
        }
    }
}
