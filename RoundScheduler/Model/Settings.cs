using System.IO;
using System.Windows.Input;
using RoundScheduler.Services;
using RoundScheduler.Utils;

namespace RoundScheduler.Model
{
    public class Settings : BasePropertyChanged
    {
        public void OpenSoundFile()
        {
            string initialDir = SoundFile.IsNotNullOrEmpty() ? Path.GetDirectoryName(SoundFile) : string.Empty;
            var soundFile = FileDialogService.FileNameFromOpenFileDialog(ProgramTexts.FileDialog_SoundFileFilter, initialDir);
            if (soundFile.IsNotNullOrEmpty())
                SoundFile = soundFile;
        }

        private string _soundFile;
        public string SoundFile
        {
            get { return _soundFile; }
            set
            {
                bool changed = value != _soundFile;
                _soundFile = value;
                if (changed) InvokePropertyChanged("SoundFile");
            }
        }

        private bool _needToLoopRestSound;
        public bool NeedToLoopRestSound
        {
            get { return _needToLoopRestSound; }
            set
            {
                bool changed = value != _needToLoopRestSound;
                _needToLoopRestSound = value;
                if (changed) InvokePropertyChanged("NeedToLoopRestSound");
            }
        }

        private ICommand _openSoundFileCommand;
        public ICommand OpenSoundFileCommand
        {
            get
            {
                if (_openSoundFileCommand == null)
                {
                    _openSoundFileCommand = new DelegateCommand(OpenSoundFile);
                }

                return _openSoundFileCommand;
            }
        }
    }
}
