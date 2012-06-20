using System;

namespace RoundScheduler.Model
{
    [Serializable]
    public class Round : BasePropertyChanged
    {
        public Round()
        {
        }

        public Round(TimeSpan roundTime, TimeSpan restTime)
        {
            _roundTime = roundTime;
            _restTime = restTime;
        }

        private TimeSpan _roundTime;
        public TimeSpan RoundTime
        {
            get { return _roundTime; }
            set
            {
                if (_roundTime == value)
                    return;

                _roundTime = value;
                InvokePropertyChanged("RoundTime");
            }
        }

        private TimeSpan _restTime;
        public TimeSpan RestTime
        {
            get { return _restTime; }
            set 
            {
                if (_restTime == value)
                    return;
                
                _restTime = value;
                InvokePropertyChanged("RestTime");
            }
        }

        private bool _isRoundPassed;
        public bool IsRoundPassed
        {
            get { return _isRoundPassed; }
            set
            {
                bool changed = value != _isRoundPassed;
                _isRoundPassed = value;
                if (changed) InvokePropertyChanged("IsRoundPassed");
            }
        }

        private bool _isRestPassed;
        public bool IsRestPassed
        {
            get { return _isRestPassed; }
            set
            {
                bool changed = value != _isRestPassed;
                _isRestPassed = value;
                if (changed) InvokePropertyChanged("IsRestPassed");
            }
        }

        public TimeSpan GetOverallTime()
        {
            return RoundTime + RestTime;
        }
    }
}
