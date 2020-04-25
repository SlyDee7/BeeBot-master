using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Runtime.Remoting.Contexts;
using BeeBot.Core.UserAccounts;
using Discord.Commands;
using BeeBot.Core;

namespace BeeBot
{
    class Program1
    {
        DiscordSocketClient _client;
        CommandHandler _handler;
        // private SocketUser user;

        static void Main(string[] args)
        => new Program1().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (Config.bot.token == "" || Config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            _client.UserJoined += AnnounceJoinedUser;  //Check if userjoined
            _client.Log += Log;
           // _client.Ready += RepeatingTimer.StartTimer;
            _client.ReactionAdded += OnReactionAdded_TestLocation;
            _client.ReactionAdded += OnReactionAdded_TestSex;
            _client.ReactionAdded += OnReactionAdded_NSFW;
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            Global.Client = _client;
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            await Task.Delay(-1);
        }


        public async Task AnnounceJoinedUser(SocketGuildUser user)//welcomes New Players
        {
            {
                string[] banner = { "welcomebanner1", "welcomebanner2", "welcomebanner1" }; // string system
                Random r = new Random(); // randomizes it
                string selection = banner[r.Next(0, banner.Length)]; // selects one
                var channel = _client.GetChannel(0) as SocketTextChannel; //gets channel to send message in //Welcomes the new user (Copy ID) (0) << Replace
                var embed = new EmbedBuilder()
                .WithImageUrl(Utilities.GetAlert(selection))
                .WithDescription(Utilities.GetAlert("welcome1") + user.Mention);
                await channel.SendMessageAsync("", embed: embed);
            }
        }

        private async Task OnReactionAdded_TestLocation(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            
            if (reaction.MessageId == Global.LocationMessageIdToTrack)
            {
                string[] location_role_arr = { "North America", "South America", "Europe", "Asia", "Africa", "Australia" };
                string[] location_emot_arr = { "1⃣", "2⃣", "3⃣", "4⃣", "5⃣", "6⃣" };
                List<SocketRole> UserRolesRemoval = new List<SocketRole>();

                for (int i = 0; i < 6; i++)
                {
                    if (reaction.Emote.Name == location_emot_arr[i])
                    {
                        SocketGuildUser Current_User = (SocketGuildUser)reaction.User;
                        if (null == Current_User) return;
      
                        SocketRole Role_To_Add = Current_User.Guild.Roles.FirstOrDefault(x => x.Name.Contains(location_role_arr[i])) as SocketRole;
                        if (null == Role_To_Add) return;
                        

                        /* Add all of our current location roles to the list */
                        foreach (SocketRole myRole in Current_User.Roles.ToArray())
                        {
                            if (location_role_arr.Contains(myRole.Name))
                            {
                                
                                UserRolesRemoval.Add(myRole);
                            }
                        }
                        
                        /* Throw them all in the trash */
                        if (UserRolesRemoval.Count > 0)
                        {
                            
                            await Current_User.RemoveRolesAsync(UserRolesRemoval);
                        }
                        
                        await Current_User.AddRoleAsync(Role_To_Add);
                        
                        break;
                    }
                }
            }
        }


        private async Task OnReactionAdded_TestSex(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.MessageId == Global.SexMessageIdToTrack)
            {
                string[] sex_role_arr = { "Male", "Female" };
                string[] sex_emot_arr = { "🚹", "🚺" };
                List<SocketRole> UserRolesRemoval = new List<SocketRole>();

                for (int i = 0; i < 2; i++)
                {

                    if (reaction.Emote.Name == sex_emot_arr[i])
                    {
                        SocketGuildUser Current_User = (SocketGuildUser)reaction.User;
                        if (null == Current_User) return;
                       
                        SocketRole Role_To_Add = Current_User.Guild.Roles.FirstOrDefault(x => x.Name.Contains(sex_role_arr[i])) as SocketRole;
                        if (null == Role_To_Add) return;
                       

                        /* Add all of our current location roles to the list */
                        foreach (SocketRole myRole in Current_User.Roles.ToArray())
                        {
                            if (sex_role_arr.Contains(myRole.Name))
                            {
                                UserRolesRemoval.Add(myRole);
                            }
                        }

                        /* Throw them all in the trash */
                        if (UserRolesRemoval.Count > 0)
                        {
                            await Current_User.RemoveRolesAsync(UserRolesRemoval);
                        }

                        await Current_User.AddRoleAsync(Role_To_Add);

                        break;
                    }
                }
            }
        }

        private async Task OnReactionAdded_NSFW(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.MessageId == Global.NSFWIdToTrack)
            {
                string[] nsfw_role_arr = { "Clean", "Toxic" };
                string[] nsfw_emot_arr = { "✅", "⛔" };
                List<SocketRole> UserRolesRemoval = new List<SocketRole>();

                for (int i = 0; i < 2; i++)
                {

                    if (reaction.Emote.Name == nsfw_emot_arr[i])
                    {
                        SocketGuildUser Current_User = (SocketGuildUser)reaction.User;
                        if (null == Current_User) return;

                        SocketRole Role_To_Add = Current_User.Guild.Roles.FirstOrDefault(x => x.Name.Contains(nsfw_role_arr[i])) as SocketRole;
                        if (null == Role_To_Add) return;
                        //updated this
                        /* Add all of our current location roles to the list */
                        foreach (SocketRole myRole in Current_User.Roles.ToArray())
                        {
                            if (nsfw_role_arr.Contains(myRole.Name))
                            {
                                UserRolesRemoval.Add(myRole);
                            }
                        }

                        /* Throw them all in the trash */
                        if (UserRolesRemoval.Count > 0)
                        {
                            await Current_User.RemoveRolesAsync(UserRolesRemoval);
                        }

                        await Current_User.AddRoleAsync(Role_To_Add);

                        break;
                    }
                }
            }
        }



        private async Task Log(LogMessage msg)
            {
            Console.WriteLine(msg.Message);
            }
    }
}