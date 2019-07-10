﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using SquidDraftLeague.Draft.Extensions;
using SquidDraftLeague.Draft.Map;
using SquidDraftLeague.Settings;

namespace SquidDraftLeague.Draft
{
    public class Set
    {
        public enum WinningTeam
        {
            Alpha,
            Bravo,
            Tie
        }

        public int SetNumber { get; }

        public SdlPlayer Host { get; set; }

        public SdlPlayer Halved { get; private set; }

        public DateTime? StartTime { get; set; }

        public IEnumerable<SdlPlayer> AllPlayers => this.AlphaTeam.Players.Concat(this.BravoTeam.Players).Concat(this.DraftPlayers);

        public WinningTeam Winning =>
            this.AlphaTeam.Score == this.BravoTeam.Score ? WinningTeam.Tie :
            this.AlphaTeam.Score > this.BravoTeam.Score ? WinningTeam.Alpha : WinningTeam.Bravo;

        public SdlTeam AlphaTeam { get; }
        public SdlTeam BravoTeam { get; }
        
        public readonly List<SdlPlayer> DraftPlayers = new List<SdlPlayer>();

        public int MatchNum;
        public List<Stage> PlayedStages = new List<Stage>();

        public int ResolveMode = 0;
        public bool Locked { get; set; }

        public event EventHandler DraftTimeout;
        public event EventHandler Closed;

        private Timer draftTimer;

        private GameMode[] modeOrder;

        public Set(int setNumber)
        {
            this.SetNumber = setNumber;

            this.AlphaTeam = new SdlTeam("Alpha");
            this.BravoTeam = new SdlTeam("Bravo");

            this.draftTimer = new Timer(60000) {AutoReset = false};
            this.draftTimer.Elapsed += this.DraftTimer_Elapsed;
        }

        /// <summary>
        /// Using a snake draft order, calculates the drafting turn based on players left.
        /// </summary>
        /// <returns>Team whose turn it is to pick.</returns>
        public SdlTeam GetPickingTeam()
        {
            switch (this.DraftPlayers.Count)
            {
                case 6: // 1
                case 3: // 4
                case 2: // 5
                    return this.BravoTeam;
                case 5: // 2
                case 4: // 3
                case 1: // 6
                    return this.AlphaTeam;
                default:
                    return this.AlphaTeam;
            }
        }

        private void DraftTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!this.DraftPlayers.Any())
            {
                this.draftTimer.Stop();
                return;
            }

            this.DraftTimeout?.Invoke(this, EventArgs.Empty);
        }

        public void ReportScore(string winner)
        {
            winner = winner.ToLower();

            if (winner == "alpha")
            {
                this.AlphaTeam.OrderedMatchResults.Add(1);
                this.BravoTeam.OrderedMatchResults.Add(0);
            }
            else
            {
                this.AlphaTeam.OrderedMatchResults.Add(0);
                this.BravoTeam.OrderedMatchResults.Add(1);
            }
        }

        public void ResetTimeout()
        {
            this.draftTimer.Stop();
            this.draftTimer.Start();
        }

        public void MoveLobbyToSet(Lobby lobby)
        {
            List<SdlPlayer> orderedPlayers = lobby.Players.OrderByDescending(e => e.PowerLevel).ToList();

            this.AlphaTeam.AddPlayer(orderedPlayers[0], true);
            this.BravoTeam.AddPlayer(orderedPlayers[1], true);

            this.DraftPlayers.AddRange(orderedPlayers.Skip(2));

            this.Halved = lobby.Halved;
            this.StartTime = DateTime.UtcNow;
        }

        public void Close()
        {
            this.StartTime = null;
            this.DraftTimeout = null;
            this.Halved = null;
            this.AlphaTeam.Clear();
            this.BravoTeam.Clear();
            this.DraftPlayers.Clear();
            this.PlayedStages.Clear();
            this.MatchNum = 0;

            this.modeOrder = null;

            this.Closed?.Invoke(this, EventArgs.Empty);
        }

        public Stage PickStage(Stage[] stages)
        {
            if (this.modeOrder == null)
            {
                this.modeOrder = Enum.GetValues(typeof(GameMode)).Cast<GameMode>().Shuffle().ToArray();
            }

            stages = stages
                .Where(e => this.PlayedStages.All(f => f.MapName != e.MapName) && e.Mode == this.modeOrder[(this.MatchNum - 1) % 4])
                .ToArray();

            return stages[Globals.Random.Next(0, stages.Length - 1)];
        }
    }
}
