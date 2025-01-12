using App.Modules.Matchmaking.Data;
using App.Modules.Matchmaking.Enums;

namespace App.Modules.Matchmaking.API.Data
{
    public sealed class CreateGameResult
    {
        public PendingGameData Data  { get; }
        public PendingGameCreateError   Error { get; } = PendingGameCreateError.None;

        public CreateGameResult(PendingGameData data)
        {
            Data = data;
        }

        public CreateGameResult(PendingGameCreateError error)
        {
            Error = error;
        }
    }
}