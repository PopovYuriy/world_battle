using App.Modules.Matchmaking.Data;
using App.Modules.Matchmaking.Enums;

namespace App.Modules.Matchmaking.API.Data
{
    public sealed class JoinGameResult
    {
        public PendingGameData Data { get; }
        public PendingGameJoinError Error { get; } = PendingGameJoinError.None;

        public JoinGameResult(PendingGameData data)
        {
            Data = data;
        }

        public JoinGameResult(PendingGameJoinError error)
        {
            Error = error;
        }
    }
}