
using System;
using Xamarin.Forms;

namespace mtda.Framework
{
    public enum TimerState
    {
        Stopped,
        Running,
        Paused
    }

    public interface IRobustTimer
    {
        TimeSpan RemainingTime { get; }
        TimeSpan Interval { get; }
        TimerState State { get; }

        void Start();
        void Pause();
        void Reset();
    }

    public static class RobustTimerFactory
    {
        public static IRobustTimer Create(TimeSpan interval, TimeSpan timespan, Action<TimeSpan> onTick, Action onComplete)
        {
            return new RobustTimer(interval, timespan, TimerState.Stopped, onTick, onComplete);
        }

        public static IRobustTimer CreateAndPause(TimeSpan interval, TimeSpan timespan, Action<TimeSpan> onTick, Action onComplete)
        {
            return new RobustTimer(interval, timespan, TimerState.Paused, onTick, onComplete);
        }

        public static IRobustTimer CreateAndStartRunning(TimeSpan interval, TimeSpan timespan, Action<TimeSpan> onTick, Action onComplete)
        {
            var timer = new RobustTimer(interval, timespan, TimerState.Running, onTick, onComplete);
            timer.Start();
            return timer;
        }

        private class RobustTimer : IRobustTimer
        {
            private TimeSpan _remainingTime;
            public TimeSpan RemainingTime
            {
                get
                {
                    return _remainingTime;
                }
            }

            private TimeSpan _interval;
            public TimeSpan Interval
            {
                get
                {
                    return _interval;
                }
            }

            private TimerState _state;
            public TimerState State
            {
                get { return _state; }
            }

            readonly Action<TimeSpan> _onTick;
            readonly Action _onComplete;

            public RobustTimer(TimeSpan interval, TimeSpan timespan, TimerState initialState, Action<TimeSpan> onTick, Action onComplete)
            {
                _interval = interval;
                _remainingTime = timespan;
                _state = initialState;
                _onTick = onTick;
                _onComplete = onComplete;
            }

            public void Start()
            {
                _state = TimerState.Running;

                RunTimer();
            }

            public void Pause()
            {
                _state = TimerState.Paused;
            }

            public void Reset()
            {
                _state = TimerState.Stopped;
                _remainingTime = new TimeSpan();
                _interval = new TimeSpan();
            }

            private void RunTimer()
            {
                Device.StartTimer(_interval, () =>
                {
                    if (_state != TimerState.Running)
                    {
                        return false;
                    }

                    // reduce the remaining time by the interval
                    _remainingTime = _remainingTime - _interval;

                    // if the timer has completed
                    if (_remainingTime.TotalMilliseconds <= 0)
                    {
                        Reset();
                        _onComplete.Invoke();

                        return false;
                    }

                    // fire the timer event
                    _onTick.Invoke(_remainingTime);
                    return true;
                });
            }
        }
    }
}
