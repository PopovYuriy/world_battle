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
        public const string WinnerPlayerIdKey = "WinnerPlayerId";
        public const string PlayersKey = "Players";
        public const string GridKey = "Grid";
        public const string TurnsKey = "Turns";
        
        [JsonProperty(PropertyName = UidKey)] public string Uid { get; private set; }
        [JsonProperty(PropertyName = LastTurnPlayerIdKey)] public string LastTurnPlayerId { get; set; }
        [JsonProperty(PropertyName = WinnerPlayerIdKey)] public string WinnerPlayerId { get; set; }
        [JsonProperty(PropertyName = PlayersKey)] public PlayerGameData[] Players { get; private set; }
        [JsonProperty(PropertyName = GridKey)] public GridModel Grid { get; set; }
        [JsonProperty(PropertyName = TurnsKey)] public List<string> Turns { get; set; }

        public IReadOnlyList<string> TurnsList => Turns;

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
}