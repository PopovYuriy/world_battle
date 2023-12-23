using Core.Signals;

namespace App.Signals
{
    public sealed class StartExistGameSignal : ISignal
    {
        public string GameUid { get; }

        public StartExistGameSignal(string gameUid)
        {
            GameUid = gameUid;
        }
    }
}