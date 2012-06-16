using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Input;
using RoundScheduler.Events;
using RoundScheduler.Utils;

namespace RoundScheduler.Model
{
    public class RoundSchedule : BasePropertyChanged
    {
        private const string pauseButtonText = "Пауза";
        private const string startButtonText = "Старт";

        public RoundSchedule()
        {
            Rounds = new ObservableCollection<Round>();
            Timer = new RoundTimer();
            OverralTime = TimeSpan.Zero;
            Rounds.CollectionChanged += RoundsTimeCollectionChanged;
            Timer.RestEnded += TimerRestEnded;
            Timer.RoundEnded += TimerRoundEnded;
            CurrentRoundIndex = 1;
            RestEndSound = RoundEndSound = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ring.wav");
            PauseButtonText = pauseButtonText;
            StartStopButtonText = startButtonText;
        }

        public RoundTimer Timer { get; private set; }

        private void TimerRoundEnded(object sender, RoundEndedEventArgs e)
        {
            var soundPlayer = new SoundPlayer(RestEndSound);
            soundPlayer.Play();
        }

        private void TimerRestEnded(object sender, RoundEndedEventArgs e)
        {
            ++CurrentRoundIndex;
            StartNextRound();
            var soundPlayer = new SoundPlayer(RestEndSound);
            soundPlayer.Play();
        }

        private void StartNextRound()
        {
            if (Rounds == null || !Rounds.Any())
                return;
            if (CurrentRoundIndex < Rounds.Count - 1)
                Timer.StartRound(Rounds[CurrentRoundIndex - 1]);
            else
                CurrentRoundIndex = 1;
        }

        private void ResetTimer()
        {
            Timer.StopCurrentRound();
            CurrentRoundIndex = 1;
        }

        private void RoundsTimeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SignRoundToChanges(e.NewItems);
            SignRoundToChanges(e.OldItems);
            InvokePropertyChanged("CanStart"); 

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.Cast<Round>().ForEach(x => OverralTime += x.GetOverallTime());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.Cast<Round>().ForEach(x => OverralTime -= x.GetOverallTime());
                    break;
            }
        }

        private void SignRoundToChanges(IList roundsCollection)
        {
            if (roundsCollection == null)
                return;

            var rounds = roundsCollection.Cast<Round>();
            foreach (var round in rounds)
            {
                round.PropertyChanged -= RoundTimeChanged;
                round.PropertyChanged += RoundTimeChanged;
            }
        }

        private void RoundTimeChanged(object sender, PropertyChangedEventArgs property)
        {
            // TODO: bad performance
            OverralTime = TimeSpan.Zero;
            Rounds.ForEach(x => OverralTime += x.GetOverallTime());
        }

        #region ViewModel_Properties 
        public ObservableCollection<Round> Rounds { get; private set; }

        public bool CanStart
        {
            get { return Rounds.IsNotNullOrEmpty(); }
        }

        private int _curentRoundIndex;
        public int CurrentRoundIndex
        {
            get { return _curentRoundIndex; }
            private set
            {
                bool changed = value != _curentRoundIndex;
                _curentRoundIndex = value;
                if (changed) InvokePropertyChanged("CurrentRoundIndex");
            }
        }

        private string _roundEndSound;
        public string RoundEndSound
        {
            get { return _roundEndSound; }
            set
            {
                bool changed = value != _roundEndSound;
                _roundEndSound = value;
                if (changed) InvokePropertyChanged("RoundEndSound");
            }
        }

        private string _restEndSound;
        public string RestEndSound
        {
            get { return _restEndSound; }
            set
            {
                bool changed = value != _restEndSound;
                _restEndSound = value;
                if (changed) InvokePropertyChanged("RestEndSound");
            }
        }

        private string _pauseButtonText;
        public string PauseButtonText
        {
            get { return _pauseButtonText; }
            set
            {
                bool changed = value != _pauseButtonText;
                _pauseButtonText = value;
                if (changed) InvokePropertyChanged("PauseButtonText");
            }
        }

        private string _startStopButtonText;
        public string StartStopButtonText
        {
            get { return _startStopButtonText; }
            set
            {
                bool changed = value != _startStopButtonText;
                _startStopButtonText = value;
                if (changed) InvokePropertyChanged("StartStopButtonText");
            }
        }

        private TimeSpan _overralTime;
        public TimeSpan OverralTime
        {
            get { return _overralTime; }
            private set
            {
                bool changed = value != _overralTime;
                _overralTime = value;
                if (changed) InvokePropertyChanged("OverralTime");
            }
        }


        private static readonly Lazy<IEnumerable<TimeSpan>> _defaultRange = new Lazy<IEnumerable<TimeSpan>>(
            () => Enumerable.Range(1, 20).Select(x => TimeSpan.FromSeconds(10 * x)));

        public IEnumerable<TimeSpan> DefaultRange
        {
            get { return _defaultRange.Value; }
        }
        #endregion

        #region Commands

        private ICommand _startTimerCommand;
        public ICommand StartTimerCommand
        {
            get
            {
                if (_startTimerCommand == null)
                {
                    _startTimerCommand = new DelegateCommand(() =>
                    {
                        if (Timer.IsRunning)
                        {
                            StartStopButtonText = startButtonText;
                            ResetTimer();
                        }
                        else
                        {
                            StartStopButtonText = "Остановить";
                            StartNextRound();
                        }
                    });
                }

                return _startTimerCommand;
            }
        }

        private ICommand _pauseTimerCommand;
        public ICommand PauseTimerCommand
        {
            get
            {
                if (_pauseTimerCommand == null)
                {
                    _pauseTimerCommand = new DelegateCommand(() =>
                    {
                        Timer.PauseTimer();
                        PauseButtonText = Timer.IsPaused ? "Продолжить" : pauseButtonText;
                    });
                }
                return _pauseTimerCommand;
            }
        }

        #endregion
    }
}
