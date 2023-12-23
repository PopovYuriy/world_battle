using System;
using Newtonsoft.Json;

namespace App.Data
{
    [Serializable]
    public sealed class PendingGameData
    {
        public const string SecretWordKey = "SecretWord";
        public const string OwnerUidKey = "OwnerUid";
        public const string SecondUidKey = "SecondUid";

        [JsonProperty(PropertyName = SecretWordKey, Required = Required.Always)] public string SecretWord { get; }
        [JsonProperty(PropertyName = OwnerUidKey, Required = Required.Always)] public string OwnerUid { get; }
        [JsonProperty(PropertyName = SecondUidKey, Required = Required.AllowNull)] public string SecondUid { get; }

        public PendingGameData(string secretWord, string ownerUid)
        {
            SecretWord = secretWord;
            OwnerUid = ownerUid;
        }
    }
}