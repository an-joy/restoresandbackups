using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RestoresAndBackups.Model
{
    public class InfoTabViewItem : BaseViewItem
    {
        public AppStates AppStates;
        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private string _image;

        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        private bool _isProgressVisible;

        public bool IsProgressVisible
        {
            get { return _isProgressVisible; }
            set
            {
                _isProgressVisible = value;
                OnPropertyChanged(nameof(IsProgressVisible));
            }
        }

        private bool _isUIActive = true;

        public bool IsUIActive
        {
            get { return _isUIActive; }
            set
            {
                _isUIActive = value;
                OnPropertyChanged(nameof(IsUIActive));
            }
        }

        public AppStates AppState { get; set; }

        public InfoTabViewItem(AppStates appStates)
        {
            switch (appStates)
            {
                case AppStates.Awaiting:
                    Message = "Заполните данные и проверьте соединение!";
                    Image = "/Images/info.png";
                    AppState = AppStates.Awaiting;
                    break;

                case AppStates.Connected:
                    Message = "Соединение прошло успешно, выберите операцию!";
                    Image = "/Images/ok.png";
                    AppState = AppStates.Connected;
                    break;

                case AppStates.ConnectionError:
                    Message = "Соединение прошло неудачно, ошибка скопирована в буфер обмена!";
                    Image = "/Images/error.png";
                    AppState = AppStates.ConnectionError;
                    break;

                case AppStates.SuccessfullyBackuped:
                    Message = "Резервное копирование завершено успешно!";
                    Image = "/Images/good.png";
                    AppState = AppStates.SuccessfullyBackuped;
                    break;

                case AppStates.SuccessfullyRestored:
                    Message = "Восстановление из копии выполнено успешно!";
                    Image = "/Images/good.png";
                    AppState = AppStates.SuccessfullyRestored;
                    break;

                case AppStates.BackupError:
                    Message = "Копирование прошло неудачно, ошибка скопирована в буфер обмена!";
                    Image = "/Images/error.png";
                    AppState = AppStates.BackupError;
                    break;

                case AppStates.RestoreError:
                    Message = "Восстановление прошло неудачно, ошибка скопирована в буфер обмена!";
                    Image = "/Images/error.png";
                    AppState = AppStates.RestoreError;
                    break;

                case AppStates.IsBusy:
                    Message = "Пожалуйста, ждите...";
                    IsProgressVisible = true;
                    IsUIActive = false;
                    AppState = AppStates.IsBusy;
                    break;
            }
        }
    }

    public enum AppStates
    {
        Awaiting,
        Connected,
        ConnectionError,
        RestoreError,
        BackupError,
        SuccessfullyRestored,
        SuccessfullyBackuped,
        IsBusy
    }
}
