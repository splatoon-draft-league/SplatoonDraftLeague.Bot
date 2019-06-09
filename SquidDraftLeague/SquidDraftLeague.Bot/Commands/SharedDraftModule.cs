﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using NLog;
using SquidDraftLeague.Bot.AirTable;
using SquidDraftLeague.Bot.Commands.Attributes;
using SquidDraftLeague.Bot.Extensions;
using SquidDraftLeague.Bot.Penalties;
using SquidDraftLeague.Bot.Queuing;
using SquidDraftLeague.Language.Resources;

namespace SquidDraftLeague.Bot.Commands
{
    [Name("Overlapping Draft")]
    public class SharedDraftModule : InteractiveBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Command("betareg"),
         RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task BetaReg()
        {
            IRole playerRole = this.Context.Guild.Roles.First(e => e.Name == "Player");

            foreach (SocketGuildUser socketGuildUser in this.Context.Guild.Users.Where(e => e.Roles.Any(f => f.Name == "Beta")))
            {
                await AirTableClient.RegisterPlayer(socketGuildUser, 2000);

                if (socketGuildUser.Roles.All(e => e.Name != "Player"))
                {
                    await socketGuildUser.AddRoleAsync(playerRole);
                }
            }

            await this.ReplyAsync("OK, this is epic.");
        }

