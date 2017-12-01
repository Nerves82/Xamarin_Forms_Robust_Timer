# Xamarin Forms Robust Timer
A robust wrapper around the Xamarin Forms Timer implementation.

Xamarin Forms allows only the most primitive use of a timer via its "Device" class. 
https://developer.xamarin.com/api/member/Xamarin.Forms.Device.StartTimer/p/System.TimeSpan/System.Func%7BSystem.Boolean%7D/

The robust timer is designed to wrap the simple Xamarin Forms mechanism and provide basic functionality such as starting, stopping, pausing, resuming, setting a new duration, setting a new interval and querying for remaining duration and timer state.

The timer is instantiated using a static factory class and outside access to the timer is limited via the use of an interface.
```C#
IRobustTimer timer = RobustTimerFactory.Create();
```

The interval between timer “ticks” and the amount of time the timer will run are passed into the Create methods as TimeSpans.
```C#
RobustTimerFactory.Create(TimeSpan.FromSeconds(1));
```

There is 1 required action and one optional action that need to be supplied to the timer. The “tick” action, which is executed one per interval returning the remaining duration and the optional "completed" action which is executed when the timer has run for the provided duration.

These actions can be supplied as either predefined Actions;
```C#
RobustTimerFactory.Create(TimerTickedAction);
Action<TimeSpan> TimerTickAction => TimerTick;
private void TimerTick(TimeSpan remainingTime) {    }
```

Or lambdas;
```C#
RobustTimerFactory.Create((timeLeft) => { }, () => { }); 
```

## Usage Example
```C#
IRobustTimer timer = RobustTimerFactory.CreateAndStartRunning(
    TimeSpan.FromSeconds(1),
    TimeSpan.FromHours(4),
    (timeRemaining) => { Console.WriteLine($"Time Remaining: {timeRemaining}") },
    () => { Console.WriteLine("Timer Complete")});

Console.WriteLine($"Timer tickes every: {timer.Interval}";

Console.WriteLine($"Timer will run for: {timer.RemainingTime}";

Console.WriteLine($"Timer is: {timer.State}"; // running

timer.Pause();

Console.WriteLine($"Timer is: {timer.State}"; // paused

timer.Reset();
```
