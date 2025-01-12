using App.Modules.Matchmaking.Enums;

namespace App.Modules.Matchmaking.Data
{
    public sealed class PendingGameJoinedData
    {
        public PendingGameJoinError Error { get; }
        public PendingGameData      Data  { get; }

        public PendingGameJoinedData(PendingGameData data)
        {
            Data = data;
        }

        public PendingGameJoinedData(PendingGameJoinError error)
        {
            Error = error;
        }
    }
}