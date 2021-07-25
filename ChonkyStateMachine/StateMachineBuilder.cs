using System;
using System.Collections.Generic;

namespace ChonkyStateMachine
{
    public sealed class StateMachineBuilder<State, Trigger>
        where State : class
    {
        private readonly StateMachine<State, Trigger> _stateMachine;
        public bool IsConstructed { get; private set; } = false;
        public bool IsLax { get; }

        /// <summary>
        /// Constructor a state machine given a series of configuration (i.e. states/transitions).
        /// </summary>
        /// <param name="lax"> If lax is enabled states don't have to be added manually and are implied based on their existence in transitions. </param>
        public StateMachineBuilder(bool lax = false)
        {
            this.IsLax = lax;
            this._stateMachine = new StateMachine<State, Trigger>(IsLax);
        }

        /// <summary>
        /// Add a series of states to the state machine
        /// </summary>
        public StateMachineBuilder<State, Trigger> AddStates(params State[] states)
        {
            if (IsConstructed) throw new InvalidOperationException("StateMachine is already constructed, not modifiable now");

            foreach (var state in states)
            {
                _stateMachine.AddState(state);
            }

            return this;
        }

        /// <summary>
        /// Add a transition to the state machine.
        /// 
        /// If in lax mode the states from/to will be created if they don't exist.
        /// </summary>
        /// <param name="trigger"> The trigger that causes the transition. </param>
        /// <param name="from"> The starting state for the transition. </param>
        /// <param name="to"> The resultant state for the transition, if not supplied (or null) this will become a re-enterant transition. </param>
        public StateMachineBuilder<State, Trigger> AddTransition(Trigger trigger, State from, State to = null)
        {
            if (IsConstructed) throw new InvalidOperationException("StateMachine is already constructed, not modifiable now");

            _stateMachine.AddTransition(trigger, from, to ?? from);

            return this;
        }

        /// <summary>
        /// Construct the resultant state machine given the configuration.
        /// </summary>
        public StateMachine<State, Trigger> Construct()
        {
            IsConstructed = true;
            return _stateMachine;
        }
    }
}
