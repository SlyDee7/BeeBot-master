using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

//namespace BeeBot.Core
/*{
   internal static class RepeatingTimer
    {
        private static Timer loopingTimer;
        private static SocketTextChannel channel;
        internal  static Task StartTimer()
        {
            channel = Global.Client.GetGuild(451805295082209300).GetTextChannel(451805295082209302);

            loopingTimer = new Timer()
            {
                Interval = 5000,
                AutoReset = true,
                Enabled = true

            };
            loopingTimer.Elapsed += OnTimerTicked;

            return Task.CompletedTask;
        }

        private static async void OnTimerTicked(object sender, ElapsedEventArgs e)
        {
           // await channel.SendMessageAsync("ping!");
        }
    }
}*/
