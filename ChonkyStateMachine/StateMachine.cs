using System;
using System.Collections.Generic;

namespace ChonkyStateMachine
{
    public class StateMachine<State, Transition>
    {
        private readonly Dictionary<State, Dictionary<Transition, State>> _sm = new Dictionary<State, Dictionary<Transition, State>>();
        private readonly bool _isLax;

        public StateMachine(bool isLax)
        {
            this._isLax = isLax;
        }

        internal void AddState(State state)
        {
            if (_sm.ContainsKey(state))
            {
                if (!_isLax)
                {
                    throw new StateMachineValidationException<State, Transition>($"State {state} already exists in state machine.")
                    {
                        State = state,
                        Issue = StateMachineValidationIssues.STATE_ALREADY_EXISTS
                    };
                }
            }
            else
            {
                _sm.Add(state, new Dictionary<Transition, State>());
            }
        }

        internal void AddTransition(Transition trigger, State start, State end)
        {
            if (_isLax)
            {
                AddState(start);
                AddState(end);
            }

            if (!_sm.TryGetValue(start, out var startTransitions))
            {
                throw new StateMachineValidationException<State, Transition>($"State {start} doesn't exist in state machine.")
                {
                    State = start,
                    Issue = StateMachineValidationIssues.STATE_MISSING,
                };
            }

            if (startTransitions.ContainsKey(trigger))
            {
                throw new StateMachineValidationException<State, Transition>($"State {start} with trigger {trigger} already exists in state machine.")
                {
                    State = start,
                    Trigger = trigger,
                    Issue = StateMachineValidationIssues.TRANSITION_OVERLAPS,
                };
            }

            if (!_sm.ContainsKey(end))
            {
                throw new StateMachineValidationException<State, Transition>($"State {end} doesn't exist in state machine.")
                {
                    State = end,
                    Issue = StateMachineValidationIssues.STATE_MISSING,
                };
            }

            startTransitions.Add(trigger, end);
        }

        public State NextState(State from, Transition trigger)
        {
            if (!_sm.TryGetValue(from, out var startTransitions))
            {
                throw new StateMachineValidationException<State, Transition>($"State {from} doesn't exist in state machine.")
                {
                    State = from,
                    Issue = StateMachineValidationIssues.STATE_MISSING,
                };
            }

            if (!startTransitions.TryGetValue(trigger, out var result))
            {
                throw new StateMachineValidationException<State, Transition>($"State {from} with trigger {trigger} doesn't exist in state machine.")
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
