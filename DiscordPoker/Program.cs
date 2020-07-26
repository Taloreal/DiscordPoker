using System;
using System.IO;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Commands_Framework.Services;
using System.Linq;

namespace DiscordPoker {

    public class Program {

        public static string Token = "NzM0ODkxNTg5Mjk3NjM1NDk5.XxYUCw.9EqLosJ36ZXB0AlxEyTs4HM3OKU";
        public static Table Game = new Table();
        private static double Default_Cash = 1000;
        public static List<Player> Players = new List<Player>();
        public static List<Command> Commands = new List<Command>() { 
            new Command(">register", async arg => { await Command_Register(arg); }),
                                     
            //Testing Commands       
            new Command(">clr",      async arg => { await Command_Clear(arg); }),
            new Command(">next ",    async arg => { await Command_NextCard(arg); }),
            new Command(">shuffle",  async arg => { await Command_Shuffle(arg); }),
        };

        private static async Task MessageReceived(SocketMessage arg) {
            string msg = arg.Content.ToLower();
            for (int i = 0; i < Commands.Count; i++) {
                if (msg.StartsWith(Commands[i].Activator)) {
                    Commands[i].Function(arg);
                }
            }
        }

        private static async Task Command_Register(SocketMessage arg) {
            if (Players.Any(p => p.ID == arg.Author.Id)) {
                await arg.Channel.SendMessageAsync("ERROR: You're already registered.");
                return;
            }
            Player user = new Player(arg.Author.Username, arg.Author.Id);
            user.Give(Default_Cash);
            Players.Add(user);
            await arg.Channel.SendMessageAsync(
                "SUCCESS! You've registered and have been granted $" + Default_Cash.ToString() + ".");
        }

        #region TESTING_COMMANDS
        private static async Task Command_NextCard(SocketMessage arg) {
            if (!int.TryParse(arg.Content.Substring(6), out int amount)) { return; }
            List<Card> cards = Game.Dek.DrawCards(amount);
            Bitmap canvas = Card.DrawCards(cards);
            await SendBitmap(arg, canvas, "");
        }

        private static async Task Command_Shuffle(SocketMessage arg) {
            Game.Dek.Shuffle();
            await arg.Channel.SendMessageAsync("The deck has been shuffled.");
        }

        private static async Task Command_Clear(SocketMessage arg) {
            try {
                if (arg.Content == ">clr") { await Command_Clear_BruteForce(arg); return; }
                string at = arg.Author.Discriminator;
                string chan = arg.Channel.Name;
                if (chan.EndsWith("#" + at)) { await Command_PrivateClear(arg); return; }
                string msg = arg.Content.ToLower();
                if (!msg.StartsWith(">clr ") || msg.Length < 6) { return; }
                if (!int.TryParse(msg.Substring(5), out int num)) { return; }
                IEnumerable<IMessage> msgs = await arg.Channel.GetMessagesAsync(num).FlattenAsync();
                foreach (IMessage m in msgs) { await m.DeleteAsync(); }
            } catch (Exception e) { }
        }

        private static async Task Command_Clear_BruteForce(SocketMessage arg) {
            IEnumerable<IMessage> msgs = await arg.Channel.GetMessagesAsync(10).FlattenAsync();
            foreach (IMessage m in msgs) {
                try { await m.DeleteAsync(); }
                catch { continue; }
            }
        }

        private static async Task Command_PrivateClear(SocketMessage arg) {
            string msg = arg.Content.ToLower();
            if (!msg.StartsWith(">clr ") || msg.Length < 6) { return; }
            if (!int.TryParse(msg.Substring(5), out int num)) { return; }
            IEnumerable<IMessage> msgs = await arg.Channel.GetMessagesAsync(num).FlattenAsync();
            foreach (IMessage m in msgs) { if (m.Author.IsBot) { await m.DeleteAsync(); } }
        }
        #endregion

        private static async Task SendBitmap(SocketMessage ogMsg, Bitmap bmp, string text) {
            bmp.Save(@"temp.png", System.Drawing.Imaging.ImageFormat.Png);
            await ogMsg.Channel.SendFileAsync("temp.png", text);
        }

        #region BOT_CORE
        public static void Main(string[] args) {
            Players = Player.Remember_Players();
            Client = Services.GetRequiredService<DiscordSocketClient>();
            MainAsync().GetAwaiter().GetResult();
        }

        public static ServiceProvider Services = ConfigureServices();
        public static DiscordSocketClient Client;
        public static async Task MainAsync() {
            Client.MessageReceived += MessageReceived;
            await Client.LoginAsync(TokenType.Bot, Token);
            await Client.StartAsync();
            await Services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            await Task.Delay(-1);
        }

        private static ServiceProvider ConfigureServices() {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .BuildServiceProvider();
        }
        #endregion
    }
}
