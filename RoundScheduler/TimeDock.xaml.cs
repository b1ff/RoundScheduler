using RoundScheduler.Model;

namespace RoundScheduler
{
	/// <summary>
	/// Interaction logic for TimeDock.xaml
	/// </summary>
	public partial class TimeDock
	{
		public TimeDock(TimeDockViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}
