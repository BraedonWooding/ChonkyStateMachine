using ChonkyStateMachine;
using NUnit.Framework;
using System;

namespace Tests
{
    public class DocumentationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        private class SimpleState
        {
            public string State;
            public string SubState;

            public SimpleState(string state, string subState = null)
            {
                this.State = state;
                this.SubState = subState;
            }

            public override bool Equals(object obj) =>
                obj is SimpleState state &&
                       State == state.State &&
                       SubState == state.SubState;

            public override int GetHashCode() => HashCode.Combine(State, SubState);
        }

        [Test]
        public void DocumentationTest_Lax()
        {
            var offHook = new SimpleState("OffHook");
            var ringing = new SimpleState("Ringing");
            var connected = new SimpleState("Connected");
            var disconnected = new SimpleState("Disconnected");

            // internal 'sub states'
            var connected_micMuted = new SimpleState("Connected", "Unmuted");
            var connected_micUnmuted = new SimpleState("Connected", "Muted");
            var ringing_retried = new SimpleState("Ringing", "Retried");

            var sm = new StateMachineBuilder<SimpleState, string>(lax: true)
                .AddStates(offHook, ringing, connected, disconnected, connected_micMuted, connected_micUnmuted, ringing_retried)
                .AddTransition(trigger: "CallDialed", from: offHook, to: ringing)
                // ... whatever transitions you want
                // want to define internal states? easy! want to allow a transition to be re-enterant?
                // Just define it to point to itself! (by default if no `to` is specified it's re-enterrant)
                .AddTransition(trigger: "Muted", from: connected_micUnmuted, to: connected_micMuted)
                .AddTransition(trigger: "Unmuted", from: connected_micMuted, to: connected_micUnmuted)
                .AddTransition(trigger: "RetryingConnection", from: ringing, to: ringing_retried)
                // and can specify an internal state to start from for exiting internal transitions, these will take priority over other transitions
                .AddTransition(trigger: "RetryingConnection", from: ringing_retried, to: disconnected)
                .Construct();
            // once constructed you can't modify a SM.

            // Note how the SM doesn't have 'our' state in it?  We don't even need a context
            // we can legitimately just say what would you do given this state and this trigger
            Assert.AreEqual(sm.NextState(from: offHook, "CallDialed"), ringing);
            Assert.AreEqual(sm.NextState(from: ringing, "RetryingConnection"), ringing_retried);
            Assert.AreEqual(sm.NextState(from: ringing_retried, "RetryingConnection"), disconnected);
        }
    }
}