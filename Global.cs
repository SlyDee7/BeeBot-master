using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeBot
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static ulong LocationMessageIdToTrack { get; set; }
        internal static ulong SexMessageIdToTrack { get; set; }
        internal static ulong NSFWIdToTrack { get; set; }
    }
}
