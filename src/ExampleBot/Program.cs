using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.EventQueue;
using Discord.WebSocket;

namespace ExampleBot
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public DiscordSocketClient Discord { get; set; }

        public async Task MainAsync()
        {
            string token = Environment.GetEnvironmentVariable("TOKEN");

            Discord = new DiscordSocketClient();

            Discord.Log += (msg) => { Console.WriteLine(msg); return Task.CompletedTask; };

            var queue = new EventQueue<DiscordSocketClient>(Discord);
            queue.Register(nameof(Discord.Ready));
            queue.Register(nameof(Discord.MessageReceived));

            var cts = new CancellationTokenSource();
            _ = ProcessEvents(queue, cts.Token);

            await Discord.LoginAsync(TokenType.Bot, token);
            await Discord.StartAsync();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;

                cts.Cancel();
            };

            await Task.Delay(-1, cts.Token);

            await Discord.SetStatusAsync(UserStatus.Invisible);
            await Discord.StopAsync();
        }

        public async Task ProcessEvents(EventQueue<DiscordSocketClient> queue, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                while (queue.Events.TryDequeue(out var ev))
                {
                    Console.WriteLine($"event {ev.Name.ToLower()}");

                    if (ev.Name == nameof(DiscordSocketClient.MessageReceived))
                    {
                        var msg = (SocketMessage)ev.Argument;

                        // larger bots may want to implement a work queue here, instead of running everything sequentially
                        await MessageReceivedAsync(msg);
                    }
                }
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
        }

        public async Task MessageReceivedAsync(SocketMessage msg)
        {
            // hook to commands here, if you want
            if (msg.Content == $"<@{Discord.CurrentUser.Id}> ping")
                await msg.Channel.SendMessageAsync("pong!");
        }
    }
}
