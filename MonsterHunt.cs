using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using BeeBot.Core;
using BeeBot;
using BeeBot.Modules;
using BeeBot.Core.UserAccounts;
using System.Diagnostics;


namespace BeeBot.Core.MonsterHunt
{
    public class MonsterHunt
    {
        private static Random Randomizer = new Random();
        private static Stopwatch LogicTimer = new Stopwatch();
        private static bool IsGameStart = false;
        private static uint NumberOfDuck = 0;
        private static int TimeUntilNextWave_Minimum = 60; //3600
        private static int TimeUntilNextWave_Maximum = 65; //14400
        private static int TimeUntilEndWave = 360 * 1000;
        private static long TimeUntilNextWave = TimeUntilNextWave_Minimum * 1000;
        private static uint RewardExperience = 100;

        public struct S_ammunition
        {
            public byte Bullets;
            public byte Magazine;
        }

        public static Dictionary<ulong, S_ammunition> PlayerBulletTracker = new Dictionary<ulong, S_ammunition>();

        public static SocketTextChannel HunterChannel = BeeBot.Global.Client.GetChannel(0) as SocketTextChannel;    //!!SET THIS TO THE HUNTER CHANNEL!! (copy ID) (0) << replace

        internal static async void RunLogic()
        {
            /* if (false == LogicTimer.IsRunning)
             {
                 LogicTimer.Start();
             }*/

            if (IsGameStart)
            {
                if (0 == NumberOfDuck || LogicTimer.ElapsedMilliseconds > TimeUntilEndWave)
                {
                    IsGameStart = false;
                    // TimeUntilNextWave = Randomizer.Next(TimeUntilNextWave_Minimum, TimeUntilNextWave_Maximum) * 1000;

                    if (NumberOfDuck > 0)
                    {
                        await HunterChannel.SendMessageAsync("@here Notorious Monster has gotten away. Better luck next time!");
                    }
                    else
                    {
                        await HunterChannel.SendMessageAsync("@here Game Over.");
                    }


                    NumberOfDuck = 0;
                    LogicTimer.Stop();    //LogicTimer.Restart();
                    PlayerBulletTracker.Clear();
                }
            }
            /* else
             {
                 if (LogicTimer.ElapsedMilliseconds > TimeUntilNextWave)
                 {
                     IsGameStart = true;
                     NumberOfDuck = 1;

                     await HunterChannel.SendMessageAsync("@here A wild monster appears. kill it!");

                     LogicTimer.Restart();
                     PlayerBulletTracker.Clear();
                 }
             }*/
        }


        internal static async Task Shoot(SocketUser user)
        {
            if (false == IsGameStart || 0 == NumberOfDuck)
            {
                await HunterChannel.SendMessageAsync(user.Mention + (" You can only shoot during a game."));
                return;
            }

            var CurrentUser = UserAccounts.UserAccounts.GetAccount(user);
            if (null == CurrentUser) return;

            S_ammunition PlayerAmmo = new S_ammunition();
            PlayerGetAmmo(user.Id, ref PlayerAmmo);

            /* Check that the player has the bullets */
            if (0 == PlayerAmmo.Bullets)
            {
                await HunterChannel.SendMessageAsync(user.Mention + (Utilities.GetAlert((0 == PlayerAmmo.Magazine) ? "OutofAmmoMag" : "OutofAmmo")));
                return;
            }

            /* 95 % Chance to miss */
            if (95 > Randomizer.Next(0, 100))
            {

                await HunterChannel.SendMessageAsync(user.Mention + (Utilities.GetAlert("Missed")));
            }
            else
            {
                /* Hit the Monster! */
                NumberOfDuck--;
                await HunterChannel.SendMessageAsync(user.Mention + (Utilities.GetAlert("Hit")));

                BeeBot.Core.LevelingSystem.Leveling.AddExperience((SocketGuildUser)user, HunterChannel, RewardExperience, true);
            }

            /* Update their ammo count */
            PlayerSetAmmo(user.Id, PlayerAmmo);

            /* Take a bullet from their gun */
            if (0 == (PlayerAmmo.Bullets--))
            {
                //You need to reload?
            }

            /* Update their ammo count */
            PlayerSetAmmo(user.Id, PlayerAmmo);
        }


        internal static async Task Reload(SocketUser user)
        {
            var CurrentUser = UserAccounts.UserAccounts.GetAccount(user);
            if (null == CurrentUser) return;

            if (false == IsGameStart || 0 == NumberOfDuck)
            {
                await HunterChannel.SendMessageAsync(user.Mention + (" You can only reload during a game."));
                return;
            }

            S_ammunition PlayerAmmo = new S_ammunition();
            PlayerGetAmmo(user.Id, ref PlayerAmmo);

            if (0 != PlayerAmmo.Bullets)
            {
                await HunterChannel.SendMessageAsync(user.Mention + (" > You already have bullets in your gun."));
            }
            else
            {
                if (0 == PlayerAmmo.Magazine)
                {
                    await HunterChannel.SendMessageAsync(user.Mention + (Utilities.GetAlert("OutofAmmoMag")));
                }
                else
                {
                    if (25 > Randomizer.Next(0, 100))
                    {
                        await HunterChannel.SendMessageAsync(user.Mention + (Utilities.GetAlert("Jammed")));
                    }
                    else
                    {
                        /* Update their ammo count */
                        PlayerAmmo.Bullets = 5;
                        PlayerAmmo.Magazine = 0;
                        PlayerSetAmmo(user.Id, PlayerAmmo);

                        await HunterChannel.SendMessageAsync(user.Mention + (Utilities.GetAlert("Reload")));
                    }
                }
            }
        }

        public static void PlayerGetAmmo(ulong PlayerID, ref S_ammunition AmmoPack)
        {
            if (false == PlayerBulletTracker.TryGetValue(PlayerID, out AmmoPack))
            {
                Console.WriteLine("Debug: Add New Magazine");
                /* We don't have a record. Make a new one and give them 10 bullets! */
                AmmoPack.Bullets = 5;  //5 In Gun
                AmmoPack.Magazine = 1; //1 Mag

                PlayerBulletTracker.Add(PlayerID, AmmoPack);
            }
        }

        public static void PlayerSetAmmo(ulong PlayerID, S_ammunition AmmoPack)
        {
            Console.WriteLine("Debug: Update Player Ammo {0}/{1}", AmmoPack.Bullets, AmmoPack.Magazine);
            PlayerBulletTracker[PlayerID] = AmmoPack;
        }


        internal static async Task ForceStart(SocketUser user)
        {
            if (IsGameStart) return;

            IsGameStart = true;
            NumberOfDuck = 1;
            LogicTimer.Reset();
            LogicTimer.Start();
            PlayerBulletTracker.Clear();

            await HunterChannel.SendMessageAsync("@here A wild Notorious Monster appears. Destroy it fast!");
        }
    }
}