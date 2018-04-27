using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace ExampleBot
{
    public class Program
    {
        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client = new DiscordSocketClient();

        private CommandHandler handler = new CommandHandler();

        public async Task StartAsync()
        {
            await client.LoginAsync(TokenType.Bot, "INSERT BOT TOKEN HERE");

            await client.StartAsync();

            await handler.InitializeAsync(client);

            await Task.Delay(-1);
        }
    }
}
