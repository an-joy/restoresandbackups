using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using RestoresAndBackups.Properties;
using Clipboard = System.Windows.Forms.Clipboard;
using MessageBox = System.Windows.MessageBox;

namespace RestoresAndBackups.Model
{
    public class MainModel : BaseViewItem
    {
        public ICommand CheckConnectionCommand { get; set; }
        public ICommand BackupCommand { get; set; }
        public ICommand RestoreCommand { get; set; }
        public ICommand PathChoiceCommand { get; set; }


        public static ConnectionStringViewModel ConnectionStringModel { get; set; } = new ConnectionStringViewModel();

        public InfoTabViewItem InfoTab { get; set; }


        public MainModel()
        {
            CheckConnectionCommand = new DelegateCommand(x => CheckConnection());
            BackupCommand = new DelegateCommand(x => Backup());
            PathChoiceCommand = new DelegateCommand(x => PathChoice());
            RestoreCommand = new DelegateCommand(x => Restore());


            InfoTab = new InfoTabViewItem(AppStates.Awaiting);

            if (ConnectionStringModel.SelectedAuthenticationKind.Value == null)
            {
                ConnectionStringModel.SelectedAuthenticationKind = ConnectionStringModel.AuthenticationKinds().FirstOrDefault(x => x.Key == AuthenticationKind.Windows);
            }
        }



