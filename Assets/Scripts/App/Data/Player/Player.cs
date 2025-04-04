using System;
using Newtonsoft.Json;

namespace App.Data.Player
{
    [Serializable]
    public sealed class Player : IPlayerMutable
    {
        public const string UidKey = "Uid";
        public const string NameKey = "Name";
        
        [JsonProperty(PropertyName = UidKey, Required = Required.Always)] public string Uid { get; private set; }
        [JsonProperty(PropertyName = NameKey, Required = Required.AllowNull)] public string Name { get; private set; }

        public Player() { }

        public Player(string uid, string name)
        {
            Uid = uid;
            Name = name;
        }

        public void SetUidAndName(string uid, string name)
        {
            Uid = uid;
            Name = name;
        }

        public void SetName(string name) => Name = name;

        public static IPlayer CreateGuestUser() => new Player("guest", "Guest");
    }
}