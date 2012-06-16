using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using RoundScheduler.Utils;

namespace RoundScheduler.Model
{
    public class RoundsInitializer : BasePropertyChanged
    {
        private readonly RoundSchedule _schedule;

        private static readonly Lazy<IEnumerable<int>> _defaultRoundRange = new Lazy<IEnumerable<int>>(
            () => Enumerable.Range(1, 30));

        public RoundsInitializer(RoundSchedule schedule)
        {
            _schedule = schedule;
        }

        private int _roundsCount;

        public int RoundsCount
        {
            get { return _roundsCount; }
            set
            {
                bool changed = value != _roundsCount;
                _roundsCount = value;
                if (changed) InvokePropertyChanged("RoundsCount");
            }
        }

        private TimeSpan _defaultRoundTime;

        public TimeSpan DefaultRoundTime
        {
            get { return _defaultRoundTime; }
            set
            {
                bool changed = value != _defaultRoundTime;
                _defaultRoundTime = value;
                if (changed) InvokePropertyChanged("DefaultRoundTime");
            }
        }

        private TimeSpan _defaultRestTime;

        public TimeSpan DefaultRestTime
        {
            get { return _defaultRestTime; }
            set
            {
                bool changed = value != _defaultRestTime;
                _defaultRestTime = value;
                if (changed) InvokePropertyChanged("DefaultRestTime");
            }
        }

        public IEnumerable<TimeSpan> DefaultTimeRange
        {
            get { return _schedule.DefaultRange; }
        }

        public IEnumerable<int> DefaultRoundRange
        {
            get { return _defaultRoundRange.Value; }
        }

        #region Commands
        private ICommand _setupRounds;
        
        public ICommand SetupRoundsCommand
        {
            get
            {
                if (_setupRounds == null)
                    _setupRounds = new DelegateCommand(SetupRounds);
                return _setupRounds;
            }
        }
        #endregion

        private void SetupRounds()
        {
            if (RoundsCount > 0 
                && DefaultRestTime != default(TimeSpan) 
                && DefaultRoundTime != default(TimeSpan))
            {
                _schedule.Rounds.Clear();
                Enumerable.Range(0, RoundsCount)
                    .Select(x => new Round(DefaultRoundTime, DefaultRestTime))
                    .ForEach(x => _schedule.Rounds.Add(x));
            }
        }
    }
}
