using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace BeeBot.Core.LevelingSystem
{
    public class Leveling
    {
		private static Stopwatch Timer = new Stopwatch();
		private static Dictionary<ulong, long> PlayerCooldownTime = new Dictionary<ulong, long>();
		private static long CooldownTime = ( 30 * 1000 );   //30 Seconds
		private static uint MessageExperience = 10;

		internal static async void AddExperience( SocketGuildUser user, SocketTextChannel channel, uint Experience, bool IgnoreCooldown = false )
		{
			if( false == Timer.IsRunning )
			{
				Timer.Start();
				PlayerCooldownTime.Clear();
			}

			if( false == IgnoreCooldown )
			{
				long TimeSinceLastMessage = 0;
				if( false == PlayerCooldownTime.TryGetValue( user.Id, out TimeSinceLastMessage ) )
				{
					PlayerCooldownTime.Add( user.Id, Timer.ElapsedMilliseconds );
				}
				else
				{
					if( Math.Abs( Timer.ElapsedMilliseconds - TimeSinceLastMessage ) < CooldownTime )
					{
						return;
					}
				}

				PlayerCooldownTime[ user.Id ] = Timer.ElapsedMilliseconds;
			}

			UserAccounts.UserAccount CurrentAccount = UserAccounts.UserAccounts.GetAccount( user );
			if( null == CurrentAccount ) return;

			uint OldLevel = CurrentAccount.LevelNumber;

			CurrentAccount.XP += Experience;

			if( OldLevel != CurrentAccount.LevelNumber )
			{
				SendLevelUpNotice( user, CurrentAccount.LevelNumber, CurrentAccount.XP, channel );
				SetNewLevelRole( user, channel, CurrentAccount.LevelNumber );
			}

			UserAccounts.UserAccounts.SaveAccounts();

			Console.WriteLine( "User {0} Add Exp {1}", user.Nickname, Experience );
		}

		internal static async void SendLevelUpNotice( SocketGuildUser user, uint level, uint experience, SocketTextChannel channel )
		{
			// the user leveled up
			EmbedBuilder EmbedMsg = new EmbedBuilder();
			EmbedMsg.WithColor( 67, 160, 71 );
			EmbedMsg.WithTitle( "LEVEL UP!" );
			EmbedMsg.WithDescription( user.Username + " just leveled up!" );
			EmbedMsg.AddInlineField( "LEVEL", level );
			EmbedMsg.AddInlineField( "XP", experience );

			await channel.SendMessageAsync( "", embed: EmbedMsg );
		}

		internal static async void SetNewLevelRole( SocketGuildUser user, SocketTextChannel channel, uint Level )
		{
			SocketRole RoleToAdd = null;
			string RoleName = "";
			switch( Level )
			{
				case 1: RoleName = "Weebmacht"; break;
				case 10: RoleName = "Weebandiers"; break;
				case 20: RoleName = "SS-Weeben"; break;
				case 30: RoleName = "Gestapo"; break;
				default: return;
			}

			if( null != ( RoleToAdd = user.Guild.Roles.First( x => x.Name.ToString() == RoleName ) ) )
			{
				string LevelUpNotice = String.Format( " You have unlocked {0} role!", RoleToAdd.Name );
				await channel.SendMessageAsync( $"{ user.Mention}" + LevelUpNotice );
				await user.AddRoleAsync( RoleToAdd );
			}
		}

		internal static async void OnMessageReceive(SocketGuildUser user, SocketTextChannel channel)
        {
			AddExperience( user, channel, MessageExperience, false );
        }
    }
    
}
