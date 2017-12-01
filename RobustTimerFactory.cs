
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
        TimeSpan Duration { get; }
        TimeSpan Interval { get; }
        TimerState State { get; }

        void Start();
        void Pause();
        void Reset();
        void SetInterval(TimeSpan interval);
        void SetDuration(TimeSpan remainingTime);
    }

    public static class RobustTimerFactory
    {
        public static IRobustTimer Create(TimeSpan interval, TimeSpan timespan, Action<TimeSpan> onTick, Action onComplete = null)
        {
            return new RobustTimer(interval, timespan, TimerState.Stopped, onTick, onComplete);
        }

        public static IRobustTimer CreateAndPause(TimeSpan interval, TimeSpan timespan, Action<TimeSpan> onTick, Action onComplete = null)
        {
            return new RobustTimer(interval, timespan, TimerState.Paused, onTick, onComplete);
        }

        public static IRobustTimer CreateAndStartRunning(TimeSpan interval, TimeSpan timespan, Action<TimeSpan> onTick, Action onComplete = null)
        {
            var timer = new RobustTimer(interval, timespan, TimerState.Running, onTick, onComplete);
            timer.Start();
            return timer;
        }

        private class RobustTimer : IRobustTimer
        {
            private TimeSpan _originalDuration;

            private TimeSpan _duration;
            public TimeSpan Duration
            {
                get
                {
                    return _duration;
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

            public RobustTimer(TimeSpan interval, TimeSpan timespan, TimerState initialState, Action<TimeSpan> onTick, Action onComplete = null)
            {
                _interval = interval;
                _duration = timespan;
                _originalDuration = timespan;
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
                _duration = _originalDuration;
            }

            public void SetInterval(TimeSpan interval)
            {
                _interval = interval;
            }

            public void SetDuration(TimeSpan remainingTime)
            {
                _duration = remainingTime;
                _originalDuration = remainingTime;
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
                    _duration = _duration - _interval;

                    // if the timer has completed
                    if (_duration.TotalMilliseconds <= 0)
                    {
                        Reset();
                        _onComplete?.Invoke();

                        return false;
                    }

                    // fire the timer event
                    _onTick.Invoke(_duration);
                    return true;
                });
            }
        }
    }
}
