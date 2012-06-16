using System;

namespace RoundScheduler.Model
{
    public class Round : BasePropertyChanged
    {
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

        public TimeSpan GetOverallTime()
        {
            return RoundTime + RestTime;
        }
    }
}
