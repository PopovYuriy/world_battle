using App.Modules.Matchmaking.Data;
using Core.Signals;

namespace App.Modules.Matchmaking
{
    public static class MatchmakerSignal
    {
        public sealed class PendingGameCreated : SignalBase<PendingGameCreatedData>
        {
            public PendingGameCreated(PendingGameCreatedData arg) : base(arg) {}
        }

        public sealed class PendingGameJoined : SignalBase<PendingGameJoinedData>
        {
            public PendingGameJoined(PendingGameJoinedData arg) : base(arg) {}
        }

        public sealed class PendingGameDeleted : SignalBase<PendingGameData>
        {
            public PendingGameDeleted(PendingGameData arg) : base(arg) {}
        }

        public sealed class PendingGameExpired : ISignal {}

        public sealed class PendingGameSessionCreated : SignalBase<string>
        {
            public PendingGameSessionCreated(string arg) : base(arg) {}
        }
    }
}