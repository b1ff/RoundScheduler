using System.IO;
using Microsoft.Win32;
using RoundScheduler.Utils;

namespace RoundScheduler.Services
{
    public static class FileDialogService
    {
        public static string FileNameFromOpenFileDialog(string fileFilter, string initialDir = null)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = fileFilter;
            if (initialDir.IsNotNullOrEmpty() && Directory.Exists(initialDir))
            {
                openFileDialog.InitialDirectory = initialDir;
            }
            
            var showDialogResult = openFileDialog.ShowDialog();
            return showDialogResult.Value == true ? openFileDialog.FileName : string.Empty;
        }

        public static string FileNameFromSaveFileDialog(string fileFilter)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = fileFilter;
            var dialogResult = saveFileDialog.ShowDialog();
            return dialogResult.Value == true ? saveFileDialog.FileName : string.Empty;
        }
    }
}