using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReadyPlayerMe
{
    public abstract class StateMachine : MonoBehaviour
    {
        private readonly Stack<StateType> previousStates = new Stack<StateType>();
        private readonly Dictionary<StateType, State> stateTypeMap = new Dictionary<StateType, State>();

        [SerializeField] protected List<StateType> statesToSkip;

        protected Action<StateType, StateType> StateChanged;
        private StateType currentState;
        
        protected void Initialize(List<State> states)
        {
            foreach (var state in states)
            {
                stateTypeMap.Add(state.StateType, state);
            }
        }

        public void SetState(StateType stateType)
        {
            var previousState = currentState; 
            if (previousState != StateType.None)
            {
                stateTypeMap[previousState].gameObject.SetActive(false);

                if (statesToSkip.Contains(stateType))
                {
                    SetState(stateTypeMap[stateType].NextState);
                    return;
                }
                
                previousStates.Push(previousState);
            }

            currentState = stateType;
            if (stateType != StateType.End)
            {
                stateTypeMap[currentState].gameObject.SetActive(true);
            }
            StateChanged?.Invoke(currentState, previousState);
        }

        public void ClearPreviousStates()
        {
            previousStates.Clear();
        }

        protected void Back()
        {
            var previousState = currentState; 
            stateTypeMap[previousState].gameObject.SetActive(false);
            currentState = previousStates.Pop();
            if (currentState != StateType.None)
            {
                stateTypeMap[currentState].gameObject.SetActive(true);
                StateChanged?.Invoke(currentState, previousState);
            }
        }
    }
}
