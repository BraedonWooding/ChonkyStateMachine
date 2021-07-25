using System;
using System.Collections.Generic;

namespace ChonkyStateMachine
{
    /// <summary>
    /// Possible validation issues for the state machine.
    /// </summary>
    public enum StateMachineValidationIssues
    {
        /// <summary>
        /// The given state already exists in the system.
        /// 
        /// Ignored if lax mode is enabled.
        /// </summary>
        STATE_ALREADY_EXISTS,

        /// <summary>
        /// The given state is missing from the system.
        /// 
        /// Ignored if lax mode is enabled except for the case
        /// when you are querying the resultant state machine
        /// using invalid states (which will still result in this error).
        /// </summary>
        STATE_MISSING,

        /// <summary>
        /// The transition overlaps with an identical transition.
        /// That is A --(X)--> B overlaps with A' --(X')--> C
        /// given that A' == A and X' == X
        /// </summary>
        TRANSITION_OVERLAPS,

        /// <summary>
        /// The specified trigger doesn't relate to any real transition.
        /// </summary>
        NO_TRANSITION_EXISTS,
    }

    public class StateMachineValidationException<TState, TTrigger> : Exception
    {
        /// <summary>
        /// The state that the action was performed from.
        /// </summary>
        public TState State { get; set; }

        /// <summary>
        /// The trigger that was performed.
        /// </summary>
        public TTrigger Trigger { get; set; }

        /// <summary>
        /// The generic type of issue that ocurred.
        /// </summary>
        public StateMachineValidationIssues Issue { get; set; }

        public StateMachineValidationException(string error) : base(error)
        {
        }
    }
}
