using System;
using System.Collections.Generic;

namespace ChonkyStateMachine
{
    public enum StateMachineValidationIssues
    {
        STATE_ALREADY_EXISTS,
        STATE_MISSING,
        TRANSITION_OVERLAPS,
        NO_TRANSITION_EXISTS,
    }

    public class StateMachineValidationException<TState, TTransition> : Exception
    {
        public TState State { get; set; }
        public TTransition Trigger { get; set; }

        public string Error { get; }
        public StateMachineValidationIssues Issue { get; set; }

        public StateMachineValidationException(string error) : base(error)
        {
            this.Error = error;
        }
    }
}
