using System;
using System.Threading.Tasks;
using Discord.Addons.EventQueue;
using Discord.WebSocket;
using Xunit;

namespace Tests
{
    // xunit doesn't work with the reflection magic i'm using
    // or it's haunted
    //
    // todo: investigate a way to make unit tests work
    /*
    public class Tests : IClassFixture<TestFixture>
    {
        private EventQueue<BaseSocketClient> _base;
        private EventQueue<DiscordSocketClient> _discord;
        private EventQueue<DiscordShardedClient> _sharded;

        private DiscordSocketClient _client;

        public Tests(TestFixture fixture)
        {
            _base = fixture.Base;
            _discord = fixture.Discord;
            _sharded = fixture.Sharded;

            _client = fixture.Client;
        }

        [Fact]
        public void Test_Register()
        {
            // should pass
            _base.Register(nameof(BaseSocketClient.LoggedIn)); // a/1
            _base.Register(nameof(BaseSocketClient.Log)); // a/2
            _base.Register(nameof(BaseSocketClient.MessageDeleted)); // a/3
            _base.Register(nameof(BaseSocketClient.MessageUpdated)); // a/4

            _discord.Register(nameof(DiscordSocketClient.LoggedIn));
            _discord.Register(nameof(DiscordSocketClient.Ready));
            _sharded.Register(nameof(DiscordShardedClient.ShardReady));

            // should fail

            // re-register event
            Assert.Throws<InvalidOperationException>(() =>
            {
                _base.Register(nameof(BaseSocketClient.LoggedIn));
            });
            // invalid event
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                _sharded.Register("this is fake");
            });
        }

        [Fact]
        public async Task Test_Hooked()
        {
            await _client.LoginAsync(Discord.TokenType.Bot, "die");

            Assert.True(_discord.Events.Count > 0);
            var a = _discord.Events.TryDequeue(out var ev);
            Assert.True(a);
            Assert.True(_discord.Events.Count == 0);

            Assert.Equal(nameof(DiscordSocketClient.LoggedIn), ev.Name);
            Assert.IsType<Unit>(ev.Argument);
        }
    }*/
}
