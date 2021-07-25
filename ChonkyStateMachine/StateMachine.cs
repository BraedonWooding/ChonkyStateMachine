using System;
using System.Collections.Generic;

namespace ChonkyStateMachine
{
    /// <summary>
    /// A simple state machine.
    /// </summary>
    public sealed class StateMachine<State, Trigger>
    {
        /// <summary>
        /// Underlying graph like structure, (adj list - like)
        /// </summary>
        private readonly Dictionary<State, Dictionary<Trigger, State>> _sm = new Dictionary<State, Dictionary<Trigger, State>>();
        private readonly bool _isLax;

        internal StateMachine(bool isLax)
        {
            this._isLax = isLax;
        }

        internal void AddState(State state)
        {
            if (_sm.ContainsKey(state))
            {
                if (!_isLax)
                {
                    throw new StateMachineValidationException<State, Trigger>($"State {state} already exists in state machine.")
                    {
                        State = state,
                        Issue = StateMachineValidationIssues.STATE_ALREADY_EXISTS
                    };
                }
            }
            else
            {
                _sm.Add(state, new Dictionary<Trigger, State>());
            }
        }

        /// <summary>
        /// Get all possible triggers from a given state.
        /// </summary>
        public IEnumerable<Trigger> GetPossibleTriggers(State state)
        {
            if (!_sm.TryGetValue(state, out var startTransitions))
            {
                throw new StateMachineValidationException<State, Trigger>($"State {state} doesn't exist in state machine.")
                {
                    State = state,
                    Issue = StateMachineValidationIssues.STATE_MISSING,
                };
            }

            return startTransitions.Keys;
        }

        /// <summary>
        /// Get all possible resultant states (and the corresponding trigger) from a state.
        /// </summary>
        public IEnumerable<KeyValuePair<Trigger, State>> GetPossibleStates(State state)
        {
            if (!_sm.TryGetValue(state, out var startTransitions))
            {
                throw new StateMachineValidationException<State, Trigger>($"State {state} doesn't exist in state machine.")
                {
                    State = state,
                    Issue = StateMachineValidationIssues.STATE_MISSING,
                };
            }

            return startTransitions;
        }

        internal void AddTransition(Trigger trigger, State start, State end)
        {
            if (_isLax)
            {
                AddState(start);
                AddState(end);
            }

            if (!_sm.TryGetValue(start, out var startTransitions))
            {
                throw new StateMachineValidationException<State, Trigger>($"State {start} doesn't exist in state machine.")
                {
                    State = start,
                    Issue = StateMachineValidationIssues.STATE_MISSING,
                };
            }

            if (startTransitions.ContainsKey(trigger))
            {
                throw new StateMachineValidationException<State, Trigger>($"State {start} with trigger {trigger} already exists in state machine.")
                {
                    State = start,
                    Trigger = trigger,
                    Issue = StateMachineValidationIssues.TRANSITION_OVERLAPS,
                };
            }

            if (!_sm.ContainsKey(end))
            {
                throw new StateMachineValidationException<State, Trigger>($"State {end} doesn't exist in state machine.")
                {
                    State = end,
                    Issue = StateMachineValidationIssues.STATE_MISSING,
                };
            }

            startTransitions.Add(trigger, end);
        }

        /// <summary>
        /// Get the next state in the system from a given state applying a given trigger
        /// </summary>
        public State NextState(State from, Trigger trigger)
        {
            if (!_sm.TryGetValue(from, out var startTransitions))
            {
                throw new StateMachineValidationException<State, Trigger>($"State {from} doesn't exist in state machine.")
                {
                    State = from,
                    Issue = StateMachineValidationIssues.STATE_MISSING,
                };
            }

            if (!startTransitions.TryGetValue(trigger, out var result))
            {
                throw new StateMachineValidationException<State, Trigger>($"State {from} with trigger {trigger} doesn't exist in state machine.")
                {
                    State = from,
                    Trigger = trigger,
                    Issue = StateMachineValidationIssues.NO_TRANSITION_EXISTS,
                };
            }

            return result;
        }
    }
}
