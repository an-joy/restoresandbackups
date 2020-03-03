using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace RestoresAndBackups.Model
{
    class PathPrintModel : BaseViewItem
    {
        public ICommand ChooseDirectoryCommand { get; set; }
        public ICommand ChooseFileCommand { get; set; }

        public ConnectionStringViewModel ConnectionStringModel { get; set; } = new ConnectionStringViewModel();

        public PathPrintModel()
        {
            ChooseDirectoryCommand = new DelegateCommand(x => ChooseDirectory());
            ChooseFileCommand = new DelegateCommand(x => ChooseFile());
            ConnectionStringModel = MainModel.ConnectionStringModel;
        }

        public void ChooseDirectory()
        {
            ConnectionStringModel.IsPopUpOpened = false;
            var folderDialog = new FolderBrowserDialog();
            folderDialog.Description = $"Выберите директорию для резервного копирования {ConnectionStringModel.DbNameText}:";
            var result = folderDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderDialog.SelectedPath))
            {
                ConnectionStringModel.PathText = folderDialog.SelectedPath;
                OnPropertyChanged(nameof(ConnectionStringModel));
            }
        }

        public void ChooseFile()
        {
            ConnectionStringModel.IsPopUpOpened = false;
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Backup files (*.bak) | *.bak";
            fileDialog.Title = "Открыть файл восстановления";
            var result = fileDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrEmpty(fileDialog.FileName))
            {
                ConnectionStringModel.PathText = fileDialog.FileName;
                OnPropertyChanged(nameof(ConnectionStringModel));
            }
        }
    }
}
