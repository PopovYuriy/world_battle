using App.Modules.Matchmaking.Enums;

namespace App.Modules.Matchmaking.Data
{
    public sealed class PendingGameCreatedData
    {
        public PendingGameCreateError Error { get; }
        public PendingGameData        Data  { get; }

        public PendingGameCreatedData(PendingGameData data)
        {
            Data = data;
        }

        public PendingGameCreatedData(PendingGameCreateError error)
        {
            Error = error;
        }
    }
}