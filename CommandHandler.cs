using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using BeeBot.Core.LevelingSystem;
using BeeBot.Core.MonsterHunt;

namespace BeeBot
{
    class CommandHandler
    {
        DiscordSocketClient _client;
        CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {

            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommandAsync;

        }
        //await _client.SetGameAsync(Utilities.GetAlert("*game*"));

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;

            BeeBot.Core.MonsterHunt.MonsterHunt.RunLogic();

            // Mute check
           /* var userAccount = UserAccounts.GetAccount(context.User);
            if (userAccount.IsMuted)
            {
                await context.Message.DeleteAsync();
                return;
            }*/

            // Leveling up
            Leveling.OnMessageReceive((SocketGuildUser)context.User, (SocketTextChannel)context.Channel);

            int argPos = 0;
            if (msg.HasStringPrefix(Config.bot.cmdPrefix, ref argPos)
                || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }

            MonsterHunt.RunLogic();
        }
    }
}
