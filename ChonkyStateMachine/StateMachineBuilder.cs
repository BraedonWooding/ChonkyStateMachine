using System;
using System.Collections.Generic;

namespace ChonkyStateMachine
{
    public class StateMachineBuilder<State, Transition>
        where State : class
    {
        private readonly StateMachine<State, Transition> _stateMachine;
        public bool IsConstructed { get; private set; } = false;
        public bool IsLax { get; }

        public StateMachineBuilder(bool lax = false)
        {
            this.IsLax = lax;
            this._stateMachine = new StateMachine<State, Transition>(IsLax);
        }

        public StateMachineBuilder<State, Transition> AddStates(params State[] states)
        {
            if (IsConstructed) throw new InvalidOperationException("StateMachine is already constructed, not modifiable now");

            foreach (var state in states)
            {
                _stateMachine.AddState(state);
            }

            return this;
        }

        public StateMachineBuilder<State, Transition> AddTransition(Transition trigger, State from, State to = null)
        {
            if (IsConstructed) throw new InvalidOperationException("StateMachine is already constructed, not modifiable now");

            _stateMachine.AddTransition(trigger, from, to ?? from);

            return this;
        }

        public StateMachine<State, Transition> Construct()
        {
            IsConstructed = true;
            return _stateMachine;
        }
    }
}
