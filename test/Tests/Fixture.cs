using System;
using Discord.Addons.EventQueue;
using Discord.WebSocket;

namespace Tests
{
    public class TestFixture : IDisposable
    {
        public EventQueue<BaseSocketClient> Base { get; }
        public EventQueue<DiscordSocketClient> Discord { get; }
        public EventQueue<DiscordShardedClient> Sharded { get; }

        public DiscordSocketClient Client { get; }

        public TestFixture()
        {
            var @base = (BaseSocketClient)(new DiscordSocketClient());
            var discord = new DiscordSocketClient();
            var sharded = new DiscordShardedClient();

            Base = new EventQueue<BaseSocketClient>(@base);
            Discord = new EventQueue<DiscordSocketClient>(discord);
            Sharded = new EventQueue<DiscordShardedClient>(sharded);

            Client = discord;
        }

        void IDisposable.Dispose()
        {
        }
    }
}
