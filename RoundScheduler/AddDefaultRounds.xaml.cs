using System.Windows;
using RoundScheduler.Model;

namespace RoundScheduler
{
    /// <summary>
    /// Interaction logic for AddDefaultRounds.xaml
    /// </summary>
    public partial class AddDefaultRounds
    {
        public AddDefaultRounds(RoundSchedule roundSchedule)
        {
            this.DataContext = new RoundsInitializer(roundSchedule);

            InitializeComponent();
        }

        private void DialogClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