        [Command("cleanup"),
         RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Cleanup()
        {
            foreach (SocketGuildUser socketGuildUser in this.Context.Guild.Users)
            {
                if (socketGuildUser.IsBot)
                    continue;

                foreach (ulong cleanupRoleId in CommandHelper.DraftRoleIds)
                {
                    if (socketGuildUser.Roles.All(e => e.Id != cleanupRoleId))
                        continue;

                    IRole role = this.Context.Guild.GetRole(cleanupRoleId);
                    await socketGuildUser.RemoveRoleAsync(role);
                }
            }

            await this.ReplyAndDeleteAsync("\uD83D\uDC4C");
        }

        [Command("status"),
         Summary("Gets the status of a lobby or set.")]
        public async Task Status([Summary(
                "Optional parameter to specify a lobby number. Not required for sets or if you are checking a lobby you are in.")]
            int lobbyNum = 0)
        {
            if (!(this.Context.User is IGuildUser user))
                return;

            Set setInChannel = CommandHelper.SetFromChannel(this.Context.Channel.Id);

            if (setInChannel != null && setInChannel.AllPlayers.Any())
            {
                await this.ReplyAsync(embed: setInChannel.GetEmbedBuilder().Build());
            }
            else if (LobbyModule.Lobbies.All(e => e.LobbyNumber != lobbyNum))
            {
                Lobby joinedLobby = LobbyModule.Lobbies.FirstOrDefault(e => e.Players.Any(f => f.DiscordId == user.Id));

                if (joinedLobby == null)
                {
                    await this.ReplyAsync(Resources.NotInLobby);
                    return;
                }

                EmbedBuilder builder = joinedLobby.GetEmbedBuilder();

                await this.ReplyAsync(embed: builder.Build());
            }
            else
            {
                Lobby selectedLobby = LobbyModule.Lobbies.First(e => e.LobbyNumber == lobbyNum);
                await this.ReplyAsync(embed: selectedLobby.GetEmbedBuilder().Build());
            }
        }

        [Command("close"),
         RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Close(string type, int number)
        {
            type = type.ToLower();

            if (type == "set")
            {
                SetModule.Sets[number - 1].Close();
                await this.ReplyAsync($"An admin has closed set number {number}.");
            }
            else if (type == "lobby")
            {
                LobbyModule.Lobbies[number - 1].Close();
                await this.ReplyAsync($"An admin has closed lobby number {number}.");
            }
            else
            {
                await this.ReplyAsync("Please specify \"set\" or \"lobby\".");
            }
        }

        [Command("report"),
         Summary("Reports a player for any reason. Only to be used in DMs with this bot."),
         ExampleCommand("%report \"DeltaJordan#5497\" Called me a crayon eater.")]
        public async Task Report(
            [Summary("Name formatted like so `\"DeltaJordan#5497\"`. The quotes are required if the name has spaces.")]
            string username, 
            [Summary("The reason this person is being reported. Explain as much as possible."),
             Remainder] string reason)
        {
            if (!(this.Context.Channel is IDMChannel))
            {
                return;
            }

            string[] splitName = username.Replace("@", "").Split('#');

            if (splitName.Length < 2)
            {
                await this.ReplyAsync(Resources.InvalidReportNameSplit);
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                await this.ReplyAsync(Resources.InvalidReportNoReason);
            }

            SocketUser reportedUser = null;

            try
            {
                reportedUser = Program.Client.GetUser(splitName[0], splitName[1]);
            }
            catch (Exception e)
            {
                Logger.Trace(e);
            }

            if (reportedUser == null)
            {
                await this.ReplyAsync(Resources.InvalidReportNameResolve);
            }
            else
            {
                SocketTextChannel modChannel = (SocketTextChannel)Program.Client.GetChannel(572608285904207875);

                EmbedBuilder builder = new EmbedBuilder
                {
                    Description = $"**{reportedUser.Mention} reported by {this.Context.User.Mention}.**",
                    Timestamp = DateTimeOffset.Now
                };

                builder.AddField(e =>
                {
                    e.Name = "Reason:";
                    e.Value = $"{reason}";
                });

                await modChannel.SendMessageAsync($"<@572539082039885839>", embed: builder.Build());
                await this.ReplyAsync(Resources.ReportResponse);
            }
        }

        [Command("penalty"),
        RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Penalty(IUser user, int amount, [Remainder] string notes)
        {
            try
            {
                if (amount <= 0)
                {
                    await this.ReplyAsync("Amount should be greater than zero.");
                    return;
                }

                await AirTableClient.PenalizePlayer(user.Id, amount, notes);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }

            await this.ReplyAsync($"Penalized {user.Mention} {amount} points.");
        }

        [Command("leave", RunMode = RunMode.Async),
         Summary("Leaves a currently joined lobby.")]
        public async Task Leave()
        {
            if (!(this.Context.User is IGuildUser user))
                return;

            Lobby joinedLobby = LobbyModule.Lobbies.FirstOrDefault(e => e.Players.Any(f => f.DiscordId.GetGuildUser(this.Context).Id == user.Id));

            if (joinedLobby != null)
            {
                SocketRole setRole = this.Context.Guild.Roles.FirstOrDefault(e => e.Name == $"In Set ({joinedLobby.LobbyNumber})");
                SocketRole devRole = this.Context.Guild.Roles.First(e => e.Name == "Developer");

                if (setRole == null)
                {
                    await devRole.ModifyAsync(e => e.Mentionable = true);
                    await this.Context.Channel.SendMessageAsync($"{devRole.Mention} Fatal Error! Unable to find In Set role with name \"In Set ({joinedLobby.LobbyNumber})\".");
                    await devRole.ModifyAsync(e => e.Mentionable = false);
                    return;
                }

                await user.RemoveRoleAsync(setRole);
                joinedLobby.RemovePlayer(joinedLobby.Players.FirstOrDefault(e => e.DiscordId.GetGuildUser(this.Context).Id == user.Id));

                await this.ReplyAsync($"You have left lobby #{joinedLobby.LobbyNumber}.");

                if (joinedLobby.Players.Count == 0)
                {
                    joinedLobby.Close();

                    await this.ReplyAsync($"Lobby #{joinedLobby.LobbyNumber} has been disbanded.");
                }
            }
            else
            {
                Set joinedSet = SetModule.Sets.FirstOrDefault(e => e.AllPlayers.Any(f => f.DiscordId.GetGuildUser(this.Context).Id == user.Id));

                if (joinedSet == null)
                {
                    return;
                }

                string penaltyDir = Directory.CreateDirectory(Path.Combine(Globals.AppPath, "Penalties")).FullName;
                string penaltyFile = Path.Combine(penaltyDir, $"{user.Id}.penalty");

                string penaltyMessage = "If you leave the set, you will be instated with a penalty of 15 points. ";
                Record record;

                if (File.Exists(penaltyFile))
                {
                    record = JsonConvert.DeserializeObject<Record>(File.ReadAllText(penaltyFile));

                    int infractionCount = record.InfractionsThisMonth() + 1;

                    switch (infractionCount)
                    {
                        case 1:
                            penaltyMessage += "";
                            break;
                        case 2:
                            penaltyMessage += "In addition, you will be banned from participating in SDL for 24 hours.";
                            break;
                        case 3:
                            penaltyMessage += "In addition, you will be banned from participating in SDL for 1 week.";
                            break;
                        default:
                            penaltyMessage +=
                                "In addition, you will be banned from participating in SDL for 1 week AND will be barred from participating in cups.";
                            break;
                    }
                }
                else
                {
                    penaltyMessage += " Otherwise, this time will be just a warning.";

                    record = new Record
                    {
                        AllInfractions = new List<Infraction>()
                    };
                }

                await this.ReplyAsync(penaltyMessage + " Are you sure you wish to leave the set? (Y/N)");

                SocketMessage response = await this.NextMessageAsync(timeout: TimeSpan.FromMinutes(1));

                if (response == null)
                {
                    await this.ReplyAsync($"{user.Mention} took too long to respond. Assuming you changed your mind, please continue with the set.");
                }
                else if (response.Content.ToLower() == "y")
                {
                    await AirTableClient.PenalizePlayer(user.Id, 15, "Left a set.");

                    record.AllInfractions.Add(new Infraction
                    {
                        Penalty = 15,
                        Notes = "Left a set.",
                        TimeOfOffense = DateTime.Now
                    });

                    if (joinedSet.AlphaTeam.Players.Any(e => e.DiscordId.GetGuildUser(this.Context).Id == user.Id))
                    {
                        joinedSet.AlphaTeam.RemovePlayer(
                            joinedSet.AlphaTeam.Players.First(e => e.DiscordId.GetGuildUser(this.Context).Id == user.Id));
                    }
                    else if (joinedSet.BravoTeam.Players.Any(e => e.DiscordId.GetGuildUser(this.Context).Id == user.Id))
                    {
                        joinedSet.BravoTeam.RemovePlayer(
                            joinedSet.BravoTeam.Players.First(e => e.DiscordId.GetGuildUser(this.Context).Id == user.Id));
                    }
                    else if (joinedSet.DraftPlayers.Any(e => e.DiscordId.GetGuildUser(this.Context).Id == user.Id))
                    {
                        joinedSet.DraftPlayers.Remove(
                            joinedSet.DraftPlayers.First(e => e.DiscordId.GetGuildUser(this.Context).Id == user.Id));
                    }

                    File.WriteAllText(penaltyFile, JsonConvert.SerializeObject(record, Formatting.Indented));

                    await this.ReplyAsync(
                        $"{user.Mention} Aforementioned penalty applied. Don't make a habit of this! " +
                        $"As for the rest of the set, you will return to <#572536965833162753> to requeue. Removing access to this channel in 30 seconds.");

                    Lobby movedLobby = LobbyModule.Lobbies.First(e => !e.Players.Any());

                    if (movedLobby == null)
                    {
                        // TODO Not sure what to do if all lobbies are filled.
                        return;
                    }

                    foreach (SdlPlayer joinedSetPlayer in joinedSet.AllPlayers)
                    {
                        movedLobby.AddPlayer(joinedSetPlayer, true);
                    }

                    joinedSet.Close();

                    SocketRole setRole = this.Context.Guild.Roles.FirstOrDefault(e => e.Name == $"In Set ({joinedSet.SetNumber})");
                    SocketRole devRole = this.Context.Guild.Roles.First(e => e.Name == "Developer");

                    if (setRole == null)
                    {
                        await devRole.ModifyAsync(e => e.Mentionable = true);
                        await this.ReplyAsync(
                            $"{devRole.Mention} Fatal Error! Unable to find In Set role with name \"In Set ({joinedSet.SetNumber})\".");
                        await devRole.ModifyAsync(e => e.Mentionable = false);
                        return;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(30));

                    foreach (SdlPlayer movedLobbyPlayer in movedLobby.Players)
                    {
                        await movedLobbyPlayer.DiscordId.GetGuildUser(this.Context).RemoveRoleAsync(setRole);
                    }

                    await ((ITextChannel) this.Context.Client.GetChannel(572536965833162753))
                        .SendMessageAsync($"{8 - movedLobby.Players.Count} players needed to begin.", 
                            embed: movedLobby.GetEmbedBuilder().Build());
                }
                else
                {
                    await this.ReplyAsync("K. Please continue with the set.");
                }
            }
        }
    }
}