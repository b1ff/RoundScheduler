using Microsoft.Win32;

namespace RoundScheduler.Services
{
    public static class FileDialogService
    {
        public static string FileNameFromOpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = ProgramTexts.FileDialog_XmlFileFilter;
            var showDialogResult = openFileDialog.ShowDialog();
            return showDialogResult.Value == true ? openFileDialog.FileName : string.Empty;
        }

        public static string FileNameFromSaveFileDialog()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = ProgramTexts.FileDialog_XmlFileFilter;
            var dialogResult = saveFileDialog.ShowDialog();
            return dialogResult.Value == true ? saveFileDialog.FileName : string.Empty;
        }
    }
}