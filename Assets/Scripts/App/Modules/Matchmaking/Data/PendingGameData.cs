using System;
using Firebase.Database;
using Newtonsoft.Json;

namespace App.Modules.Matchmaking.Data
{
    [Serializable]
    public sealed class PendingGameData
    {
        public const string IdKey             = "id";
        public const string KeywordKey        = "keyword";
        public const string HostUidKey        = "hostUid";
        public const string OpponentUidKey    = "opponentUid";
        public const string SessionCreatedKey = "sessionCreated";
        public const string CreatedAtKey      = "createdAt";

        [JsonProperty(PropertyName = IdKey, Required = Required.Always)]              public  string Id             { get; }
        [JsonProperty(PropertyName = KeywordKey, Required = Required.Always)]         public  string Keyword        { get; }
        [JsonProperty(PropertyName = HostUidKey, Required = Required.Always)]         public  string HostUid        { get; }
        [JsonProperty(PropertyName = OpponentUidKey, Required = Required.Default)]    public  string OpponentUid    { get; private set; }
        [JsonProperty(PropertyName = SessionCreatedKey, Required = Required.Default)] public  bool   SessionCreated { get; }
        [JsonProperty(PropertyName = CreatedAtKey, Required = Required.Always)]       private object CreatedAt      { get; }

        private long _createdAtTimestamp;

        [JsonIgnore] public long CreatedAtTimestamp => _createdAtTimestamp == default ? Convert.ToInt64(CreatedAt) : _createdAtTimestamp;

        public PendingGameData(string keyword, string hostUid)
        {
            Id = Guid.NewGuid().ToString();
            Keyword = keyword;
            HostUid = hostUid;
            CreatedAt = ServerValue.Timestamp;
        }

        [JsonConstructor]
        public PendingGameData(object createdAt, string id, string keyword, string hostUid, string opponentUid, bool sessionCreated)
        {
            CreatedAt = createdAt;
            Id = id;
            Keyword = keyword;
            HostUid = hostUid;
            OpponentUid = opponentUid;
            SessionCreated = sessionCreated;
        }

        public void SetOpponentUid(string opponentUid)
        {
            OpponentUid = opponentUid;
        }
    }
}