        public void CheckConnection()
        {
            Thread thread = new Thread(CheckConnectionFunc);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void CheckConnectionFunc()
        {
            InfoTab = new InfoTabViewItem(AppStates.IsBusy);
            OnPropertyChanged(nameof(InfoTab));

            if (ConnectionStringModel.SelectedAuthenticationKind.Key == AuthenticationKind.Windows)
            {
                ConnectionStringModel.ConnectionString = $"Server={ConnectionStringModel.ServerNameText};Database=master;Trusted_Connection=True;";

                var isSuccessfull = ConnectionStringModel.IsSQLConnected(ConnectionStringModel.ConnectionString);

                if (isSuccessfull)
                {
                    SaveSettings();
                    InfoTab = new InfoTabViewItem(AppStates.Connected);
                }
                else
                {
                    InfoTab = new InfoTabViewItem(AppStates.ConnectionError);
                }
            }
            else
            {
                ConnectionStringModel.ConnectionString = $"Server={ConnectionStringModel.ServerNameText};Database={ConnectionStringModel.DbNameText};User Id={ConnectionStringModel.UserNameText};" +
                    $"Password={ConnectionStringModel.PasswordText};";

                var isSuccessfull = ConnectionStringModel.IsSQLConnected(ConnectionStringModel.ConnectionString);

                if (isSuccessfull)
                {
                    SaveSettings();
                    InfoTab = new InfoTabViewItem(AppStates.Connected);
                }
                else
                {
                    InfoTab = new InfoTabViewItem(AppStates.ConnectionError);
                }
            }
            OnPropertyChanged(nameof(InfoTab));
            
        }

        public void SaveSettings()
        {
            if (Settings.Default.ServerNames != null)
            {
                if (!Settings.Default.ServerNames.Contains(ConnectionStringModel.ServerNameText) && !string.IsNullOrEmpty(ConnectionStringModel.ServerNameText))
                {
                    Settings.Default.ServerNames.Add(ConnectionStringModel.ServerNameText);
                    Settings.Default.Save();
                }
            }
            else
            {
                Settings.Default.ServerNames = new System.Collections.Specialized.StringCollection();
                Settings.Default.ServerNames.Add(ConnectionStringModel.ServerNameText);
                Settings.Default.Save();
            }

            if (Settings.Default.DbNames != null)
            {
                if (!Settings.Default.DbNames.Contains(ConnectionStringModel.DbNameText) && !string.IsNullOrEmpty(ConnectionStringModel.DbNameText))
                {
                    Settings.Default.DbNames.Add(ConnectionStringModel.DbNameText);
                    Settings.Default.Save();
                }
            }
            else
            {
                Settings.Default.DbNames = new System.Collections.Specialized.StringCollection();
                Settings.Default.DbNames.Add(ConnectionStringModel.DbNameText);
                Settings.Default.Save();
            }

            if (ConnectionStringModel.SelectedAuthenticationKind.Key == AuthenticationKind.SQL)
            {
                if (Settings.Default.UserNames != null)
                {
                    if (!Settings.Default.UserNames.Contains(ConnectionStringModel.UserNameText) && !string.IsNullOrEmpty(ConnectionStringModel.UserNameText))
                    {
                        Settings.Default.UserNames.Add(ConnectionStringModel.UserNameText);
                        Settings.Default.Save();
                    }
                }
                else
                {
                    Settings.Default.UserNames = new System.Collections.Specialized.StringCollection();
                    Settings.Default.UserNames.Add(ConnectionStringModel.UserNameText);
                    Settings.Default.Save();
                }

                if (Settings.Default.IsSavePassword)
                {
                    Settings.Default.Password = ConnectionStringModel.PasswordText;
                    Settings.Default.Save();
                }
                else
                {
                    Settings.Default.Password = string.Empty;
                    Settings.Default.Save();
                }
            }
        }

        public void Backup()
        {
            Thread thread = new Thread(BackupFunc);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void BackupFunc()
        {
            if (InfoTab.AppState != AppStates.Awaiting)
            {
                InfoTab = new InfoTabViewItem(AppStates.IsBusy);
                OnPropertyChanged(nameof(InfoTab));

                try
                {
                    var fileName = $"{ConnectionStringModel.DbNameText} Backup {DateTime.Now.ToShortDateString()}.bak";

                    using (SqlConnection connection = new SqlConnection(ConnectionStringModel.ConnectionString))
                    {
                        var commandText = $@"BACKUP DATABASE {ConnectionStringModel.DbNameText} TO  DISK = N'{ConnectionStringModel.PathText}\{fileName}' 
                                          WITH NOFORMAT, INIT,  NAME = N'{ConnectionStringModel.DbNameText}-Полная База данных Резервное копирование', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";

                        var command = new SqlCommand(commandText, connection);
                        command.CommandText = commandText;
                        command.CommandTimeout = 3000;
                        connection.Open();
                        command.ExecuteNonQuery();
                    }

                    if (Settings.Default.Paths != null)
                    {
                        if (!Settings.Default.Paths.Contains(ConnectionStringModel.PathText) && !string.IsNullOrEmpty(ConnectionStringModel.PathText))
                        {
                            Settings.Default.Paths.Add(ConnectionStringModel.PathText);
                            Settings.Default.Save();
                        }
                    }
                    else
                    {
                        Settings.Default.Paths = new System.Collections.Specialized.StringCollection();
                        Settings.Default.Paths.Add(ConnectionStringModel.PathText);
                        Settings.Default.Save();
                    }

                    InfoTab = new InfoTabViewItem(AppStates.SuccessfullyBackuped);
                    OnPropertyChanged(nameof(InfoTab));
                }
                catch (Exception ex)
                {
                    Clipboard.SetText(ex.Message);
                    InfoTab = new InfoTabViewItem(AppStates.BackupError);
                    OnPropertyChanged(nameof(InfoTab));
                }
            }
            else
                MessageBox.Show("Проверьте соединение перед выполнением операции!");
        }

        public void Restore()
        {
            Thread thread = new Thread(RestoreFunc);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void RestoreFunc()
        {
            if (InfoTab.AppState != AppStates.Awaiting)
            {
                InfoTab = new InfoTabViewItem(AppStates.IsBusy);
                OnPropertyChanged(nameof(InfoTab));

                try
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionStringModel.ConnectionString))
                    {
                        var commandText = $@"RESTORE DATABASE {ConnectionStringModel.DbNameText} FROM DISK = '{ConnectionStringModel.PathText}'";

                        var command = new SqlCommand(commandText, connection);
                        command.CommandText = commandText;
                        command.CommandTimeout = 3000;
                        connection.Open();
                        command.ExecuteNonQuery();
                    }

                    if (Settings.Default.Paths != null)
                    {
                        if (!Settings.Default.Paths.Contains(ConnectionStringModel.PathText) && !string.IsNullOrEmpty(ConnectionStringModel.PathText))
                        {
                            Settings.Default.Paths.Add(ConnectionStringModel.PathText);
                            Settings.Default.Save();
                        }
                    }
                    else
                    {
                        Settings.Default.Paths = new System.Collections.Specialized.StringCollection();
                        Settings.Default.Paths.Add(ConnectionStringModel.PathText);
                        Settings.Default.Save();
                    }

                    InfoTab = new InfoTabViewItem(AppStates.SuccessfullyRestored);
                    OnPropertyChanged(nameof(InfoTab));
                }
                catch (Exception ex)
                {
                    Clipboard.SetText(ex.Message);
                    InfoTab = new InfoTabViewItem(AppStates.RestoreError);
                    OnPropertyChanged(nameof(InfoTab));
                }
            }
            else
                MessageBox.Show("Проверьте соединение перед выполнением операции!");
        }

        public void PathChoice()
        {
            if (ConnectionStringModel.IsPopUpOpened != true)
                ConnectionStringModel.IsPopUpOpened = true;
            else
                ConnectionStringModel.IsPopUpOpened = false;
        }


    }
}
