using System.Windows;
using RoundScheduler.Model;
using RoundScheduler.Utils;

namespace RoundScheduler
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsDialog
    {
        private readonly Settings settings;

        public SettingsDialog()
        {
            InitializeComponent();

            this.settings = SettingsManager.CurrentSettings;
            DataContext = this.settings;
        }

        private void OnSoundFileFocus(object sender, RoutedEventArgs e)
        {
            settings.OpenSoundFile();
        }


        private void SaveSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsManager.SaveAsCurrentSettings(settings);
            Close();
        }
    }
}
