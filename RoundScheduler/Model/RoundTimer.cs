using System;
using System.Windows.Threading;
using RoundScheduler.Events;

namespace RoundScheduler.Model
{
    public class RoundTimer : BasePropertyChanged
    {
        private Round _round;
        private DispatcherTimer _timer;
        private bool _isRest;

        private TimeSpan _currentRoundTime;
        private TimeSpan _currentRestTime;
        private TimeSpan _overralPassed;

        public event RoundEndedEventHandler RoundEnded;
        public event RoundEndedEventHandler RestEnded;
        public event EventHandler FiveSecondsBeforeRestEnd;

        public RoundTimer()
        {
            CurrentRestTime = TimeSpan.Zero;
            CurrentRoundTime = TimeSpan.Zero;

            InitializeTimer();
        }

        public bool IsRunning { get { return _timer.IsEnabled; } }

        public bool IsRoundRunning { get { return !_isRest && _timer.IsEnabled; } }

        public bool IsRestRunning { get { return _isRest && _timer.IsEnabled; } }

        public TimeSpan CurrentRestTime
        {
            get { return _currentRestTime; }
            set
            {
                bool changed = value != _currentRestTime;
                _currentRestTime = value;
                if (changed)
                {
                    InvokePropertyChanged("CurrentRestTime");
                    InvokePropertyChanged("RestTimeLeft");
                }
            }
        }

        public TimeSpan CurrentRoundTime
        {
            get { return _currentRoundTime; }
            set
            {
                bool changed = value != _currentRoundTime;
                _currentRoundTime = value;
                if (changed)
                { 
                    InvokePropertyChanged("CurrentRoundTime");
                    InvokePropertyChanged("RoundTimeLeft");
                }
            }
        }

        public TimeSpan RoundTimeLeft
        {
            get
            {
                if (_round == null)
                    return TimeSpan.Zero;
                return _round.RoundTime - CurrentRoundTime;
            }
        }

        public TimeSpan RestTimeLeft
        {
            get
            {
                if (_round == null)
                    return TimeSpan.Zero;
                return _round.RestTime - CurrentRestTime;
            }
        }

        public TimeSpan OverralPassed
        {
            get { return _overralPassed; }
            set
            {
                bool changed = value != _overralPassed;
                _overralPassed = value;
                if (changed) InvokePropertyChanged("OverralPassed");
            }
        }

        public void StartRound(Round round)
        {
            if (round == null)
                throw new ArgumentNullException("round");

            _round = round;
            InvokePropertyChanged("RestTimeLeft");
            InvokePropertyChanged("RoundTimeLeft");
            _timer.Start();
        }

        public void StopCurrentRound()
        {
            _timer.Stop();
            ResetRoundTime();
            _round = null;
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += TimerTick;
        }

        public bool IsPaused { get; set; }
        public void PauseTimer()
        {
            IsPaused = !IsPaused;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (_round == null)
                _timer.Stop();

            if (IsPaused)
                return;

            if (_isRest)
                RestTimerTick();
            else
                RoundTimerTick();
                
            OverralPassed += TimeSpan.FromSeconds(1);
        }

        private void RoundTimerTick()
        {
            if (CurrentRoundTime == _round.RoundTime)
            {
                _isRest = true;
                _round.IsRoundPassed = true;
                if (RoundEnded != null)
                    RoundEnded(this, new RoundEndedEventArgs(_round));
                return;
            }

            CurrentRoundTime += TimeSpan.FromSeconds(1);
        }

        private void RestTimerTick()
        {
            if ((CurrentRestTime == _round.RestTime - TimeSpan.FromSeconds(4)) && FiveSecondsBeforeRestEnd != null)
            {
                FiveSecondsBeforeRestEnd(this, EventArgs.Empty);
            }

            if (CurrentRestTime == _round.RestTime)
            {
                _isRest = false;
                _round.IsRestPassed = true;
                _timer.Stop();
                ResetRoundTime();
                if (RoundEnded != null)
                    RestEnded(this, new RoundEndedEventArgs(_round));
                return;
            }

            CurrentRestTime += TimeSpan.FromSeconds(1);
        }

        private void ResetRoundTime()
        {
            CurrentRoundTime = TimeSpan.Zero;
            CurrentRestTime = TimeSpan.Zero;
        }
    }
}
