using System;
using System.Collections.Generic;
using Game.Abilities;
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
        public const string ModificationsDataKey = "ModificationsData";
        public const string AbilityDataKey = "AbilityData";
        public const string PlayersKey = "Players";
        public const string GridKey = "Grid";
        public const string TurnsKey = "Turns";
        
        [JsonProperty(PropertyName = UidKey)] public string Uid { get; private set; }
        [JsonProperty(PropertyName = LastTurnPlayerIdKey)] public string LastTurnPlayerId { get; set; }
        [JsonProperty(PropertyName = WinDataKey)] public WinData WinData { get; set; }
        [JsonProperty(PropertyName = GiveUpDataKey)] public SurrenderData SurrenderData { get; set; }
        [JsonProperty(PropertyName = ModificationsDataKey)] public ModificationsData ModificationsData { get; set; }
        [JsonProperty(PropertyName = AbilityDataKey)] public AbilityData AbilityData { get; set; }
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
        [JsonProperty(PropertyName = "Points")] public int Points { get; set; }
        [JsonProperty(PropertyName = "AbilitiesCosts")] public Dictionary<AbilityType, int> AbilitiesCosts { get; set; }
        [JsonIgnore] public bool IsControllable { get; set; }

        [JsonConstructor]
        public PlayerGameData(string uid, string name, int points, Dictionary<AbilityType, int> abilitiesCosts)
        {
            Uid = uid;
            Name = name;
            Points = points;
            AbilitiesCosts = abilitiesCosts;
        }
        
        public PlayerGameData(string uid, string name, Dictionary<AbilityType, int> abilitiesCosts, bool isControllable)
        {
            Uid = uid;
            Name = name;
            Points = 0;
            AbilitiesCosts = abilitiesCosts;
            IsControllable = isControllable;
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

    [Serializable]
    public sealed class AbilityData
    {
        [JsonProperty(PropertyName = "Type")] public AbilityType Type { get; private set; }
        [JsonProperty(PropertyName = "Initiator")] public string InitiatorUid { get; private set; }
        [JsonProperty(PropertyName = "CellId")] public int CellId { get; private set; }
        [JsonProperty(PropertyName = "FinalTurnId")] public int FinalTurnId { get; private set; }

        public AbilityData(AbilityType type, string initiatorUid, int cellId, int finalTurnId)
        {
            Type = type;
            InitiatorUid = initiatorUid;
            FinalTurnId = finalTurnId;
        }
    }

    [Serializable]
    public sealed class ModificationsData
    {
        [JsonProperty(PropertyName = "LockedCells")] public List<LockedCellData> LockedCells { get; private set; }

        [JsonConstructor]
        public ModificationsData(List<LockedCellData> lockedCells)
        {
            LockedCells = lockedCells;
        }

        public ModificationsData()
        {
            LockedCells = new List<LockedCellData>();
        }
    }

    [Serializable]
    public sealed class LockedCellData
    {
        [JsonProperty(PropertyName = "CellId")] public int CellId { get; private set; }
        [JsonProperty(PropertyName = "FinalTurnNumber")] public int FinalTurnNumber { get; private set; }

        public LockedCellData(int cellId, int finalTurnNumber)
        {
            CellId = cellId;
            FinalTurnNumber = finalTurnNumber;
        }
    }

    public enum WinReason : byte
    {
        None = 0, 
        BaseCaptured = 1,
        Surrender = 2
    }
}