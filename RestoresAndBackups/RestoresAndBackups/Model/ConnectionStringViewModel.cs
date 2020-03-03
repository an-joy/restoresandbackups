using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Media;

namespace RestoresAndBackups.Model
{
    public class ConnectionStringViewModel : BaseViewItem
    {
        public AuthenticationKind AuthenticationKind;

        public Brush ForegroundBrush { get; set; }

        public string ConnectionString { get; set; }

        private string _serverNameText;

        public string ServerNameText
        {
            get { return _serverNameText; }
            set
            {
                _serverNameText = value;
                OnPropertyChanged(nameof(ServerNameText));
            }
        }

        private string _dbNameText;

        public string DbNameText
        {
            get { return _dbNameText; }
            set
            {
                _dbNameText = value;
                OnPropertyChanged(nameof(DbNameText));
            }
        }

        private string _userNameText;

        public string UserNameText
        {
            get { return _userNameText; }
            set
            {
                _userNameText = value;
                OnPropertyChanged(nameof(UserNameText));
            }
        }

        private string _passwordText;

        public string PasswordText
        {
            get { return _passwordText; }
            set
            {
                _passwordText = value;
                OnPropertyChanged(nameof(PasswordText));
            }
        }

        private string _pathText;

        public string PathText
        {
            get { return _pathText; }
            set
            {
                _pathText = value;
                OnPropertyChanged(nameof(PathText));
            }
        }

        private bool _isSQLAuth;

        public bool IsSQLAuth
        {
            get { return _isSQLAuth; }
            set
            {
                _isSQLAuth = value;
                OnPropertyChanged(nameof(IsSQLAuth));
            }
        }

        private bool _isPopUpOpened;

        public bool IsPopUpOpened
        {
            get { return _isPopUpOpened; }
            set
            {
                _isPopUpOpened = value;
                OnPropertyChanged(nameof(IsPopUpOpened));
            }
        }

        private KeyValuePair<AuthenticationKind, string> _selectedAuthenticationKind;

        public KeyValuePair<AuthenticationKind, string> SelectedAuthenticationKind
        {
            get { return _selectedAuthenticationKind; }
            set
            {
                _selectedAuthenticationKind = value;

                if (_selectedAuthenticationKind.Key == AuthenticationKind.Windows)
                {
                    UserNameText = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    IsSQLAuth = false;
                    ForegroundBrush = Brushes.Gray;
                    Properties.Settings.Default.IsSavePassword = false;
                    PasswordText = string.Empty;
                    OnPropertyChanged(nameof(PasswordText));
                }
                else
                {
                    if (Properties.Settings.Default.UserNames != null)
                        UserNameText = Properties.Settings.Default.UserNames[0];
                    else
                        UserNameText = string.Empty;

                    if (!string.IsNullOrEmpty(Properties.Settings.Default.Password))
                        PasswordText = Properties.Settings.Default.Password;


                    IsSQLAuth = true;
                    ForegroundBrush = Brushes.Black;
                    Properties.Settings.Default.IsSavePassword = true;
                }

                OnPropertyChanged(nameof(SelectedAuthenticationKind));
                OnPropertyChanged(nameof(ForegroundBrush));
            }
        }

        public Dictionary<AuthenticationKind, string> AuthenticationKinds()
        {
            Dictionary<AuthenticationKind, string> kinds = new Dictionary<AuthenticationKind, string>();
            kinds.Add(AuthenticationKind.Windows, "Проверка подлинности Windows");
            kinds.Add(AuthenticationKind.SQL, "Проверка подлинности SQL Server");
            return kinds;
        }

        public bool IsSQLConnected(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    connection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    Clipboard.SetText(ex.Message);
                    return false;
                }
            }
        }
    }

    public enum AuthenticationKind
    {
        Windows,
        SQL
    }
}
