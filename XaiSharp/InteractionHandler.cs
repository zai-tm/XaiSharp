using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace XaiSharp
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;

        public InteractionHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += HandleInteraction;
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var context = new SocketInteractionContext(_client, arg);
                await _commands.ExecuteCommandAsync(context, _services);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
