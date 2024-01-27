using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.StateMachine
{
    public class StackedStateMachine<TStateType> where TStateType : Enum
    {
        private Dictionary<TStateType, IState> _registeredStates;
        private Stack<IState> _statesStack;

        public StackedStateMachine()
        {
            _statesStack = new Stack<IState>();
            _registeredStates = new Dictionary<TStateType, IState>();
        }

        public void Dispose()
        {
            if (_statesStack.TryPeek(out var state))
                FinishState(state);
        }

        public void RegisterState(TStateType type, IState state)
        {
            if (_registeredStates.ContainsKey(type))
            {
                Debug.LogError($"State {type} already exist in registered states");
                return;
            }
            
            _registeredStates.Add(type, state);
        }

        public void PushState(TStateType type)
        {
            if (!_registeredStates.ContainsKey(type))
            {
                Debug.LogError($"State {type} is not found in registered states");
                return;
            }
            
            if (_statesStack.Count > 0)
                FinishState(_statesStack.Peek());

            var state = _registeredStates[type];
            _statesStack.Push(state);
            StartState(_statesStack.Peek());
        }

        private void StateFinishedHandler()
        {
            FinishState(_statesStack.Pop());

            if (_statesStack.TryPeek(out var state))
                StartState(state);
        }

        private void FinishState(IState state)
        {
            state.OnFinished -= StateFinishedHandler;
            state.Finish();
        }

        private void StartState(IState state)
        {
            state.OnFinished += StateFinishedHandler;
            state.Reset();
            state.Start();
        }
    }
}