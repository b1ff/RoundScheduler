﻿using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using RoundScheduler.Model;
using RoundScheduler.Services;

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

        private void BeginingEditGridCellValue(object sender, DataGridBeginningEditEventArgs e)
        {
            var round = e.Row.DataContext as Round;
            if (round == null) return;

            if (e.Column.Header.ToString() == ProgramTexts.RoundTime && round.IsRoundPassed)
                e.Cancel = true;

            if (e.Column.Header.ToString() == ProgramTexts.RestTime && round.IsRestPassed)
                e.Cancel = true;
        }
    }
}
