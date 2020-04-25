using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeBot.Core.UserAccounts;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using BeeBot;
using BeeBot.Core.MonsterHunt;

namespace BeeBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {

        [Command("warn")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task WarnUser(IGuildUser user, [Remainder] string reason = "No reason provided")
        {
            var userAccount = UserAccounts.GetAccount((SocketUser)user);
            userAccount.NumberOfWarnings++;
            UserAccounts.SaveAccounts();

            // punishment check
            if (userAccount.NumberOfWarnings >= 3)
            {
                var dmChannel = await user.GetOrCreateDMChannelAsync();
                await dmChannel.SendMessageAsync("You have been banned. Reason: " + reason);
                await user.Guild.AddBanAsync(user, 5);
                await Context.Channel.SendMessageAsync(user.Mention + " Has been banned. Reason: " + reason);
            }
            else if (userAccount.NumberOfWarnings == 2)
            {
                var dmChannel = await user.GetOrCreateDMChannelAsync();
                await dmChannel.SendMessageAsync("This is your final warning. Reason for warning: " + reason);
                await Context.Channel.SendMessageAsync(user.Mention + " Has been warned twice. Reason for warning: " + reason);
            }
            else if (userAccount.NumberOfWarnings == 1)
            {
                var dmChannel = await user.GetOrCreateDMChannelAsync();
                await dmChannel.SendMessageAsync("This is your first warning. Reason for warning: " + reason);
                await Context.Channel.SendMessageAsync(user.Mention + " Has been warned. Reason: " + reason);
            }
        }

        [Command("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUser(IGuildUser user, [Remainder]string reason = "No reason provided.")
        {
            var dmChannel = await user.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync("You have been kicked. Reason: " + reason);
            await user.KickAsync(reason);
            await Context.Channel.SendMessageAsync(user.Mention + " Has been kicked. Reason: " + reason);
        }

        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUser(IGuildUser user, [Remainder]string reason = "No reason provided.")
        {
            var dmChannel = await user.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync("You have been banned. Reason: " + reason);
            await user.Guild.AddBanAsync(user, 5, reason);
            await Context.Channel.SendMessageAsync(user.Mention + " Has been banned. Reason: " + reason);
        }

        [Command("hunt")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Forcestarthunt()
        {
            await MonsterHunt.ForceStart(Context.User);
        }

        /*[Command("whatlevelis")]
        public async Task WhatLevelIs(uint xp)
        {
            uint level = (uint)Math.Sqrt(xp / 50);
            await Context.Channel.SendMessageAsync("The level is " + level);
        }*/
        [Command("stats")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task MyStats(IGuildUser user)
        {
            var account = UserAccounts.GetAccount((SocketUser)user);
            if (null == account) return;

            uint CurrentLevel = account.LevelNumber;
            uint CurrentXP = account.XP;
            uint NextLevelXP = (uint)Math.Pow(CurrentLevel + 1, 2) * 50;
            uint ExpTNL = (uint)Math.Abs(NextLevelXP - CurrentXP);

            var embed = new EmbedBuilder();
            embed.WithTitle("Level : " + CurrentLevel);
            embed.WithDescription($"**Has {CurrentXP} XP\n\n>> Next level at {NextLevelXP}**\n\n");
            embed.WithColor(new Color(0, 255, 0));
            embed.WithTimestamp(DateTimeOffset.FromUnixTimeMilliseconds(1534271035765));
            await Context.Channel.SendMessageAsync("", false, embed);
        }

         [Command("mystats")]
        public async Task MyStats()
        {
            UserAccount CurrentAccount = UserAccounts.GetAccount(Context.User);
            if (null == CurrentAccount) return;

            uint CurrentLevel = CurrentAccount.LevelNumber;
            uint CurrentXP = CurrentAccount.XP;
            uint NextLevelXP = (uint)Math.Pow(CurrentLevel + 1, 2) * 50;
            uint ExpTNL = (uint)Math.Abs(NextLevelXP - CurrentXP);

            var embed = new EmbedBuilder();
            embed.WithTitle("Level : " + CurrentLevel);
            embed.WithDescription($"**You have {CurrentXP} XP\n\n>> Next level at {NextLevelXP}**\n\n");
            embed.WithColor(new Color(0, 255, 0));
            embed.WithTimestamp(DateTimeOffset.FromUnixTimeMilliseconds(1534271035765));
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("addxp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddXP(string target_name, uint xp)
        {
            target_name = target_name.Insert(2, "!");
            SocketGuildUser TargetUser = Context.Guild.Users.FirstOrDefault(x => x.Mention.Contains(target_name));
            if (null == TargetUser)
            {
                Console.WriteLine("Failed to find user");
                return;
            }

            //account.XP += xp;

            BeeBot.Core.LevelingSystem.Leveling.AddExperience(TargetUser, (SocketTextChannel)Context.Channel, xp, true);

            await Context.Channel.SendMessageAsync($"Has gained {xp} XP.");

        }

        [Command("announce")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Echo([Remainder]string message)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Announcement :loud_sound:");
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 255, 0));
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("pick")]
        public async Task PickOne([Remainder]string message)
        {
            string[] options = message.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            Random r = new Random();
            string selection = options[r.Next(0, options.Length)];

            var embed = new EmbedBuilder();
            embed.WithTitle("Choice for " + Context.User.Username);
            embed.WithDescription(selection);
            embed.WithColor(new Color(0, 255, 0));
            embed.WithThumbnailUrl("");

            await Context.Channel.SendMessageAsync("", false, embed);
            DataStorage.AddPairToStorage(Context.User.Username + DateTime.Now.ToLongTimeString(), selection);
        }

        [Command("help")]
        public async Task RevealSecret([Remainder]string arg = "")
        {
            var embed = new EmbedBuilder()
            .WithTitle("Hello! Here is a list of _**commands**_ I am able to do. :robot: ")
            .WithColor(new Color(182, 0, 22))
           // .WithTimestamp(DateTimeOffset.FromUnixTimeMilliseconds(1534040007130))
            
            .WithThumbnailUrl("https://cdn.discordapp.com/attachments/476490288345514020/478034905863946250/399930843037958144.png")
            .WithAuthor(author =>
            {
                author
               .WithName("Bee.exe")
               .WithUrl("https://cdn.discordapp.com/attachments/476490288345514020/478034905863946250/399930843037958144.png");
            })
            .WithDescription(Utilities.GetAlert("BOI"));
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync("", embed: embed);
        }

        [Command("data")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task GetData()
        {
            await Context.Channel.SendMessageAsync("Data Has " + DataStorage.GetPairsCount() + " pairs.");
        }

        [Command("delete", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task purgeFuction(int amount = 9)
        {
            await Context.Message.DeleteAsync();
            var messages = await Context.Channel.GetMessagesAsync(amount + 1).Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);
            var count = messages.Count();
            Console.WriteLine(count + " Messages Deleted");
        }


        [Command("roles")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task HandleReactionMessage([Remainder]string arg = "")
        {

            RestUserMessage msg = await Context.Channel.SendMessageAsync(Utilities.GetAlert("roles"));
            Global.LocationMessageIdToTrack = msg.Id;
            msg = await Context.Channel.SendMessageAsync(Utilities.GetAlert("genders"));
            Global.SexMessageIdToTrack = msg.Id;
            msg = await Context.Channel.SendMessageAsync(Utilities.GetAlert("NSFW"));
            Global.NSFWIdToTrack = msg.Id;
         
        }


        [Command("shoot")]
        public async Task Shoot()
        {
            Console.WriteLine("Shoot guns");
            await MonsterHunt.Shoot(Context.User);
        }

        [Command("reload")]
        public async Task Reload()
        {
            Console.WriteLine("Reload guns");
            await MonsterHunt.Reload(Context.User);
        }
    }
}


