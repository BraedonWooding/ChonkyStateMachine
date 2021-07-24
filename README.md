# ChonkyStateMachine

> By Braedon Wooding

A simple, short, and sweet 200 line C# State Machine that is *fast* and easy to understand/use.

## Example

```csharp
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
```

If `lax` is set to false (default) then you'll have to add each state manually like so;

```csharp
var sm = new StateMachineBuilder<SimpleState, string>(lax: true)
    .AddStates(offHook, ringing, connected, disconnected, connected_micMuted, connected_micUnmuted, ringing_retried)
    // .AddTransition... and so on
```

## Benchmarks

Compared to [Stateless](https://github.com/dotnet-state-machine/stateless).  This is a pretty simple benchmark though that doesn't really test substates / more complicated scenarios.

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19042.1110 (20H2/October2020Update)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.205
  [Host]     : .NET Core 3.1.17 (CoreCLR 4.700.21.31506, CoreFX 4.700.21.31502), X64 RyuJIT
  DefaultJob : .NET Core 3.1.17 (CoreCLR 4.700.21.31506, CoreFX 4.700.21.31502), X64 RyuJIT

|                   Method |           Mean |         Error |         StdDev |         Median |      Gen 0 |      Gen 1 | Gen 2 |     Allocated |
|------------------------- |---------------:|--------------:|---------------:|---------------:|-----------:|-----------:|------:|--------------:|
|     StatelessCreateNever |     844.325 us |    16.4782 us |     15.4137 us |     837.415 us |   342.7734 |          - |     - |   1,077,182 B |
|        ChonkyCreateNever |      68.500 us |     1.2992 us |      1.6893 us |      68.108 us |          - |          - |     - |             - |
| StatelessCreateEveryTime | 214,516.330 us | 4,089.2581 us | 10,772.7415 us | 211,344.000 us | 61333.3333 | 15333.3333 |     - | 218,416,160 B |
|    ChonkyCreateEverytime | 162,594.792 us | 3,215.2304 us |  6,346.5512 us | 163,338.917 us | 18666.6667 |          - |     - |  59,020,800 B |
|      StatelessCreateOnce |   1,304.168 us |    25.9425 us |     67.4281 us |   1,294.157 us |   423.8281 |   138.6719 |     - |   1,350,968 B |
|         ChonkyCreateOnce |     278.942 us |     5.4722 us |      8.6795 us |     278.011 us |    21.4844 |          - |     - |      68,896 B |
| StatelessCostConstructor |       5.497 us |     0.1473 us |      0.4273 us |       5.447 us |     2.5711 |          - |     - |       8,072 B |
|    ChonkyCostConstructor |       3.255 us |     0.0628 us |      0.1049 us |       3.229 us |     1.0185 |          - |     - |       3,200 B |

Outliers
  Benchmark.StatelessCreateNever: Default     -> 2 outliers were removed (919.47 us, 970.07 us)
  Benchmark.StatelessCreateEveryTime: Default -> 4 outliers were removed (247.29 ms..331.06 ms)
  Benchmark.StatelessCreateOnce: Default      -> 8 outliers were removed (1.54 ms..1.86 ms)
  Benchmark.StatelessCostConstructor: Default -> 3 outliers were removed (6.86 us..8.66 us)
  Benchmark.ChonkyCreateEverytime: Default    -> 3 outliers were removed (185.43 ms..203.07 ms)
  Benchmark.ChonkyCreateNever: Default        -> 3 outliers were removed (74.58 us..80.10 us)

> Constructor tests creation of a 10 object State Machine.  The rest test a full traversal of a 800 transition state machine.

> Tests were written in a way that was as similar as possible to each other while making it as optimal to the specific implementation as possible.

As you can see Chonky beats Stateless in every test.  It does extraordinary at the case which I cared about (CreateNever) where you never create the state machine since it's already constructed (i.e. within a constructor or somewhere).

The motivation for this was that we had some runtime defined state machine and had a ton of different items entering in different states, we wanted a very easy way to determine which states were possible for the object to move into.  With Stateless we have to create the whole state machine individually for every object which takes up about 1 MB per object, for a lot of objects (800 for example in CreateEveryTime) you can easily see how this can add up a lot.

Chonky automatically drops in as a performance improving replacement here, but it also acts as a beautiful improvement to let us just initialise it once.  The fact that Chonky has 0 memory overhead after creation of the state machine is just perfect though and really does show how 'simple' it is.  It consists *purely* of around 200 lines of code compared to the almost 4000 lines of code in Stateless (though a mute point since Stateless does a lot more).

It is nice to see also that Chonky has a significantly lower memory overhead in general, in the create once test it was a 20x difference!
