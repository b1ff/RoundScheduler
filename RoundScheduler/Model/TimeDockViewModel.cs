using System;
using System.ComponentModel;

namespace RoundScheduler.Model
{
	public class TimeDockViewModel : BasePropertyChanged
	{
		private readonly RoundSchedule _roundSchedule;
		private TimeSpan _time;

		public TimeDockViewModel(RoundSchedule roundSchedule)
		{
			_roundSchedule = roundSchedule;
			roundSchedule.Timer.PropertyChanged += TimeChanged;
		}

		public int RoundIndex
		{
			get { return _roundSchedule.CurrentRoundIndex; }
		}

		public TimeSpan Time
		{
			get { return _time; }
			set
			{
				bool changed = value != _time;
				_time = value;
				if (changed) InvokePropertyChanged("Time");
			}
		}

		private void TimeChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "RoundTimeLeft")
			{
				Time = _roundSchedule.Timer.RoundTimeLeft;
			}
			else if (args.PropertyName == "RestTimeLeft")
			{
				Time = _roundSchedule.Timer.RestTimeLeft;
			}
			else if(args.PropertyName == "CurrentRoundIndex")
			{
				InvokePropertyChanged("RoundIndex");
			}
		}
	}
}
