using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;
using RoundScheduler.Dto;
using RoundScheduler.Events;
using RoundScheduler.Services;
using RoundScheduler.Utils;

namespace RoundScheduler.Model
{
    public class RoundSchedule : BasePropertyChanged
    {
        private readonly XmlSerializer _roundsSerializer = new XmlSerializer(typeof(List<RoundSerializable>));

        private readonly SoundPlayer restSoundPlayer;

        public RoundSchedule()
        {
            Rounds = new ObservableCollection<Round>();
            Timer = new RoundTimer();
            OverralTime = TimeSpan.Zero;
            Rounds.CollectionChanged += RoundsTimeCollectionChanged;
            Timer.RestEnded += TimerRestEnded;
            Timer.RoundEnded += TimerRoundEnded;
            Timer.FiveSecondsBeforeRestEnd += TimerFiveSecondsBeforeRestEnd;
            CurrentRoundIndex = 1;
            RestEndSound = RoundEndSound = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ring.wav");
            PauseButtonText = ProgramTexts.Pause;
            StartStopButtonText = ProgramTexts.Start;
            RestGongTimer = new DispatcherTimer();
            RestGongTimer.Tick += RestTimerTick;
            RestGongTimer.Interval = TimeSpan.FromSeconds(1);
            restSoundPlayer = new SoundPlayer(RestEndSound);
        }
        
        public RoundTimer Timer { get; private set; }

        private DispatcherTimer RestGongTimer { get; set; }

        private void RestTimerTick(object sender, EventArgs eventArgs)
        {
            restSoundPlayer.Stop();
            restSoundPlayer.Play(); 
        }
        
        private void TimerFiveSecondsBeforeRestEnd(object sender, EventArgs e)
        {
            RestGongTimer.Start();
        }

        private void TimerRoundEnded(object sender, RoundEndedEventArgs e)
        {
            var soundPlayer = new SoundPlayer(RestEndSound);
            soundPlayer.Play();
        }

        private void TimerRestEnded(object sender, RoundEndedEventArgs e)
        {
            RestGongTimer.Stop();
            ++CurrentRoundIndex;
            StartNextRound();
        }

        private void StartNextRound()
        {
            if (Rounds.IsNullOrEmpty())
                return;
            if (CurrentRoundIndex <= Rounds.Count)
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

        private void SaveRounds()
        {
            if (Rounds.IsNullOrEmpty()) return;

            var fileName = FileDialogService.FileNameFromSaveFileDialog();
            if (fileName.IsNullOrEmpty()) return;
            
            using (var sw = new StreamWriter(fileName))
            {
                var roundsToSerialize = Rounds.Select(x => new RoundSerializable(x)).ToList();
                _roundsSerializer.Serialize(sw, roundsToSerialize);
            }
        }

        private void LoadRounds()
        {
            var fileName = FileDialogService.FileNameFromOpenFileDialog();
            if (fileName.IsNullOrEmpty()) return;

            try
            {
                using (var sr = new StreamReader(fileName))
                {
                    var deserializedRounds = (List<RoundSerializable>)_roundsSerializer.Deserialize(sr);
                    Rounds.Clear();
                    deserializedRounds.ForEach(x => Rounds.Add(x.GetRound()));
                }
            }
            catch
            {
                MessageBox.Show(ProgramTexts.ErrorWhileSavingRoundsToFile);
            }
        }

        #region ViewModel_Properties 

        public ObservableCollection<Round> Rounds { get; private set; }

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

        private ICommand _saveRoundsCommand;
        public ICommand SaveRoundsComand
        {
            get
            {
                if (_saveRoundsCommand == null)
                {
                    _saveRoundsCommand = new DelegateCommand(SaveRounds, () => Rounds.IsNotNullOrEmpty());
                }

                return _saveRoundsCommand;
            }
        }

        private ICommand _loadRoundsCommand;
        public ICommand LoadRoundsCommand
        {
            get
            {
                if (_loadRoundsCommand == null)
                {
                    _loadRoundsCommand = new DelegateCommand(LoadRounds);
                }

                return _loadRoundsCommand;
            }
        }

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
                            StartStopButtonText = ProgramTexts.Start;
                            ResetTimer();
                        }
                        else
                        {
                            StartStopButtonText = ProgramTexts.Stop;
                            StartNextRound();
                        }
                    }, () => Rounds.IsNotNullOrEmpty());
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
                        PauseButtonText = Timer.IsPaused ? ProgramTexts.Continue : ProgramTexts.Pause;
                    }, 
                    () => Timer.IsRunning);
                }
                return _pauseTimerCommand;
            }
        }

        #endregion
    }
}
