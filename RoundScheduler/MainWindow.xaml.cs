using System.Windows.Controls;
using System.Windows.Data;
using RoundScheduler.Model;

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

            this._roundSchedule = new RoundSchedule();
            this.DataContext = _roundSchedule;
            this.AddTimeRangeColumn("Время раунда", "RoundTime");
            this.AddTimeRangeColumn("Время перерыва", "RestTime");

            this.InitializeRounds.Click += InitializeRoundsClick;
        }

        private void InitializeRoundsClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new AddDefaultRounds((RoundSchedule)this.DataContext);
            dialog.ShowDialog();
        }

        private void AddTimeRangeColumn(string columnHeader, string bindingPath)
        {
            var comboBoxColumn = new DataGridComboBoxColumn();
            comboBoxColumn.ItemsSource = _roundSchedule.DefaultRange;
            comboBoxColumn.SelectedValueBinding = new Binding(bindingPath);
            comboBoxColumn.Header = columnHeader;
            this.RoundsGrid.Columns.Add(comboBoxColumn);
        }
    }
}
