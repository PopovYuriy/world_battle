using App.Modules.GameSessions.API.Enums;
using App.Modules.GameSessions.Data;
using Core.Signals;

namespace App.Modules.GameSessions
{
    public static class GameSessionsSignal
    {
        public sealed class GameCreatedSignal : SignalBase<GameSessionType, GameSessionData>
        {
            public GameCreatedSignal(GameSessionType arg1, GameSessionData arg2) : base(arg1, arg2) {}
        }
        
        public sealed class GameDeletedSignal : SignalBase<GameSessionType, GameSessionData>
        {
            public GameDeletedSignal(GameSessionType arg1, GameSessionData arg2) : base(arg1, arg2) {}
        }
    }
}