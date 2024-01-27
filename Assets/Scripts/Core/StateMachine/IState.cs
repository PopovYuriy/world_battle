using System;

namespace Core.StateMachine
{
    public interface IState
    {
        event Action OnFinished;
        void Start();
        void Finish();
        void Reset();
    }
}