using System;
using Core.Signals;

namespace App.Signals
{
    public enum CreateOnlineGameSignalType : byte
    {
        Create = 0,
        Find = 1
    }
    
    public sealed class CreateOnlineGameSignal : ISignal
    {
        public string SecretWord { get; }
        public CreateOnlineGameSignalType Type { get; }

        public event Action OnGameStarted;

        public CreateOnlineGameSignal(string secretWord, CreateOnlineGameSignalType type)
        {
            SecretWord = secretWord;
            Type = type;
        }

        public void DispatchGameStarted() => OnGameStarted?.Invoke();
    }
}