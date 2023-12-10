using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Password.Models;
using Password.Services;
using Rg.Plugins.Popup.Services;
using Password.Views;
using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
using System;
using Xamarin.Essentials;

namespace Password.ViewModels
{
    public class PopupAddViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Credential> Credentials { get; set; } = new ObservableCollection<Credential>();

        private Credential_DAO _credentialDAO;
        private UserData_DAO _userDataDAO;
        private AboutViewModel _aboutViewModel;

        public Command AddAppCommand { get; }
        public Command ClosePopupCommand { get; }

        private string _passwordApp;
        public string PasswordApp
        {
            get { return _passwordApp; }
            set
            {
                _passwordApp = value;
                PasswordStrength = EvaluatePasswordStrength.CheckStrength(value);
                OnPropertyChanged();
            }
        }

        private EvaluatePasswordStrength.PasswordStrength _passwordStrength;
        public EvaluatePasswordStrength.PasswordStrength PasswordStrength
        {
            get { return _passwordStrength; }
            set
            {
                _passwordStrength = value;
                OnPropertyChanged();
            }
        }

        private string _application;
        public string Application
        {
            get { return _application; }
            set
            {
                _application = value;
                OnPropertyChanged();
            }
        }

        private string _userNameApp;
        public string UserNameApp
        {
            get { return _userNameApp; }
            set
            {
                _userNameApp = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _url;
        public string URL
        {
            get { return _url; }
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        public PopupAddViewModel()
        {
            AddAppCommand = new Command(ExecuteAddAppCommand);
            ClosePopupCommand = new Command(ExecuteClosePopupCommand);

            _credentialDAO = new Credential_DAO();
            _userDataDAO = new UserData_DAO();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ExecuteAddAppCommand()
        {
            var encryptedPassword = EncryptionService.Encrypt(this.PasswordApp);
            string userId = Preferences.Get("IdUser", "");

            var newCredential = new Credential
            {
                Application = this.Application,
                UserNameApp = this.UserNameApp,
                PasswordApp = encryptedPassword,
                Email = this.Email,
                URL = this.URL,
                CreatedDate = DateTime.Now.ToString(),
                UserId = userId,
                LastModifiedDate = null
            };

            await _credentialDAO.AddCredential(newCredential);

            // Reset properties
            this.Application = "";
            this.UserNameApp = "";
            this.PasswordApp = "";
            this.Email = "";
            this.URL = "";

            await PopupNavigation.Instance.PopAsync();
            _aboutViewModel?.RefreshData();
        }

        private async void ExecuteClosePopupCommand()
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
