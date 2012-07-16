using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using RoundScheduler.Model;
using RoundScheduler.Utils;

namespace RoundScheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly RoundSchedule _roundSchedule;

        public MainWindow()
        {
            InitializeComponent();

            _roundSchedule = new RoundSchedule();
            this.DataContext = _roundSchedule;
            this.AddTimeRangeColumn(ProgramTexts.RoundTime, "RoundTime");
            this.AddTimeRangeColumn(ProgramTexts.RestTime, "RestTime");

            this.InitializeRounds.Click += InitializeRoundsClick;
            this.Settings.Click += ShowSettings;
        }

        private void InitializeRoundsClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new AddDefaultRounds((RoundSchedule)this.DataContext);
            dialog.ShowDialog();
        }

        private void ShowSettings(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new SettingsDialog();
            dialog.ShowDialog();
        }

        private void AddTimeRangeColumn(string columnHeader, string bindingPath)
        {
            var comboBoxColumn = new DataGridComboBoxColumn();
            comboBoxColumn.ItemsSource = _roundSchedule.DefaultRange;
            comboBoxColumn.SelectedValueBinding = new Binding(bindingPath);
            comboBoxColumn.Header = columnHeader;
            comboBoxColumn.CanUserSort = false;
            
            this.RoundsGrid.Columns.Add(comboBoxColumn);
        }

        private void BeginingEditGridCellValue(object sender, DataGridBeginningEditEventArgs e)
        {
            var round = e.Row.DataContext as Round;
            if (round == null) return;

            if (e.Column.Header.ToString() == ProgramTexts.RoundTime && round.IsRoundPassed)
                e.Cancel = true;

            if (e.Column.Header.ToString() == ProgramTexts.RestTime && round.IsRestPassed)
                e.Cancel = true;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                _roundSchedule.PauseTimerCommand.Execute(null);
            }
        }
    }
}
