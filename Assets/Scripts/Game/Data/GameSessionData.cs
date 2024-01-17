using System;
using System.Collections.Generic;
using Game.Grid;
using Newtonsoft.Json;

namespace Game.Data
{
    [Serializable]
    public sealed class GameSessionData : IGameTurnsProvider
    {
        public const string UidKey = "Uid";
        public const string LastTurnPlayerIdKey = "LastTurnPlayerId";
        public const string WinDataKey = "WinData";
        public const string GiveUpDataKey = "GaveUpData";
        public const string PlayersKey = "Players";
        public const string GridKey = "Grid";
        public const string TurnsKey = "Turns";
        
        [JsonProperty(PropertyName = UidKey)] public string Uid { get; private set; }
        [JsonProperty(PropertyName = LastTurnPlayerIdKey)] public string LastTurnPlayerId { get; set; }
        [JsonProperty(PropertyName = WinDataKey)] public WinData WinData { get; set; }
        [JsonProperty(PropertyName = GiveUpDataKey)] public SurrenderData SurrenderData { get; set; }
        [JsonProperty(PropertyName = PlayersKey)] public PlayerGameData[] Players { get; private set; }
        [JsonProperty(PropertyName = GridKey)] public GridModel Grid { get; set; }
        [JsonProperty(PropertyName = TurnsKey)] public List<string> Turns { get; set; }

        [JsonIgnore] public IReadOnlyList<string> TurnsList => Turns;

        public GameSessionData(string uid, PlayerGameData[] players, GridModel grid, List<string> turns)
        {
            Uid = uid;
            Players = players;
            Grid = grid;
            Turns = turns;
        }
    }

    [Serializable]
    public sealed class PlayerGameData
    {
        [JsonProperty(PropertyName = "Uid")] public string Uid { get; private set; }
        [JsonProperty(PropertyName = "Name")] public string Name { get; private set; }

        public PlayerGameData(string uid, string name)
        {
            Uid = uid;
            Name = name;
        }
    }

    [Serializable]
    public sealed class WinData
    {
        [JsonProperty(PropertyName = "PlayerId")] public string PlayerId { get; private set; }
        [JsonProperty(PropertyName = "Reason")] public WinReason Reason { get; private set; }
        [JsonProperty(PropertyName = "ProcessCount")] public int ProcessCount { get; set; }

        public WinData(string playerId, WinReason reason)
        {
            PlayerId = playerId;
            Reason = reason;
        }
    }

    [Serializable]
    public sealed class SurrenderData
    {
        [JsonProperty(PropertyName = "InitiatorUid")] public string InitiatorUid { get; private set; }

        public SurrenderData(string initiatorUid)
        {
            InitiatorUid = initiatorUid;
        }
    }

    public enum WinReason : byte
    {
        None = 0, 
        BaseCaptured = 1,
        Surrender = 2
    }
}