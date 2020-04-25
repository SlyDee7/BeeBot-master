using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeBot.Core.UserAccounts
{
    public class UserAccount
    {

        public ulong ID { get; set; }

        public uint Points { get; set; }

        public uint XP { get; set; }

        public uint level { get; set; }

        public uint LevelNumber
        {
            get
            {
                return (uint)Math.Sqrt(XP / 50); //y(XP) = x(Level) ^ 2 * 50

            }
        }
        public uint NewLevel
        {
            get
            {
                return (uint)Math.Pow(XP, 2) * 50;
            }
        }

        internal class TimeSinceLastMessage
        {
        }

        public bool IsMuted { get; set; }

        public uint NumberOfWarnings { get; set; }
    }






}



